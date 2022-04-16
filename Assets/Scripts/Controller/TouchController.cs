using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchController : MonoBehaviour {

    public float singleTap_MaxDistanceAllowed;

    public float doubleTap_MaxTimeBetweenSingleTouches;
    public float doubleTap_MaxDistanceBetweenTouches;

        [Space]

    [SerializeField]
    public TouchEvent singleTap_CallMethod;
    [SerializeField]
    public TouchEvent doubleTap_CallMethod;
    
    //public UnityEvent doubleTap_CallMethod;
    //public UnityEvent singleTap_CallMethod;

        [Space]

    //public float zoom_MinDelta;
    //public float rotation_MinDelta;

    public bool zoom_Allowed = true;
    public float zoom_OutMin;
    public float zoom_OutExtendedMin;
    public float zoom_OutMax;
    public float zoom_OutExtendedMax;
    public float zoom_Speed;

        [Space]

    public bool rotation_Allowed = true;
    public bool rotation_DoubleTapToResetRotation = true;
    public float rotation_MinDistanceBetweenFingers;

        [Space]

    public float smooth_AutoZoom;
    public float smooth_InertiaDrag;
    public float smooth_InterpolatedDirection;
    public float smooth_AutoMovement;

        [Space]

    public bool drag_Limitated; 
    public float inertia_MinIntensity;
    public float inertia_MaxIntensity;

        [Space]

    //-*-*-//

    private Vector3 drag_StartPosition = Vector3.zero;
    private Vector3 drag_InterpolatedDirection = Vector3.zero;

        //----//

    private bool drag_ResetStartPosition = true;
    private bool drag_StartInertia = true;

        //----//

    private Quaternion camera_StartingRotation;
    private Vector2 camera_MinBorderLimits = Vector2.zero;
    private Vector2 camera_MaxBorderLimits = Vector2.zero;

        //----//

    private float doubleTap_Timer;

    private bool doubleTap_Start = false;

    private Vector2 doubleTap_StartPosition = Vector2.zero;
    private Vector2 doubleTap_EndPosition = Vector2.zero;

    private bool touch_Active = true;

    private Touch singleTap_Info;
    private Touch doubleTap_Info;

    private int doubleTap_TouchesCounter;
    private Vector2 singleTap_LastPosition = Vector2.zero;

    private Coroutine auto_Movement = null;
    private Coroutine auto_Inertia = null;

    //private bool zoom_Allowed = true;
    //private bool rotation_Allowed = true;
	
    //-*-*-//

    void Start()
    {
        camera_StartingRotation = Camera.main.transform.rotation;

        //doubleTap_CallMethod.AddListener(ResetRotation);    
    }

    //-*-*-//

    void Update()
    {
        if(touch_Active)
        {
            switch(Input.touchCount)
            {
                case 0: drag_ResetStartPosition = false;
                        drag_StartInertia = true;
                        break;

                case 1: 
                        SingleTouchAnalysis();
                        break;
            
                case 2: DoubleTouchAnalysis();
                        break;
            }
        }

        //----//

        ZoomCorrection();
        CameraCorrection();

	}

    //-*-*-//

    private void SingleTouchAnalysis()
    {
        if(auto_Inertia != null)
        {
            StopCoroutine(auto_Inertia);
            auto_Inertia = null;
        }

        //----//

        Touch drag_SingleTouch = Input.GetTouch(0);

        switch(drag_SingleTouch.phase)
        {
            case TouchPhase.Began:  drag_StartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                                    //----//

                                    drag_InterpolatedDirection = Vector3.zero;

                                    //----//

                                    doubleTap_Start = true;
                                    doubleTap_EndPosition = drag_SingleTouch.position;
                                    doubleTap_TouchesCounter++;

                                    //----//

                                    singleTap_Info = drag_SingleTouch;

                                    break;

            case TouchPhase.Moved:  if(drag_ResetStartPosition)
                                    {
                                        drag_StartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        
                                        drag_ResetStartPosition = false;
                                    }

                                    //----//

                                    if(auto_Movement != null)
                                    {
                                        StopCoroutine(auto_Movement);
                                        auto_Movement = null;
                                    }
                                    Vector3 drag_Direction = drag_StartPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                                    Camera.main.transform.position = Camera.main.transform.position + drag_Direction;

                                    //----//

                                    drag_InterpolatedDirection = Vector3.Lerp(drag_InterpolatedDirection, drag_SingleTouch.deltaPosition, smooth_InterpolatedDirection);

                                    //----//

                                    doubleTap_Start = false;

                                    doubleTap_TouchesCounter = 0;

                                    break;

            case TouchPhase.Ended:  if(drag_StartInertia && auto_Inertia == null)
                                    {
                                        auto_Inertia = StartCoroutine(InertiaDrag(drag_InterpolatedDirection));
                                    }

                                    //----//

                                    if(doubleTap_TouchesCounter == 1)
                                    {
                                        singleTap_LastPosition = Input.GetTouch(0).position;

                                        doubleTap_Start = false;
                                        doubleTap_Timer = 0.0f;
                                        doubleTap_StartPosition = drag_SingleTouch.position;
                                        doubleTap_Info = drag_SingleTouch;

                                        StartCoroutine(Tap());
                                    }

                                    break;
        }
    }

    //-*-*-//

    private void DoubleTouchAnalysis()
    {
        StopCoroutine("InertiaDrag");

        drag_ResetStartPosition = true;

        Touch touch_Zero = Input.GetTouch(0);
        Touch touch_One = Input.GetTouch(1);

        Vector2 touch_ZeroPrevPos = touch_Zero.position - touch_Zero.deltaPosition;
        Vector2 touch_OnePrevPos = touch_One.position - touch_One.deltaPosition;

        float touch_PrevDirMagnitude = (touch_ZeroPrevPos - touch_OnePrevPos).magnitude;
        float touch_CurrDirMagnitude = (touch_Zero.position - touch_One.position).magnitude;

        float touch_DeltaDirMagnitude = touch_CurrDirMagnitude - touch_PrevDirMagnitude;

        Vector2 touch_CurrDir = touch_Zero.position - touch_One.position;
        Vector2 touch_PrevDir = touch_ZeroPrevPos - touch_OnePrevPos;

        float rotation_Angle = Vector2.Angle(touch_CurrDir, touch_PrevDir);

        //----//

        /*if(rotation_Angle < rotation_MinDelta && (touch_DeltaDirMagnitude < zoom_MinDelta && touch_DeltaDirMagnitude > -zoom_MinDelta))
        {

            if(rotation_Angle >= rotation_MinDelta && !zoom_Allowed)
            {
                rotation_Allowed = true;
                zoom_Allowed = false;
            }
            else if((touch_DeltaDirMagnitude >= zoom_MinDelta || touch_DeltaDirMagnitude <= -zoom_MinDelta) && !rotation_Allowed)
            {
                rotation_Allowed = false;
                zoom_Allowed = true;
            }

        }*/

        if(zoom_Allowed)
        {
            Zoom(touch_DeltaDirMagnitude * 0.01f);
        }

        //----//

        if(rotation_Allowed && (touch_Zero.position - touch_One.position).magnitude >= rotation_MinDistanceBetweenFingers)
        {
            Rotation(touch_CurrDir, touch_PrevDir);
        }


        //----//

        drag_StartInertia = false;
        doubleTap_Start = false;
    }

    //-*-*-//

    IEnumerator Tap()
    {

        while(doubleTap_Timer < doubleTap_MaxTimeBetweenSingleTouches)
        {
            if(doubleTap_CallMethod != null && doubleTap_Start == true && (doubleTap_StartPosition - doubleTap_EndPosition).magnitude <= doubleTap_MaxDistanceBetweenTouches)
            {

                if(rotation_DoubleTapToResetRotation)
                {
                    ResetRotation();
                }

                Debug.Log("Doppio tocco");
                doubleTap_CallMethod.Invoke(doubleTap_Info);
                doubleTap_Start = false;

            }   

            doubleTap_Timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        if(singleTap_CallMethod != null && doubleTap_TouchesCounter == 1 && (singleTap_Info.position - singleTap_LastPosition).magnitude <= singleTap_MaxDistanceAllowed)
        {
            Debug.Log("Singolo tocco");
            singleTap_CallMethod.Invoke(singleTap_Info);
        }

        doubleTap_TouchesCounter = 0;
    }

    //-*-*-//

    IEnumerator InertiaDrag(Vector3 direction_Last)
    {
        float inertia_Intensity = (inertia_MaxIntensity - inertia_MinIntensity) / (zoom_OutExtendedMax - zoom_OutExtendedMin) * Camera.main.orthographicSize;
        direction_Last = direction_Last * inertia_Intensity;

        float direction_DiffRot = Quaternion.Angle(Camera.main.transform.rotation, camera_StartingRotation);

        direction_Last = Quaternion.AngleAxis(Mathf.Sign(Vector3.SignedAngle(Vector3.up, Camera.main.transform.up, Vector3.forward)) * direction_DiffRot, Vector3.forward) * direction_Last;

        while(direction_Last.magnitude > 0.1f)
        {
            Camera.main.transform.position -= direction_Last;
            direction_Last = Vector3.Lerp(direction_Last, Vector3.zero, smooth_InertiaDrag * Time.deltaTime);
            
            yield return new WaitForEndOfFrame();
        }
    }

    //-*-*-//

    IEnumerator AutoMovement(Vector3 position_New)
    {
        while((Camera.main.transform.position - position_New).magnitude > 0.1f)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, position_New, smooth_AutoMovement * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        Camera.main.transform.position = position_New;
    }

    private void Rotation(Vector2 rot_ActDir, Vector2 rot_PrevDir)
    {
        float rot_Angle = Vector3.SignedAngle(rot_ActDir, rot_PrevDir, Vector3.forward);

        Camera.main.transform.rotation *= Quaternion.Euler(0.0f, 0.0f, rot_Angle);
    }

    //-*-*-//

    public void ResetRotation()
    {
        Camera.main.transform.rotation = camera_StartingRotation;
    }

    //-*-*-//

    private void Zoom(float drag_Delta)
    {

        drag_Delta *= zoom_Speed;

        Camera.main.orthographicSize -= drag_Delta;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, zoom_OutExtendedMin, zoom_OutExtendedMax);
    }

    //-*-*-//

    private void ZoomCorrection()
    {
        if(Input.touchCount < 2)
        {
            if(Camera.main.orthographicSize <= zoom_OutMin)
            {
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zoom_OutMin, smooth_AutoZoom * Time.deltaTime);
            }
            else if(Camera.main.orthographicSize >= zoom_OutMax)
            {
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zoom_OutMax, smooth_AutoZoom * Time.deltaTime);
            }
            else
            {
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, zoom_OutMin, zoom_OutMax);
            }
        }
    }

    //-*-*-//

    private void CameraCorrection()
    {
        if(drag_Limitated)
        {
            //Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, 0.0f, (logic_Game.world_X - 1) * logic_Game.tile_Plain.transform.localScale.x), Mathf.Clamp(Camera.main.transform.position.y, 0.0f, (logic_Game.world_Y - 1) * logic_Game.tile_Plain.transform.localScale.y), Camera.main.transform.position.z);
            Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, camera_MinBorderLimits.x, camera_MaxBorderLimits.x), Mathf.Clamp(Camera.main.transform.position.y, camera_MinBorderLimits.y, camera_MaxBorderLimits.y), Camera.main.transform.position.z);
        }
    }

    //-*-*-//

    public void AutoReachPosition(Vector3 movement_PositionToReach)
    {
        if(auto_Movement != null)
        {
            StopCoroutine(auto_Movement);
            auto_Movement = null;
        }

        auto_Movement = StartCoroutine(AutoMovement(movement_PositionToReach));

    }

    public void SetTouchState(bool touch_State)
    {
        touch_Active = touch_State;
    }

    public void SetCameraLimits(Vector2 camera_MinCoordinates, Vector2 camera_MaxCoordinates)
    {
        camera_MinBorderLimits = camera_MinCoordinates;
        camera_MaxBorderLimits = camera_MaxCoordinates;
    }

    [System.Serializable]
    public class TouchEvent : UnityEvent<Touch>{}

}
