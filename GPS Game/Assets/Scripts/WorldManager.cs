using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public GPSManager GPSManager;
    public Camera camera;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GPSManager.IsReady)
        {
            Vector3 camPos = camera.transform.position;
            camPos.x = GPSManager.position.x;
            camPos.z = GPSManager.position.y;
            camera.transform.position = camPos;

            Vector3 playerPos = player.transform.position;
            playerPos.x = GPSManager.position.x;
            playerPos.z = GPSManager.position.y;
            player.transform.position = playerPos;
        }
    }
}
