using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float battery = SystemInfo.batteryLevel;
        battery = Mathf.Clamp(battery, 0f, 100f);
        GetComponent<TMPro.TextMeshProUGUI>().text = "Battery: \n" + battery.ToString("P0");
    }
}
