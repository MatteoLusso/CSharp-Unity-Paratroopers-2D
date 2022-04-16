/*
   ___ ___  ___   ___ ___ ___  _   _ ___    _   _        
  | _ \ _ \/ _ \ / __| __|   \| | | | _ \  /_\ | |       
  |  _/   / (_) | (__| _|| |) | |_| |   / / _ \| |__     
  |_| |_|_\\___/ \___|___|___/ \___/|_|_\/_/ \_\____|    
                                                        
 ___ __  __   _   ___ ___ _  _   _ _____ ___ ___  _  _ 
|_ _|  \/  | /_\ / __|_ _| \| | /_\_   _|_ _/ _ \| \| |
 | || |\/| |/ _ \ (_ || || .` |/ _ \| |  | | (_) | .` |
|___|_|  |_/_/ \_\___|___|_|\_/_/ \_\_| |___\___/|_|\_|
                                                           

                                             DI MANAGER

                                   Author: Matteo Lusso
                                                 © 2020

*/   

/*

DI MANAGER works together with the DYNAMIC ICONS scripts.
You're able to add multiple instances of DYNAMIC ICONS to the single
object if you want to show multiple indicators with different behaviors.

Different variables update every frame (like the distance between
the main camera and the target). Instead of calculating the same variable
as many times as the numnber of DYNAMIC ICONS scripts added as a component,
DI MANAGER updates them only one time and the DYNAMIC ICONS
scripts simply get their common variables from this script.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DLL_Utilities;

public class DIManager : MonoBehaviour
{
    private float angleRatio;
    // It's the angle in degrees between the Vector3.right [1, 0, 0] and the screen diagonal (in screen space coordinates). It's a constant value but it will update every frame. Because the player could be allowed to change the screen resolution.
    private float angleScreen;
    private float cameraTargetWorldDist;

    private bool isTargetInFrontOfCamera;

    private Vector3 targetScreenPos;
    private Vector3 cameraTargetWorldDir;
    private Vector3 cameraTargetScreenDir;

    private int scriptCounter = 0;

    private List<int> scriptsOrders = new List<int>();
    private int scriptOrderIndex = -2;
    private bool running = false;

    //----//

    private void Awake()
    {
        foreach(DynamicIcons DI in this.gameObject.GetComponents<DynamicIcons>())
        {
            scriptsOrders.Add(DI.SCRIPT_Order);
        }

        StartCoroutine("DIInizialization");
    }

    IEnumerator DIInizialization()
    {
        for(int i = scriptOrderIndex; i < Mathf.Max(scriptsOrders.ToArray()); i++)
        {
            scriptOrderIndex++;

            yield return new WaitForEndOfFrame();
        }

        running = true;
    }

    public bool GetIsDIRunning()
    {
        return running;
    }

    public int GetScriptInitIndex()
    {
        return scriptOrderIndex;
    }

    private void Update()
    {
        angleRatio = UpdateAngleRatio();

        isTargetInFrontOfCamera = UpdateIsTargetInFrontOfCamera();

        targetScreenPos = UpdateTargetScreenPos();
        cameraTargetWorldDir = UpdateCameraTargetWorldDir();
        cameraTargetWorldDist = UpdateCameraTargetWorldDist();

        cameraTargetScreenDir = UpdateCameraTargetScreenDir();
        angleScreen = UpdateAngleScreen();
    }

    //----//

    private float UpdateAngleRatio()
    {
        return DLL_Utilities.Monitor.AspectRatioAngle();
    }

    private bool UpdateIsTargetInFrontOfCamera()
    {
        return DLL_Utilities.Monitor.IsTargetOnScreen(Camera.main ,this.gameObject);
    }

    private Vector3 UpdateTargetScreenPos()
    {
        return Camera.main.WorldToScreenPoint(this.transform.position);
    }

    private Vector3 UpdateCameraTargetWorldDir()
    {
        return this.transform.position - Camera.main.transform.position;
    }

    private float UpdateCameraTargetWorldDist()
    {
        return cameraTargetWorldDir.magnitude;
    }

    private float UpdateAngleScreen()
    {
        if(targetScreenPos.z < 0.0f && !Camera.main.orthographic)
        {
            return Vector2.SignedAngle(Vector2.right, -new Vector3(cameraTargetScreenDir.x, cameraTargetScreenDir.y, 0.0f));
        }
        else
        {
            return Vector2.SignedAngle(Vector2.right, new Vector3(cameraTargetScreenDir.x, cameraTargetScreenDir.y, 0.0f));
        }
    }

    private Vector3 UpdateCameraTargetScreenDir()
    {
        return targetScreenPos - new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
    }

    //----//

    public int GetScriptCounter()
    {
        scriptCounter++;
        return scriptCounter;
    }

    public float GetAngleRatio()
    {
        return angleRatio;
    }

    public bool GetIsTargetInFrontOfCamera()
    {
        return isTargetInFrontOfCamera;
    }

    public Vector3 GetTargetScreenPos()
    {
        return targetScreenPos;
    }

    public Vector3 GetCameraTargetWorldDir()
    {
        return cameraTargetWorldDir;
    }

    public float GetCameraTargetWorldDist()
    {
        return cameraTargetWorldDist;
    }

    public float GetAngleScreen()
    {
        return angleScreen;
    }

    public Vector3 GetCameraTargetScreenDir()
    {
        return cameraTargetScreenDir;
    }
}