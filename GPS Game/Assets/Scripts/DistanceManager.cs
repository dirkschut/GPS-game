using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the distance panel in the GUI
/// </summary>
public class DistanceManager : MonoBehaviour
{
    private TMPro.TextMeshProUGUI distanceText;

    public TMPro.TextMeshProUGUI DistanceText
    {
        get {
            if(distanceText == null)
            {
                distanceText = GetComponent<TMPro.TextMeshProUGUI>();
            }
            return distanceText; 
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Updates the text in the panel
    /// </summary>
    void Update()
    {
        int distance = WorldManager.GetDistanceToday();
        string distanceString;

        if(distance >= 100000)
        {
            int distanceKM = distance / 1000;
            distanceString = distanceKM.ToString() + "km";
        }
        else if(distance >= 1000)
        {
            float distanceKM = distance / 1000f;
            distanceString = distanceKM.ToString("F") + "km";
        }
        else
        {
            distanceString = distance + "m";
        }

        DistanceText.text = "Distance: " + distanceString;
    }
}
