using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EastEggID : MonoBehaviour {
    [Tooltip("彩蛋的ID")]
    public int eggID = 0;

    void OnDestroy() {
        EastEgg.Password += eggID.ToString();
    }
}
