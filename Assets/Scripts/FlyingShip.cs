using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingShip : MonoBehaviour {
    // All the logic is stored here, no data

    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject workerPrefab;

    // Data:
    [HideInInspector] public FlyingShipData data;
    [HideInInspector] public Choice choice1;
    [HideInInspector] public Choice choice2;

    [HideInInspector] public List<Error> errors = new List<Error>();

    // Internal references:
    private Rigidbody2D rigid;
    [SerializeField] private Transform blades;
    [SerializeField] private Transform droppoint;

    // Private variables
    private Dictionary<int, List<Worker>> assignedDictionary = new Dictionary<int, List<Worker>>();
    private float foodProcuctionTimer;
    private float materialsProductionTimer;
    private float foodConsumptionTimer;
    private int moduloAssigned;
    private float boostTimer;

    private float errorTimer;


    public bool land = false;
    private bool landed = false;

    private void Start() {
        rigid = GetComponent<Rigidbody2D>();

        AssignWorkersRandomly();
        CalculateWeight();
        data.maximumWeight = data.currentWeight - 50f;

        assignedDictionary.Add(0, data.assignedKitchen);
        assignedDictionary.Add(1, data.assignedWorkshop);
        assignedDictionary.Add(2, data.assignedEngines);

        foodProcuctionTimer = GameManager.instance.difficultySetting.FoodProcessTimer;
        materialsProductionTimer = GameManager.instance.difficultySetting.MaterialProcessTimer;
        foodConsumptionTimer = GameManager.instance.difficultySetting.FoodConsumptionTimer / data.workers.Count;

        choice1 = GenerateChoice();
        choice2 = GenerateChoice();

        errorTimer = GameManager.instance.difficultySetting.ErrorInterval;
    }

    private void FixedUpdate() {
        if (data.altitude > 0 && data.altitude < 1800f) {

            if (!land) {
                rigid.MovePosition(new Vector2(-5f, (-5f) + (data.altitude * 5f) / 1000));
            }
            else {
                rigid.MovePosition(Vector2.Lerp(transform.position, new Vector2(-5f, -4f), 0.02f));

                if (Vector2.Distance(transform.position, new Vector2(-5f, -4f)) < 0.1f && ! landed) {
                    landed = true;
                    UIManager.instance.WinMenu();
                }
            }
        }
        else if (data.altitude < 0) {
            rigid.MovePosition(Vector2.Lerp(transform.position, new Vector2(-5f, -10f), 0.02f));
            GameManager.instance.LoseGame("Crashed in the sea");
        }
        else if (data.altitude > 1800f) {
            rigid.MovePosition(Vector2.Lerp(transform.position, new Vector2(-5f, 10f), 0.02f));
            GameManager.instance.LoseGame("Went up to space");
        }
    }

    private void Update() {
        data.flightTime += Time.deltaTime;
        boostTimer -= Time.deltaTime;
        errorTimer -= Time.deltaTime;

        // Food and material process
        foodProcuctionTimer -= Time.deltaTime * data.assignedKitchen.Count / Mathf.Clamp(errors.FindAll(x => x.id == "Kitchen Damage").Count * 1.2f, 1f, 10f);
        materialsProductionTimer -= Time.deltaTime * data.assignedWorkshop.Count / Mathf.Clamp(errors.FindAll(x => x.id == "Workshop Damage").Count * 1.2f, 1f, 10f); ;

        if (foodProcuctionTimer < 0f && data.supplies > 0) {
            foodProcuctionTimer = GameManager.instance.difficultySetting.FoodProcessTimer;
            data.food++;
            data.supplies--;
        }

        if (materialsProductionTimer < 0f && data.supplies > 0) {
            materialsProductionTimer = GameManager.instance.difficultySetting.MaterialProcessTimer;
            data.materials++;
            data.supplies--;
        }

        // Food consumption
        foodConsumptionTimer -= Time.deltaTime;

        if (foodConsumptionTimer < 0) {
            if (data.food > 0) {
                data.food--;
                data.waste++;
                foodConsumptionTimer = GameManager.instance.difficultySetting.FoodConsumptionTimer / data.workers.Count;
            }
            else {
                GameManager.instance.LoseGame("Ran out of food");
                data.workers = new List<Worker>();
                data.assignedKitchen = new List<Worker>();
                data.assignedEngines = new List<Worker>();
                data.assignedWorkshop = new List<Worker>();
            }

        }

        // Errors
        if (errorTimer < 0) {
            errors.Add(GenerateError());
            errorTimer = GameManager.instance.difficultySetting.ErrorInterval;
        }


        // Status logic
        data.maximumWeight -= GameManager.instance.difficultySetting.SinkSpeed / 10f * Time.deltaTime * Mathf.Clamp(errors.FindAll(x => x.id == "Balloon Damage").Count * 1.2f, 1f, 10f);
        data.altitude += Time.deltaTime * (data.maximumWeight - data.currentWeight) / data.maximumWeight * 50f;

        float speed = 5f + data.assignedEngines.Count;
        if (errors.FindAll(x => x.id == "Hull Damage").Count > 0) {
            speed = speed / (errors.FindAll(x => x.id == "Hull Damage").Count * 1.2f);
        }
        speed += Random.Range(-1, 1f);
        speed = speed * ((boostTimer > 0) ? 2 : 1);

        data.speed = (!land) ? Mathf.Lerp(data.speed, speed, 0.003f) : Mathf.Lerp(data.speed, 0, 0.01f);
        data.distance += data.speed * Time.deltaTime;


        CalculateWeight();

        // Animations
        blades.Rotate(new Vector3(0, 0, Mathf.Clamp(data.speed * 20f, 0f, 60f)));

    }

    private void CalculateWeight() {
        data.currentWeight = data.supplies + data.food + data.waste + data.materials + (100 * data.workers.Count);
    }

    private void AssignWorkersRandomly() {
        foreach (Worker worker in data.workers) {
            float rand = Random.Range(0, 1f);

            if (rand <= 0.33f) {
                data.assignedKitchen.Add(worker);
            }
            else if (rand <= 0.66f) {
                data.assignedWorkshop.Add(worker);
            }
            else {
                data.assignedEngines.Add(worker);
            }
        }
    }

    // Assignes workers to a department, 0 = kitchen, 1 = workshop, 2 = engines
    public void AssignWorker(int id, int number) {


        if (assignedDictionary.ContainsKey(id)) {
            while (assignedDictionary[id].Count != number) {

                // We need to fill the spot with other workers evenly
                if (assignedDictionary[id].Count < number && moduloAssigned != id && assignedDictionary[moduloAssigned].Count > 0) {
                    Worker worker = assignedDictionary[moduloAssigned][0];
                    assignedDictionary[moduloAssigned].Remove(worker);
                    assignedDictionary[id].Add(worker);
                }
                // We need to place workers on other jobs evenly
                else if (assignedDictionary[id].Count > number && moduloAssigned != id) {
                    Worker worker = assignedDictionary[id][0];
                    assignedDictionary[id].Remove(worker);
                    assignedDictionary[moduloAssigned].Add(worker);
                }

                moduloAssigned = (moduloAssigned + 1) % assignedDictionary.Count;
            }
        }
    }

    public Choice GenerateChoice() {
        // Choices can be:
        // 0: Supplies
        // 1: Food
        // 2: Materials
        // 3: Waste
        // 4: Worker

        int choiceId = Random.Range(0, 5);


        Choice choice = new Choice();

        if (choiceId == 0) {
            choice.id = "Supplies";
            choice.amount = Random.Range(Mathf.RoundToInt(data.supplies / 20f), Mathf.RoundToInt(data.supplies / 5f));
            choice.weight = choice.amount;
            choice.color = "<color=#00CFFF>";

        } else if (choiceId == 1) {
            choice.id = "Food";
            choice.amount = Random.Range(Mathf.RoundToInt(data.food / 10f), Mathf.RoundToInt(data.food / 5f));
            choice.weight = choice.amount;
            choice.color = "<color=#00CF00>";

            if (choice.amount == 0) {
                return GenerateChoice();
            }
        }
        else if (choiceId == 2) {
            choice.id = "Materials";
            choice.amount = Random.Range(Mathf.RoundToInt(data.materials / 10f), Mathf.RoundToInt(data.materials / 5f));
            choice.weight = choice.amount;
            choice.color = "<color=#F55018>";


            if (choice.amount == 0) {
                return GenerateChoice();
            }
        }
        else if (choiceId == 3) {
            choice.id = "Waste";
            choice.amount = Random.Range(Mathf.RoundToInt(data.waste / 10f), Mathf.RoundToInt(data.waste / 5f));
            choice.weight = choice.amount;
            choice.color = "<color=#8e8e8e>";


            if (choice.amount == 0) {
                return GenerateChoice();
            }
        }
        else if (choiceId == 4) {
            choice.id = "Worker";
            Worker worker = null;
            int i = Random.Range(0, assignedDictionary.Count);
            choice.color = "<color=#3DEB99>";


            while (worker == null) {
                if (assignedDictionary[i].Count > 0) {
                    worker = assignedDictionary[i][0];
                }
                else {
                    i = (i + 1) % assignedDictionary.Count;
                }
            }
            choice.weight = 100;

            choice.worker = worker;
        }

        return choice;
    }

    public Error GenerateError() {
        int errorId = Random.Range(0, 4);

        Error error = new Error();

        if (errorId == 0) {
            error.id = "Hull Damage";
            error.effect = "Decreased Speed";
            error.cost = Random.Range(6, 15 + Mathf.RoundToInt(data.flightTime / 30));
        }
        else if (errorId == 1) {
            error.id = "Balloon Damage";
            error.effect = "Increased Sink Speed";
            error.cost = Random.Range(6, 15 + Mathf.RoundToInt(data.flightTime / 30));
        }
        else if (errorId == 2) {
            error.id = "Kitchen Damage";
            error.effect = "Decresed Food Production";
            error.cost = Random.Range(4, 10 + Mathf.RoundToInt(data.flightTime / 30));
        }
        else if (errorId == 3) {
            error.id = "Workshop Damage";
            error.effect = "Decresed Material Production";
            error.cost = Random.Range(4, 10 + Mathf.RoundToInt(data.flightTime / 30));
        }

        return error;
    }

    public void FixError(Error error) {
        if (data.materials > error.cost) {
            data.materials -= error.cost;
            data.waste += error.cost;

            errors.Remove(error);
        }
    }

    public void SelectChoice(int id) {

        Choice choice = (id == 0) ? choice1 : choice2;

        if (choice.id == "Supplies") {
            data.supplies -= choice.amount;
        }
        else if (choice.id == "Food") {
            data.food -= choice.amount;
        }
        else if (choice.id == "Materials") {
            data.materials -= choice.amount;
        }
        else if (choice.id == "Waste") {
            data.waste -= choice.amount;
        }
        else if (choice.worker != null) {

            if (data.assignedKitchen.Contains(choice.worker)) {
                data.assignedKitchen.Remove(choice.worker);
            } else if (data.assignedWorkshop.Contains(choice.worker)) {
                data.assignedWorkshop.Remove(choice.worker);
            }
            else if (data.assignedEngines.Contains(choice.worker)) {
                data.assignedEngines.Remove(choice.worker);
            }

            data.workers.Remove(choice.worker);
        }



        StartCoroutine(ChoiceDelay());

        if (choice.worker != null) {
            Instantiate(workerPrefab, droppoint.position, Quaternion.identity);
        }
        else {
            int rand = Random.Range(1, 4);
            for (int i = 0; i < rand; i++) {
                StartCoroutine(DropBoxes(i * Random.Range(0.2f, 1f)));
            }
        }

        choice1 = null;
        choice2 = null;
    }

    public void WasteBoost() {
        if (data.waste >= GameManager.instance.difficultySetting.WasteBoost) {
            data.waste -= GameManager.instance.difficultySetting.WasteBoost;
            boostTimer = (boostTimer > 0) ? boostTimer + 20f: 20f;
        }
    }

    IEnumerator DropBoxes(float delay) {
        yield return new WaitForSeconds(delay);
        Instantiate(boxPrefab, droppoint.position, Quaternion.identity);
    }

    IEnumerator ChoiceDelay() {
        yield return new WaitForSeconds(Random.Range(5f, 10f));

        choice1 = GenerateChoice();
        choice2 = GenerateChoice();
    }
}
