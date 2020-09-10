using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePanelManager : MonoBehaviour
{
    private ZoneID zoneID;
    public bool isOpen = false;

    public TMPro.TextMeshProUGUI nameLabel;
    public TMPro.TextMeshProUGUI coordinatesLabel;
    public TMPro.TextMeshProUGUI pointsLabel;
    public TMPro.TextMeshProUGUI labelsLabel;
    public TMPro.TextMeshProUGUI nextVisitLabel;
    
    public void OpenZoneID(ZoneID zoneID)
    {
        this.zoneID = zoneID;
        UpdateText();
        gameObject.SetActive(true);
        this.isOpen = true;
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
        this.isOpen = false;
    }

    private void UpdateText()
    {
        nameLabel.text = "Name: " + zoneID.ToString();
        coordinatesLabel.text = "Coords: lat: " + WorldManager.tiley2lat(zoneID.y, WorldManager.zoomLevel) + ", long: " + WorldManager.tilex2long(zoneID.x, WorldManager.zoomLevel);
        pointsLabel.text = "Points: " + zoneID.GetZoneData().points;
        nextVisitLabel.text = "Next Visit: " + zoneID.GetZoneData().nextVisit;
    }
}
