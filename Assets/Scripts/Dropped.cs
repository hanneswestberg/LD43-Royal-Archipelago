using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropped : MonoBehaviour {

    Vector3 vectorLeft;
    Vector3 vectorDown;
    float rot;

    private void Start() {
        vectorLeft = new Vector3(-Random.Range(0.5f, 1), 0f, 0f);
        vectorDown = new Vector3(0f, -Random.Range(0.5f, 1), 0f);
        rot = Random.Range(1f, 5f);
    }

    // Update is called once per frame
    void Update () {

        transform.position = transform.position + vectorLeft * Time.deltaTime * GameManager.instance.theShip.data.speed / 10f + vectorDown * Time.deltaTime;

        transform.Rotate(new Vector3(0f, 0f, rot));

        if (transform.position.y < -20f) {
            Destroy(gameObject);
        }


    }
}
