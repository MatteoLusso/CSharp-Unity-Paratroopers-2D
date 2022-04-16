using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameCore gameCore;
    void Start()
    {
        this.transform.position = new Vector3(gameCore.world_X * gameCore.tile_Plain.transform.localScale.x / 2, gameCore.world_Y * gameCore.tile_Plain.transform.localScale.z / 2, -10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
