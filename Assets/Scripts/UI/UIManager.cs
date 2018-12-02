using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager instance = null;

    // Status Panel Up
    [SerializeField] private Text maximumWeight;
    [SerializeField] private Text currentWeight;
    [SerializeField] private Text altitude;
    [SerializeField] private Text speed;
    [SerializeField] private Text distance;

    // Status Panel Low
    [Header("Status Panel Low")]
    [SerializeField] private Text supplies;
    [SerializeField] private Text food;
    [SerializeField] private Text waste;
    [SerializeField] private Text materials;

    // Personell Panel
    [Header("Personell Panel")]
    [SerializeField] private Slider kitchenSlider;
    [SerializeField] private Slider workshopSlider;
    [SerializeField] private Slider enginesSlider;
    [SerializeField] private Text avaliableWorkers;
    [SerializeField] private Text assignedKitchen;
    [SerializeField] private Text assignedWorkshop;
    [SerializeField] private Text assignedEngines;
    [SerializeField] private Button wasteBoost;

    [Header("Damage Panel")]
    [SerializeField] private Transform panelParent;
    [SerializeField] private Transform damagePanel;
    [SerializeField] private GameObject errorGO;
    [HideInInspector] public List<GameObject> currentErrors = new List<GameObject>();

    [Header("Altitude Meter")]
    [SerializeField] private Image fillerUpper;
    [SerializeField] private Image fillerLower;


    [Header("Choices Text")]
    [SerializeField] private Text choice1;
    [SerializeField] private Text choice2;
    [SerializeField] private Text choice1Weight;
    [SerializeField] private Text choice2Weight;
    [SerializeField] private Image item1;
    [SerializeField] private Image item2;
    [SerializeField] private Sprite worker;
    [SerializeField] private Sprite box;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource switchSource;
    [SerializeField] public AudioSource uiSource;
    [SerializeField] private AudioSource warningSource;
    [SerializeField] private AudioSource drapesSource;
    [SerializeField] public AudioSource fixSource;
    [SerializeField] private AudioSource engineSource;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip switchSound;
    [SerializeField] private AudioClip warningSound;
    [SerializeField] public AudioClip buttonSound;
    [SerializeField] public AudioClip fixSound;
    [SerializeField] private AudioClip engineSound;
    [SerializeField] private AudioClip drapesSound;

    [Header("Tutorial")]
    [SerializeField] private GameObject[] tutorials;
    private int tutorialIndex;

    [Header("StartMenu")]
    [SerializeField] private GameObject startMenu;

    [Header("LoseMenu")]
    [SerializeField] private GameObject loseMenu;
    [SerializeField] private Text loseReason;
    [SerializeField] private Text loseScoreText;


    [Header("Won Menu")]
    [SerializeField] private GameObject winMenu;
    [SerializeField] private Text winScoreText;


    // Private variables
    private Dictionary<Slider, int> sliderDictionary = new Dictionary<Slider, int>();
    private Animator anim;
    private float lastAltitude;
    private bool playedSound;


    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        sliderDictionary.Add(kitchenSlider, 0);
        sliderDictionary.Add(workshopSlider, 1);
        sliderDictionary.Add(enginesSlider, 2);

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {

        if (GameManager.instance.theShip != null) {
            maximumWeight.text = GameManager.instance.theShip.data.maximumWeight.ToString("F1");
            currentWeight.text = GameManager.instance.theShip.data.currentWeight.ToString("F1");
            altitude.text = GameManager.instance.theShip.data.altitude.ToString("F1");
            speed.text = GameManager.instance.theShip.data.speed.ToString("F1");
            distance.text = GameManager.instance.theShip.data.distance.ToString("F1");

            supplies.text = "<color=#00CFFF>" + GameManager.instance.theShip.data.supplies.ToString() + "</color>";
            food.text = "<color=#00CF00>" + GameManager.instance.theShip.data.food.ToString() + "</color>";
            waste.text = "<color=#8e8e8e>" + GameManager.instance.theShip.data.waste.ToString() + "</color>";
            materials.text = "<color=#F55018>" + GameManager.instance.theShip.data.materials.ToString() + "</color>";


            // Personell Panel
            avaliableWorkers.text = "<color=#3DEB99>" + GameManager.instance.theShip.data.workers.Count.ToString() + "</color>";
            assignedKitchen.text = "<color=#3DEB99>" + GameManager.instance.theShip.data.assignedKitchen.Count.ToString() + "</color>";
            assignedWorkshop.text = "<color=#3DEB99>" + GameManager.instance.theShip.data.assignedWorkshop.Count.ToString() + "</color>";
            assignedEngines.text = "<color=#3DEB99>" + GameManager.instance.theShip.data.assignedEngines.Count.ToString() + "</color>";

            kitchenSlider.maxValue = GameManager.instance.theShip.data.workers.Count;
            workshopSlider.maxValue = GameManager.instance.theShip.data.workers.Count;
            enginesSlider.maxValue = GameManager.instance.theShip.data.workers.Count;

            if (kitchenSlider.value != GameManager.instance.theShip.data.assignedKitchen.Count ||
                workshopSlider.value != GameManager.instance.theShip.data.assignedWorkshop.Count ||
                enginesSlider.value != GameManager.instance.theShip.data.assignedEngines.Count) {

                // Sliders must update their value
                kitchenSlider.value = GameManager.instance.theShip.data.assignedKitchen.Count;
                workshopSlider.value = GameManager.instance.theShip.data.assignedWorkshop.Count;
                enginesSlider.value = GameManager.instance.theShip.data.assignedEngines.Count;
            }

            wasteBoost.interactable = (GameManager.instance.theShip.data.waste >= GameManager.instance.difficultySetting.WasteBoost);

            foreach (Error error in GameManager.instance.theShip.errors) {
                if (currentErrors.Find(x => x.GetComponent<UIError>().thisError == error) == null) {
                    GameObject newError = Instantiate(errorGO, panelParent);
                    newError.GetComponent<UIError>().thisError = error;
                    currentErrors.Add(newError);
                    warningSource.PlayOneShot(warningSound);
                    damagePanel.GetComponent<Animator>().SetTrigger("Warning");
                }
            }


            if (lastAltitude == 0) {
                lastAltitude = GameManager.instance.theShip.data.altitude;
            }

            fillerUpper.fillAmount = (1f - (GameManager.instance.theShip.data.currentWeight / GameManager.instance.theShip.data.maximumWeight)) * 4f;
            fillerLower.fillAmount = ((GameManager.instance.theShip.data.currentWeight / GameManager.instance.theShip.data.maximumWeight ) - 1f) * 4f;

            lastAltitude = GameManager.instance.theShip.data.altitude;

            // Animations
            anim.SetBool("HaveChoices", GameManager.instance.theShip.choice1 != null);

            if (GameManager.instance.theShip.choice1 != null) {
                if (!playedSound) {
                    drapesSource.PlayOneShot(drapesSound);
                    playedSound = true;
                }

                choice1.text = GameManager.instance.theShip.choice1.color + GameManager.instance.theShip.choice1.id + ": " + ((GameManager.instance.theShip.choice1.worker == null) ? GameManager.instance.theShip.choice1.amount.ToString() : "") + "</color>";
                choice2.text = GameManager.instance.theShip.choice2.color + GameManager.instance.theShip.choice2.id + ": " + ((GameManager.instance.theShip.choice2.worker == null) ? GameManager.instance.theShip.choice2.amount.ToString() : "") + "</color>";

                item1.sprite = (GameManager.instance.theShip.choice1.worker != null) ?  worker: box;
                item2.sprite = (GameManager.instance.theShip.choice2.worker != null) ?  worker: box;

                choice1Weight.text = "Weight: " + GameManager.instance.theShip.choice1.weight.ToString();
                choice2Weight.text = "Weight: " + GameManager.instance.theShip.choice2.weight.ToString();
            }
            else {
                playedSound = false;
            }
        }
    }

    public void StartTutorial() {
        ShowStartMenu(false);
        tutorialIndex = 0;
        tutorials[tutorialIndex].SetActive(true);
        uiSource.PlayOneShot(buttonSound);

    }

    public void NextPage() {
        tutorialIndex++;
        if (tutorials.Length > tutorialIndex) {
            tutorials[tutorialIndex].SetActive(true);

            if ((tutorialIndex - 1) >= 0) {
                tutorials[tutorialIndex - 1].SetActive(false);
            }
        }
        else {
            foreach (GameObject obj in tutorials) {
                obj.SetActive(false);
                ShowStartMenu(true);
            }
        }
        uiSource.PlayOneShot(buttonSound);

    }

    public void PreviousPage() {
        tutorialIndex--;
        if (tutorialIndex > 0) {
            tutorials[tutorialIndex].SetActive(true);

            if ((tutorialIndex + 1) >= 0) {
                tutorials[tutorialIndex + 1].SetActive(false);
            }
        }
        else {
            tutorialIndex = tutorials.Length - 1;

            tutorials[tutorialIndex].SetActive(true);
            tutorials[0].SetActive(false);
        }
        uiSource.PlayOneShot(buttonSound);

    }

    public void ShowStartMenu(bool show) {
        startMenu.SetActive(show);
    }

    public void StartGame() {
        GameManager.instance.SpawnShip();
        ShowStartMenu(false);
        loseMenu.SetActive(false);
        uiSource.PlayOneShot(buttonSound);
    }

    public void SliderAssignWorker(Slider slider) {
        if (GameManager.instance.theShip != null) {
            GameManager.instance.theShip.AssignWorker(sliderDictionary[slider], Mathf.RoundToInt(slider.value));

            switchSource.pitch = Random.Range(0.8f, 1.2f);
            switchSource.PlayOneShot(switchSound);
        }
    }

    public void ButtonSelectChoice(int id) {
        GameManager.instance.theShip.SelectChoice(id);
        source.PlayOneShot(dropSound);
        uiSource.PlayOneShot(buttonSound);
        drapesSource.PlayOneShot(drapesSound);
    }

    public void ButtonWasteBoost() {
        GameManager.instance.theShip.WasteBoost();
        uiSource.PlayOneShot(buttonSound);
        engineSource.PlayOneShot(engineSound);
    }

    public void LoseMenu(string reason) {
        loseReason.text = reason;
        loseMenu.SetActive(true);
        loseScoreText.text = "Score: " + GameManager.instance.CalculateScore().ToString();
    }

    public void WinMenu() {
        winMenu.SetActive(true);
        winScoreText.text = "Score: " + GameManager.instance.CalculateScore().ToString();
    }

    public void RestartGame() {
        GameManager.instance.Restart();
    }
}
