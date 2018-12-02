using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIError : MonoBehaviour {

    public Error thisError;

    [SerializeField] private Text text;
    [SerializeField] private Button fixButton;
    [SerializeField] private Text buttonCost;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        text.text = thisError.effect;

        fixButton.interactable = (GameManager.instance.theShip.data.materials >= thisError.cost);
        buttonCost.text = "<color=#F55018>" + thisError.cost.ToString() + "</color>";
    }

    public void ButtonFixError() {
        if (GameManager.instance.theShip.data.materials >= thisError.cost) {
            GameManager.instance.theShip.FixError(thisError);
            UIManager.instance.currentErrors.Remove(gameObject);
            Destroy(gameObject);

            UIManager.instance.uiSource.PlayOneShot(UIManager.instance.buttonSound);
            UIManager.instance.fixSource.PlayOneShot(UIManager.instance.fixSound);

        }
    }
}
