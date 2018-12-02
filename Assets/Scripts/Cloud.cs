using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {

    public int scrollSpeed = 1;

    // Update is called once per frame
    void Update () {

        transform.position = transform.position + Vector3.left * (Time.deltaTime * scrollSpeed / 10f) * ((GameManager.instance.theShip != null) ? GameManager.instance.theShip.data.speed / 10f : 1);

        if (transform.position.x < -20f) {
            Destroy(gameObject);
        }
    }
}
