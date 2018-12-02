using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {

    [Range(1, 10)] public int scrollSpeed;
    public float tileSizeX;

    private Vector3 startPosition;

    void Start() {
        startPosition = transform.position;
    }

    void Update() {

        transform.position = transform.position + Vector3.left * (Time.deltaTime * scrollSpeed / 10f) * ((GameManager.instance.theShip != null) ? GameManager.instance.theShip.data.speed / 10f : 1 );

        if (transform.position.x < startPosition.x - tileSizeX) {
            transform.position = new Vector3(startPosition.x, transform.position.y, startPosition.z);
        }
    }
}
