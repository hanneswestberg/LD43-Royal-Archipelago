using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public DifficultySetting difficultySetting;

    // References:
    [SerializeField] private GameObject shipPrefab;
    [SerializeField] private Transform shipSpawnPoint;
    [SerializeField] private AudioSource bladeSource;
    [SerializeField] private AudioSource winSource;

    [SerializeField] private GameObject landPrefab;
    [SerializeField] private Transform landPosition;


    [HideInInspector] public GameObject theShipGO = null;
    [HideInInspector] public FlyingShip theShip = null;

    public bool gameIsOn = false;

    private bool spawnedLand;

    // Use this for initialization
    void Awake () {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
	}


    private void Start() {
        //SpawnShip();
        UIManager.instance.ShowStartMenu(true);
    }

    private void Update() {


        bladeSource.volume = (theShip != null && gameIsOn) ? Mathf.Clamp(theShip.data.speed / 200f, 0f, 0.05f) : 0;

        if (theShip != null) {
            // Audio
            bladeSource.pitch = (theShip.data.speed / 3f);


            if (theShip.data.distance > difficultySetting.WinDistance && !spawnedLand) {
                // Spawn land:
                spawnedLand = true;

                GameObject land = Instantiate(landPrefab, landPosition.position, Quaternion.identity);

            }
        }
    }


    public void SpawnShip() {
        gameIsOn = true;
        theShipGO = (GameObject)Instantiate(shipPrefab, shipSpawnPoint.position, Quaternion.identity);
        theShip = theShipGO.GetComponent<FlyingShip>();

        List<Worker> workerList = new List<Worker>();
        for (int i = 0; i < difficultySetting.Workers; i++) {
            workerList.Add(new Worker() { name = "Dave " + i });
        }

        theShip.data = new FlyingShipData() {
            supplies = difficultySetting.Supplies,
            food = difficultySetting.Food,
            materials = difficultySetting.Materials,
            workers = workerList,
            altitude = 1000
        };
    }

    public void LoseGame(string reason) {
        if (gameIsOn) {
            UIManager.instance.LoseMenu(reason);
            winSource.Play();
            gameIsOn = false;
        }

    }

    public void WinGame() {
        if (gameIsOn) {
            theShip.land = true;
            winSource.Play();
            gameIsOn = false;
        }
    }

    public void Restart() {
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }


    public int  CalculateScore() {

        float score = 0;

        score += (theShip.data.supplies * 3);
        score += (theShip.data.food * 10);
        score += (theShip.data.materials * 10);
        score += (theShip.data.workers.Count * 1000);

        score = score * (theShip.data.distance/5000f);



        return Mathf.RoundToInt(score);

    }

}
