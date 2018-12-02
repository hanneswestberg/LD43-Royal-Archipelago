using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour {


    [SerializeField] private List<GameObject> cloudsGO;

    private float timer = 0;


	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;

        if (timer < 0) {

            Vector3 pos = new Vector3(5f, transform.position.y + Random.Range(0, 5f), 2f);
            GameObject cloud = Instantiate(cloudsGO[Random.Range(0, cloudsGO.Count)], pos, Quaternion.identity);

            cloud.transform.localScale = Vector3.one * Random.Range(0.3f, 1.2f);
            cloud.GetComponent<Cloud>().scrollSpeed = Random.Range(1, 10);
            cloud.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, cloud.GetComponent<Cloud>().scrollSpeed / 10f);

            timer = Random.Range(8f, 12f);
        }

    }
}
