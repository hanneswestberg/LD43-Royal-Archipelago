using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandScript : MonoBehaviour {


   int scrollSpeed = 10;



    void Update() {
        transform.position = transform.position + Vector3.left * (Time.deltaTime * scrollSpeed / 10f) * ((GameManager.instance.theShip != null) ? GameManager.instance.theShip.data.speed / 10f : 0);
    }


    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.GetComponent<FlyingShip>() != null) {
            GameManager.instance.WinGame();
        }
    }

}
