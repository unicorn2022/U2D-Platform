using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowStar : MonoBehaviour {
    [Tooltip("礼物的预制体")]
    public GameObject[] gifts;

    public void GenerateGift() {
        int index = Random.Range(0, gifts.Length);
        Instantiate(gifts[index], transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
