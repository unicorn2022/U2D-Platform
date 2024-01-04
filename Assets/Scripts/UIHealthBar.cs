using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [Tooltip("生命值文本UI组件")]
    public Text healthText;
    [Tooltip("血量条UI组件")]
    public Image healthBar;

    [Tooltip("角色当前血量")]
    public static int HealthCurrent;
    [Tooltip("角色最大血量")]
    public static int HealthMax;

    void Update()
    {
        healthBar.fillAmount = (float)HealthCurrent / HealthMax;
        healthText.text = HealthCurrent.ToString() + "/" + HealthMax.ToString();
    }
}
