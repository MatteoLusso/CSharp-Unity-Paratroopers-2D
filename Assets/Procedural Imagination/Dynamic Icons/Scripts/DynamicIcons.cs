/*
   ___ ___  ___   ___ ___ ___  _   _ ___    _   _        
  | _ \ _ \/ _ \ / __| __|   \| | | | _ \  /_\ | |       
  |  _/   / (_) | (__| _|| |) | |_| |   / / _ \| |__     
  |_| |_|_\\___/ \___|___|___/ \___/|_|_\/_/ \_\____|    
                                                        
 ___ __  __   _   ___ ___ _  _   _ _____ ___ ___  _  _ 
|_ _|  \/  | /_\ / __|_ _| \| | /_\_   _|_ _/ _ \| \| |
 | || |\/| |/ _ \ (_ || || .` |/ _ \| |  | | (_) | .` |
|___|_|  |_/_/ \_\___|___|_|\_/_/ \_\_| |___\___/|_|\_|
                                                           

                                          DYNAMIC ICONS

                                   Author: Matteo Lusso
                                                 © 2021

*/

/*

DYNAMIC ICON is a script that generates useful markers on-screen to help the player during his game time.
I've tried to write a code that offers you a lot of customization by simply adding it as a component to 
the GameObject you want to indicate as a target.

The marker is easily customizable using only the editor. There a lot of fields and variables involved that
can be summarized this way: the marker can work as an indicator that "follows" the target when it's visible
on the screen and/or that indicates its position when it's outside the screen boundaries. The second
behavior is the compass one, where the icon changes its orientation to always point to the target.

Multiple instances of this script can be added to the same GameObject to create a more complex behavior
with different markers. Finally, you're also able to display a simple text near the icon and/or show the
distance from the target.

*/

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DLL_MathExt;
using DLL_Utilities;

public class DynamicIcons : MonoBehaviour
{    
    //====================================================//

    //┌ ICON
    //|
    //├─> PUBLIC:
    //|
        public Texture2D ICON_Texture2D;

        public Icon.Style ICON_Style;

        public Icon.Show ICON_Visibility;

        public Icon.OffScreenPosition ICON_OffScreenBehavior;

        public Icon.OnScreenPosition ICON_OnScreenBehavior;

        public string ICON_ScriptName = "Dynamic Icons";

        public bool ICON_Active = true;
        public bool ICON_Scale = true;
        public bool ICON_ChangeColor = false;
        public bool ICON_UpdateColorInstantaneously = false;
        public bool ICON_Fade = true;
        public bool ICON_UpdateAlphaInstantaneously = false;
        public bool ICON_Rotation = true;
        public bool ICON_UpdateOrientationInstantaneously = false;
        public bool ICON_UpdatePositionInstantaneously = false;
        public bool ICON_ShowCustomText = false;
        public bool ICON_ShowDistance = false;

        public AnimationCurve ICON_DistanceCurve = AnimationCurve.Linear(0.0f, 1.0f, 1000.0f, 0.0f);

        public float ICON_DistanceCurveMaxDistance = 1000.0f;
        public float ICON_FixedAngle = 0.0f;
        public float ICON_ColorSmoothFactor = 5.0f;
        public float ICON_FadeSmoothFactor = 5.0f;
        public float ICON_RotationSmoothFactor = 30.0f;
        public float ICON_MovementSmoothFactor = 25.0f;
        public float ICON_RotationCorrection = 0.0f;
        public float ICON_Eccentricity = 1.0f;
        public float ICON_MaxSize = 1.0f;
        public float ICON_AvoidCenter = 150.0f;
        public float TARGET_MaxVisibleDistance = 500.0f;
        public float TARGET_MinVisibleDistanceOnScreen = 0.0f;
        public float ICON_DistanceFromBorderInPixels = 25.0f;
        public float ICON_DistanceFromTargetInPixels = 50.0f;

        public Color ICON_Color;
        public Color ICON_AheadColor;
        public Color ICON_RearColor;

        public Vector2 ICON_Decentration = new Vector2(-5.0f, -5.0f);
    //|
    //└─> PRIVATE:
        private GameObject ICON_GO;

        private RawImage ICON_RawImage;

        private Vector3 ICON_NewPosition = Vector3.zero;

        private float ICON_NewRotationAroundZ = 0.0f;
        private float ICON_DistanceCorrection = 1.0f;

        private bool ICON_Initialize = true;
        private bool ICON_Restarted = true;
        private bool ICON_TooDistant = false;

    //====================================================//

    [Space]

    //====================================================//

    //┌ COMPASS
    //|
    //└─> PUBLIC:
    //|
        public Icon.CompassType COMPASS_Type;

        public Vector3 COMPASS_OrientationCorrections;

        public bool COMPASS_AutoCalculateFromWorldComp = false;

        public float COMPASS_HorLenght;
        public float COMPASS_VertLenght;
        public float COMPASS_PosX;
        public float COMPASS_PosY;
    //|
    //└─> PRIVATE:
        private Vector3 COMPASS_Dir = Vector3.zero;

    //====================================================//

    [Space]

    //====================================================//

    //┌ DISTANCE TEXT
    //|
    //├─> PUBLIC FIELDS:
        public TMP_FontAsset DT_Font;

        public Color DT_Color;

        public float DT_MaxSize = 0.5f;
        public float DT_MinDistanceFromIcon = 30.0f;
        public float DT_MaxDistanceFromIcon = 55.0f;
        public float DT_FixedAngle;
        public float DT_CustomUnitSize = 1.0f;

        public IconText.Unit DT_Unit;
        public IconText.DistancePosition DT_Behavior;
    //|   
    //└─> PRIVATE FIELDS:
        private GameObject DT_GO;

    //====================================================//

    [Space]

    //====================================================//

    //┌ CUSTOM TEXT
    //|
    //├─> PUBLIC FIELDS:
    //|
        public TMP_FontAsset CT_Font;
        public Color CT_Color;
        public float CT_MaxSize = 0.5f;
        public float CT_MinDistanceFromIcon = 30.0f;
        public float CT_MaxDistanceFromIcon = 55.0f;
        public float CT_FixedAngle;
        public string CT_String;
        public IconText.TextPosition CT_Behavior;
    //|
    //└─> PRIVATE FIELDS:
        private GameObject CT_GO;

    //====================================================//

    [Space]

    //====================================================//

    //┌ MISC
    //|
    //├ PUBLIC:
    //|
        public int SCRIPT_Order = -1;

        public bool DEBUG_ShowFullEditor = false;
    //|
    //└─> PRIVATE:
        private bool TEXT_ForceBehavior = false;
        private bool TARGET_IsOnScreen;

        private Vector3 TARGET_ScreenPos;
        private Vector3 TARGET_WorldDir;
        private Vector3 TARGET_ScreenDir;

        private float TARGET_DistanceFromCamera;
        private float TARGET_ScreenAngle;
        private float ANGLE_ScreenRatio;

    //====================================================//

    //┌ REFERENCES
    //|
    //├ PUBLIC:
    //| 
        public bool SCRIPT_IsSlave = false;
        public DynamicIcons DI_Master;
    //|
    //└─> PRIVATE FIELDS:
        private DIManager DIM_Ref;

        private Camera CAM_Main;

        private GameObject DI_UI;
        private GameObject ICON_Parent1;
        private GameObject ICON_Parent2;

        private DIWorldCompass WC_Ref;

    //====================================================//

    //****//
    
    void Awake()
    {
        if(SCRIPT_Order < -1)
        {
            SCRIPT_Order = -1;
        }
    }

    //****//

    void Start()
    {
        // Initializing the script.
        ICON_Initialize = DIInitializing(ICON_Initialize);
    }

    //****//

    private void CheckHorCompStatus()
    {
        if(CAM_Main.GetComponent<DIWorldCompass>() != null)
        {
            WC_Ref = CAM_Main.GetComponent<DIWorldCompass>();

            if(!WC_Ref.IsHorCompVisible())
            {
                COMPASS_AutoCalculateFromWorldComp = false;
            }
        }
        else
        {
            COMPASS_AutoCalculateFromWorldComp = false;
        }
    }
    
    private bool DIInitializing(bool ICON_Start)
    {
        // This function creates the hierarchy of objects. The script works using a UI canvas that is automatically generated.
        // As a child of the canvas, there is a first empty GameObject (ICON_Parent1) named as the target's GameObject to distinguish
        // its icons and texts from the ones of the other targets. A second empty GameObject child (ICON_Parent2) is created for
        // every Dynamic Icons script added to the target. Parent2 has the icon texture, the custom text, and the distance text as children.
        // They are updated by this instance of Dynamic Icons.

        if(ICON_Start)
        {
            // It's the reference to the DIManager. If the target hasn't it as component, it's added now. There is no need to add it in the editor. 
            DIM_Ref = this.gameObject.GetComponent<DIManager>() ?? this.gameObject.AddComponent<DIManager>().GetComponent<DIManager>();

            if(DIM_Ref.GetScriptInitIndex() != SCRIPT_Order && !DIM_Ref.GetIsDIRunning())
            {
                return true;
            }
            else if(DIM_Ref.GetIsDIRunning())
            {
                SCRIPT_Order = -2;
            }

            CAM_Main = Camera.main;

            //----//

            CheckHorCompStatus();

            //----//

            // Autogenerate a new UI canvas or find the existent one.
            if(!GameObject.Find("DI Canvas"))
            {
                DI_UI = new GameObject("DI Canvas");
                DI_UI.transform.position = Vector3.zero;
                DI_UI.AddComponent<Canvas>();

                DI_UI.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            }
            else
            {
                DI_UI = GameObject.Find("DI Canvas");
            }

            //----//

            // Autogenerate an empty GameObject child of the canvas and called as the target's GameObject where this script is attached or find the existent one.
            if(!GameObject.Find("[" + this.transform.name + "]"))
            {
                ICON_Parent1 = new GameObject("[" + this.transform.name + "]");
                ICON_Parent1.transform.position = Vector3.zero;
                ICON_Parent1.transform.parent = DI_UI.transform;
            }
            else
            {
                ICON_Parent1 = GameObject.Find("[" + this.transform.name + "]");
            }

            //----//

            // Autogenerate a second empty GameObject child of the previous one and called with the name choosen in the editor.
            if(ICON_ScriptName == null)
            {
                ICON_Parent2 = new GameObject("No name " + DIM_Ref.GetScriptCounter());
            }
            else
            { 
                ICON_Parent2 = new GameObject(ICON_ScriptName + " " + DIM_Ref.GetScriptCounter());
            }
            ICON_Parent2.transform.position = Vector3.zero;
            ICON_Parent2.transform.parent = ICON_Parent1.transform;
            ICON_Parent2.AddComponent<DIDestroyer>();
            ICON_Parent2.GetComponent<DIDestroyer>().SetDI(this);

            //----//

            // Auto generate the icon with its raw image.
            ICON_GO = new GameObject("Icon");
            ICON_GO.transform.parent = ICON_Parent2.transform;
            ICON_GO.AddComponent<RectTransform>();
            ICON_GO.AddComponent<MeshRenderer>();
            ICON_GO.AddComponent<CanvasRenderer>();
            ICON_GO.AddComponent<RawImage>();

            //----//

            ICON_RawImage = ICON_GO.GetComponent<RawImage>();
            ICON_RawImage.color = new Vector4(ICON_RawImage.color.r, ICON_RawImage.color.g, ICON_RawImage.color.b, 0.0f);

            //----//

            // Autogenerate the custom text GameObject.
            CT_GO = new GameObject("Custom Text");
            CT_GO.transform.parent = ICON_Parent2.transform;
            CT_GO.AddComponent<RectTransform>();
            CT_GO.AddComponent<TextMeshProUGUI>();

            //----//

            // Autogenerate the distance text GameObject.
            DT_GO = new GameObject("Distance Text");
            DT_GO.transform.parent = ICON_Parent2.transform;
            DT_GO.AddComponent<RectTransform>();
            DT_GO.AddComponent<TextMeshProUGUI>();
        }

        //----//

        return false;
    }

    //****//

    void LateUpdate()
    {
        if(!ICON_Initialize)
        {
            // It's the target distance from the camera.
            TARGET_DistanceFromCamera = DIM_Ref.GetCameraTargetWorldDist();
            
            // If this instance of DYNAMIC ICONS depends on another one, some data are obtained from the master script.
            if(SCRIPT_IsSlave)
            {
                GetVariablesFromMasterDI();
            }
            else
            {
                ICON_TooDistant = IsTargetTooDistant();
            }

            if(ICON_Active && !ICON_TooDistant)
            {
                ICON_Parent2.SetActive(true);

                ICON_RawImage.texture = ICON_Texture2D;
                if(ICON_ScriptName != null)
                {
                    ICON_Parent2.transform.name = ICON_ScriptName;
                }

                //----// COMMON VARIABLES:

                GetVariablesFromDIManager();

                //----//

                if(!SCRIPT_IsSlave)
                {
                    //The pixels between the icon and target (ICON_DistanceFromTargetInPixels) and the size of the icon itself (ICON_MaxSize)
                    // multiply this value to be reduced and increased according to the distance (in world coordinates) between the camera and the target. 
                    // The curve 'f(d)' drawn in the inspector goes from 0 to the max value chosen. That means ICON_DistanceCorrection = f(TARGET_DistanceFromCamera).
                    ICON_DistanceCorrection = ICON_DistanceCurve.Evaluate(TARGET_DistanceFromCamera);
                }

                //----// CALCULATION OF THE RAWIMAGE POSITION E ROTATION IN THE UI SPACE.

                if(ICON_Style == Icon.Style.Compass)
                {
                    CompassBehaviour();
                }
                else if(ICON_Style == Icon.Style.Indicator)
                {
                    IndicatorBehaviour();
                }

                //----// CALCULATION OF THE TEXTS POSITION IN THE UI SPACE.

                TextBehavior();

                //----// UPDATE OF THE RAWIMAGE AND TEXT SIZE, TRANSPARENCY AND COLOR

                UpdateIndicatorAndTextScale();
                UpdateIndicatorAndTextAlpha();
                UpdateIndicatorAndTextColor();

                //----//

                // This bool became true only during the frame when the bool ICON_Active goes from false to true.
                // It allows the position and the rotation of both marker and texts to be updated instantaneously 
                // when they reappear with the target in a completely different position.
                ICON_Restarted = false;

            }
            else
            {
                ICON_Restarted = true;
                ICON_Parent2.SetActive(false);
            }  
        }
    }

    //****//

    private void TextBehavior()
    {
        IconText.TextPosition CT_Aux;
        IconText.DistancePosition DT_Aux;

        // If the target is off-screen and the indicator is visible, the text behavior
        // will be forced to Around_Icon. 
        if(TEXT_ForceBehavior)
        {
            CT_Aux = IconText.TextPosition.Around_Icon_Inverse;
            DT_Aux = IconText.DistancePosition.Around_Icon_Inverse;
        }
        else
        {
            CT_Aux = CT_Behavior;
            DT_Aux = DT_Behavior;
        }

        //----//

        if(!ICON_ShowCustomText)
        {
            CT_GO.gameObject.SetActive(false);
        }
        else
        {
            CT_GO.gameObject.SetActive(true);

            CT_GO.transform.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            CT_GO.transform.GetComponent<TextMeshProUGUI>().font = CT_Font;
            CT_GO.transform.GetComponent<TextMeshProUGUI>().text = CT_String;

            // CUSTOM TEXT DISTANCE CORRECTION
            // The text has a different width and height. When it rotates around the icon, that distance needs to variate based on a custom range.
            // This allows keeping the text always at a fixed distance from the icon.

            // Calculation of the angle (in 360 deg) between the x axis and the direction between the icon and the text.
            float CT_Angle = 0.0f;
            if(CT_Aux == IconText.TextPosition.Fixed_Position)
            {
                CT_Angle = CT_FixedAngle;
            }
            else
            {
                CT_Angle = Vector3.SignedAngle(Vector3.right, CT_GO.transform.position - ICON_GO.transform.position/*COMPASS_Dir*/, Vector3.forward);
                CT_Angle = DLL_MathExt.Angles.SignedAngleTo360Angle(CT_Angle);
            }

            // Calculation of m value. It's the line slope [y = m*x].
            float CT_DirSlope = Mathf.Tan(CT_Angle * Mathf.Deg2Rad);

            // Assignment of the parameters of the equation of the ellipse [((x^2)/(a^2) + (y^2)/(b^2)) = 1] 
            float CT_a = CT_MaxDistanceFromIcon, CT_b = CT_MinDistanceFromIcon, CT_x, CT_y; 

            //Slope^2 [m^2].
            float CT_m2 = Mathf.Pow(CT_DirSlope, 2), CT_a2 = Mathf.Pow(CT_a, 2), CT_b2 = Mathf.Pow(CT_b, 2);

            //Intersection point coordinates (it's the point where the line intercepts the ellipse).
            CT_x = Mathf.Sqrt((CT_a2 * CT_b2) / ((CT_a2 * CT_m2) + CT_b2));
            CT_y = CT_DirSlope * CT_x;

            // Calculation of the distance between text and icon. The distance will range between the min and max values
            // (that's because the text is not a perfect square).
            float CT_DistanceFromIcon = 0.0f;
            if((CT_Angle >= 0.0f && CT_Angle < 90.0f) || CT_Angle >= 270.0f && CT_Angle <= 360.0f)   // First and fourth quadrant
            {
                CT_DistanceFromIcon = DLL_MathExt.Algebra.Module(CT_x, CT_y);
            }
            else if(CT_Angle >= 90.0f && CT_Angle < 270.0f )                                                                  // Second and third quadrant
            {
                CT_DistanceFromIcon = DLL_MathExt.Algebra.Module(-CT_x, -CT_y);
            }

            //----// CUSTOM TEXT NEW POSITION

            switch(CT_Aux)
            {
                // The text will stay in a fixed position near the marker.
                case IconText.TextPosition.Fixed_Position:      if(ICON_Scale)
                                                                {
                                                                    CT_GO.transform.position = ICON_GO.transform.position + (Quaternion.Euler(0.0f, 0.0f, CT_FixedAngle) * (Vector3.right * CT_DistanceFromIcon * ICON_DistanceCorrection));
                                                                }
                                                                else
                                                                {
                                                                    CT_GO.transform.position = ICON_GO.transform.position + (Quaternion.Euler(0.0f, 0.0f, CT_FixedAngle) * (Vector3.right * CT_DistanceFromIcon));
                                                                }

                                                                break;
                // The icon will always stay between the text and the screen center.
                case IconText.TextPosition.Around_Icon_Inverse: if(ICON_Scale)
                                                                {
                                                                    CT_GO.transform.position = ICON_GO.transform.position + (Quaternion.Euler(0.0f, 0.0f, TARGET_ScreenAngle) * (-Vector3.right * CT_DistanceFromIcon * ICON_DistanceCorrection));
                                                                }
                                                                else
                                                                {
                                                                    CT_GO.transform.position = ICON_GO.transform.position + (Quaternion.Euler(0.0f, 0.0f, TARGET_ScreenAngle) * (-Vector3.right * CT_DistanceFromIcon));
                                                                }
                                                                
                                                                break;
                //The text will always stay between the marker and the screen center at a fixed distance from the icon.                                   
                case IconText.TextPosition.Around_Icon:         if(ICON_Scale)
                                                                {
                                                                    CT_GO.transform.position = ICON_GO.transform.position + (Quaternion.Euler(0.0f, 0.0f, TARGET_ScreenAngle) * (Vector3.right * CT_DistanceFromIcon * ICON_DistanceCorrection));
                                                                }
                                                                else
                                                                {
                                                                    CT_GO.transform.position = ICON_GO.transform.position + (Quaternion.Euler(0.0f, 0.0f, TARGET_ScreenAngle) * (Vector3.right * CT_DistanceFromIcon));
                                                                }
                                                                
                                                                break;
            }
        }

        //----// UPDATING DISTANCE TEXT

        if(!ICON_ShowDistance)
        {
            DT_GO.gameObject.SetActive(false);
        }
        else
        {
            DT_GO.gameObject.SetActive(true);

            DT_GO.transform.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            DT_GO.transform.GetComponent<TextMeshProUGUI>().font = DT_Font;

            //CALCULATING DISTANCE
            // The distance can be showed in meters, kilometers, feet and miles.

            float text_ConvertedDistance = TARGET_DistanceFromCamera * DT_CustomUnitSize;

            switch(DT_Unit)
            {
                case IconText.Unit.Meters:      DT_GO.transform.GetComponent<TextMeshProUGUI>().text = "" +  (int)text_ConvertedDistance + " m";
                                                break;

                case IconText.Unit.Kilometers:  if((text_ConvertedDistance = (text_ConvertedDistance / 1000.0f)) >= 1.0f)
                                                {
                                                    DT_GO.transform.GetComponent<TextMeshProUGUI>().text = "" + String.Format("{0:.00}", text_ConvertedDistance) + " km";
                                                }
                                                else
                                                {
                                                    DT_GO.transform.GetComponent<TextMeshProUGUI>().text = "" + (int)(text_ConvertedDistance * 1000.0f) + " m";
                                                }

                                                break;

                case IconText.Unit.Feet:        DT_GO.transform.GetComponent<TextMeshProUGUI>().text = "" + (int)(text_ConvertedDistance * 3.281f) + " ft";
                                                break;

                case IconText.Unit.Miles:       if((text_ConvertedDistance = (text_ConvertedDistance / 1609.344f)) >= 1.0f)
                                                {
                                                    DT_GO.transform.GetComponent<TextMeshProUGUI>().text = "" + String.Format("{0:.00}", text_ConvertedDistance) + " mi";
                                                }
                                                else
                                                {
                                                    DT_GO.transform.GetComponent<TextMeshProUGUI>().text = "" + (int)(text_ConvertedDistance * 1609.344f * 3.281f) + " ft";
                                                }

                                                break;

                default:                        DT_GO.transform.GetComponent<TextMeshProUGUI>().text = "NaN";
                                                break;
            }

            //---// TEXT DISTANCE CORRECTION
            // The text has a different width and height. When it rotates around the icon, that distance needs to variate based on a custom range.
            // This allows keeping the text always at a fixed distance from the icon.

            // Calculation of the angle (in 360 deg) between the x-axis and the direction between the icon and the text.
            float DT_Angle = 0.0f;
            if(DT_Aux == IconText.DistancePosition.Fixed_Position)
            {
                DT_Angle = DT_FixedAngle;
            }
            else
            {
                DT_Angle = Vector3.SignedAngle(Vector3.right, DT_GO.transform.position - ICON_GO.transform.position /*COMPASS_Dir*/, Vector3.forward);
                DT_Angle = DLL_MathExt.Angles.SignedAngleTo360Angle(DT_Angle);
            }

            // Calculation of m value. It's the line slope [y = m*x].
            float DT_DirSlope = Mathf.Tan(DT_Angle * Mathf.Deg2Rad);

            // Assignment of the parameters of the equation of the ellipse [((x^2)/(a^2) + (y^2)/(b^2)) = 1] 
            float DT_a = DT_MaxDistanceFromIcon, DT_b = DT_MinDistanceFromIcon, DT_x, DT_y; 

            //Slope power of 2
            float DT_m2 = Mathf.Pow(DT_DirSlope, 2), DT_a2 = Mathf.Pow(DT_a, 2), DT_b2 = Mathf.Pow(DT_b, 2);

            //Intersection point coordinates (it's the point where the line interceps the ellipse).
            DT_x = Mathf.Sqrt((DT_a2 * DT_b2) / ((DT_a2 * DT_m2) + DT_b2));
            DT_y = DT_DirSlope * DT_x;

            // Calculation of the distance between text and icon. The distance will range between the min and max values
            //(that's because the text is not a perfet square).
            float DT_DistanceFromIcon = 0.0f;
            if((DT_Angle >= 0.0f && DT_Angle < 90.0f) || DT_Angle >= 270.0f && DT_Angle <= 360.0f)   // First and fourth quadrant.
            {
                DT_DistanceFromIcon = DLL_MathExt.Algebra.Module(DT_x, DT_y);
            }
            else if(DT_Angle >= 90.0f && DT_Angle < 270.0f )                                         // Second and third quadrant.
            {
                DT_DistanceFromIcon = DLL_MathExt.Algebra.Module(-DT_x, -DT_y);
            }
            
            //DISTANCE TEXT NEW POSITION

            switch(DT_Aux)
            {
                case IconText.DistancePosition.Fixed_Position:      if(ICON_Scale)
                                                                    {
                                                                        DT_GO.transform.position = ICON_GO.transform.position + (Quaternion.Euler(0.0f, 0.0f, DT_FixedAngle) * (Vector3.right * DT_DistanceFromIcon * ICON_DistanceCorrection));
                                                                    }
                                                                    else
                                                                    {
                                                                        DT_GO.transform.position = ICON_GO.transform.position + (Quaternion.Euler(0.0f, 0.0f, DT_FixedAngle) * (Vector3.right * DT_DistanceFromIcon));
                                                                    }

                                                                    break;

                case IconText.DistancePosition.Around_Icon_Inverse: if(ICON_Scale)
                                                                    {
                                                                        DT_GO.transform.position = ICON_GO.transform.position + (Quaternion.Euler(0.0f, 0.0f, TARGET_ScreenAngle) * (-Vector3.right * DT_DistanceFromIcon * ICON_DistanceCorrection));
                                                                    }
                                                                    else
                                                                    {
                                                                        DT_GO.transform.position = ICON_GO.transform.position + (Quaternion.Euler(0.0f, 0.0f, TARGET_ScreenAngle) * (-Vector3.right * DT_DistanceFromIcon));
                                                                    }
                                                                    
                                                                    break;
                                                                    
                case IconText.DistancePosition.Around_Icon:         if(ICON_Scale)
                                                                    {
                                                                        DT_GO.transform.position = ICON_GO.transform.position + (Quaternion.Euler(0.0f, 0.0f, TARGET_ScreenAngle) * (Vector3.right * DT_DistanceFromIcon * ICON_DistanceCorrection));
                                                                    }
                                                                    else
                                                                    {
                                                                        DT_GO.transform.position = ICON_GO.transform.position + (Quaternion.Euler(0.0f, 0.0f, TARGET_ScreenAngle) * (Vector3.right * DT_DistanceFromIcon));
                                                                    }
                                                                    
                                                                    break;
            }
        }
    }

    //****//

    private void IndicatorBehaviour()
    {
        // TARGET OFFSCREEN

        if((!TARGET_IsOnScreen && ICON_Visibility == Icon.Show.Always) || ICON_Visibility == Icon.Show.Off_Screen)
        {
            TEXT_ForceBehavior = true;

            // NEW POSTION IN THE UI
            // The coordinate x or y is determined using the equation of the line y = m*x + q. q = 0 because the axis origin is in the point O[Screen.width / 2, Screen.height / 2].

            // The slope m is the tangent of the angle between Vector3.right and the direction from the camera to the target (in world coordinates and previously calculated).
            float target_DirSlope = Mathf.Tan(TARGET_ScreenAngle * Mathf.Deg2Rad);

            float TARGET_ScreenAngle360 = DLL_MathExt.Angles.SignedAngleTo360Angle(TARGET_ScreenAngle);

            if(ICON_OffScreenBehavior == Icon.OffScreenPosition.Along_Screen_Border)
            {
                float coord_x = 0.0f, coord_y = 0.0f;

                switch(DLL_Utilities.Monitor.TargetPosition(TARGET_ScreenAngle360, ANGLE_ScreenRatio))
                {
                // ScreenSize(...) function determines the position of the object relative to the camera (right, bottom, left, or top) and it moves the RawImage along the respective side.

                //----//

                    case DLL_Utilities.Monitor.Side.Right:  coord_y = ((Screen.width / 2) - ICON_DistanceFromBorderInPixels) * target_DirSlope;

                                                            ICON_NewPosition = new Vector3(Screen.width - ICON_DistanceFromBorderInPixels, coord_y + (Screen.height / 2), 0.0f);

                                                            break;

                //----//

                    case DLL_Utilities.Monitor.Side.Up:     coord_x = ((Screen.height / 2) - ICON_DistanceFromBorderInPixels) / target_DirSlope;

                                                            ICON_NewPosition = new Vector3(coord_x + (Screen.width / 2), Screen.height - ICON_DistanceFromBorderInPixels, 0.0f);

                                                            break;

                //----//

                    case DLL_Utilities.Monitor.Side.Left:   coord_y = ((-Screen.width / 2) + ICON_DistanceFromBorderInPixels) * target_DirSlope;

                                                            ICON_NewPosition = new Vector3(ICON_DistanceFromBorderInPixels, coord_y + (Screen.height / 2), 0.0f);

                                                            break;

                //----//

                    case DLL_Utilities.Monitor.Side.Down:   coord_x = ((-Screen.height / 2) + ICON_DistanceFromBorderInPixels) / target_DirSlope;

                                                            ICON_NewPosition = new Vector3(coord_x + (Screen.width / 2), ICON_DistanceFromBorderInPixels, 0.0f);

                                                            break;

                //----//

                default:                                    break; 

                //----//

                }
            }

            //----// 

            else if(ICON_OffScreenBehavior == Icon.OffScreenPosition.Circular_Rotation)
            {
                // This behavior allows the markers to rotate around the screen center following a circular or elliptic pattern. That means we can calculate the point of intersection
                // between a line that goes from the screen center to the target position and the equation of the ellipse (a circle is a particular form of the ellipse equation).

                // Ellipse equation: [((x^2)/(a^2) + (y^2)/(b^2)) = 1]. If a = b = r, [((x^2)/(r^2) + (y^2)/(r^2)) = 1] and that's the circle equation.
                // a = (horizontal resolution / 2) - custom distance from border. b = (vertical resolution / 2) - custom distance from border. x and y are the coordinate of the intersection point.
                float a = (Screen.width / 2) - ICON_DistanceFromBorderInPixels, b = (Screen.height / 2) - ICON_DistanceFromBorderInPixels, x = 0, y = 0;
                
                // The float value ICON_Eccentricity goes from 0 to 1. (1 - ICON_Eccentricity) multiplies the difference (a-b).
                // If ICON_Eccentricity = 0, (a-b) = 0 and the icon will follow an elliptic pattern.
                // If ICON_Eccentricity = 1, a = b and the icon will follow a circular pattern around the screen center.
                if(Mathf.Max(Screen.width, Screen.height) == Screen.width)
                {
                    a -= (a - b) * (1 - ICON_Eccentricity);
                }
                else
                {
                    b -= (b - a) * (1 - ICON_Eccentricity);
                }

                Vector2 ICON_IntersectionPoint = DLL_MathExt.Geometry.Ellipse.IntersectionWithLine2D(new DLL_MathExt.Geometry.Ellipse(a, b), target_DirSlope);

                // Because we have considered a line and an ellipse equation centerd in (0,0), now we have to translate the insersection point P(x,y) of both half horizontal and vertical resolution.
                if((TARGET_ScreenAngle360 >= 0.0f && TARGET_ScreenAngle360 < 90.0f) || TARGET_ScreenAngle360 >= 270.0f && TARGET_ScreenAngle360 < 360.0f)   // First and fourth quadrant
                {
                    ICON_NewPosition = new Vector3(x + (Screen.width / 2), y + (Screen.height / 2), 0.0f);
                }
                else if(TARGET_ScreenAngle360 >= 90.0f && TARGET_ScreenAngle360 < 270.0f )                                                                  // Second and third quadrant
                {
                    ICON_NewPosition = new Vector3(-x + (Screen.width / 2), -y + (Screen.height / 2), 0.0f);
                }
            }

        //RAWIMAGE NEW ROTATION AROUND AXIS Z ([0, 0, 1])

            ICON_NewRotationAroundZ = TARGET_ScreenAngle;

        //----//

        }

        //----//

        else if((TARGET_IsOnScreen && ICON_Visibility == Icon.Show.Always) || (ICON_Visibility == Icon.Show.On_Screen && TARGET_ScreenPos.z >= 0.0f)) /*&& icon_ShowOnscreen) || (!icon_ShowOffscreen && TARGET_ScreenPos.z >= 0.0f))*/
        {
            TEXT_ForceBehavior = false;

            // TARGET ON-SCREEN

            switch(ICON_OnScreenBehavior)
            {
            
            //----//

                // Follow_Target behavior put the indicator between the camera and the target.

                                                                    // This if statement decides if the direction from the camera to the target (in screen coordinates) and the consecutive angle need to be calculated again.
                                                                    // It happens to avoid that the indicator overlaps the object when the last one it's perfectly aligned with the camera forward axis.
                                                                    // The script does that considering the screen center decentred when the object (in screen coordinates) is too much near the real center.
                                                                    // The result is that the indicator moves around the object when it's near at the real center of the screen.
                case Icon.OnScreenPosition.Follow_Target:           if(new Vector2(TARGET_ScreenDir.x, TARGET_ScreenDir.y).magnitude < ICON_AvoidCenter)
                                                                    {
                                                                        TARGET_ScreenDir = new Vector3(TARGET_ScreenPos.x + ICON_Decentration.x, TARGET_ScreenPos.y + ICON_Decentration.y, TARGET_ScreenPos.z) - new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
                                                                        TARGET_ScreenAngle = 180 + Vector3.SignedAngle(Vector3.right, new Vector3(Screen.width / 2, Screen.height / 2, 0.0f) - new Vector3(TARGET_ScreenPos.x + ICON_Decentration.x, TARGET_ScreenPos.y + ICON_Decentration.y, 0.0f), Vector3.forward);
                                                                    }

                                                                    if(ICON_Scale)
                                                                    {
                                                                        // The new position is the position of the target (in screen coordinate) less the normalized direction from screen center to the target (always in screen coordinates). 
                                                                        // The normalized direction multiplies a fixed constant distance in pixel and the exponential function a^(-b). b is the distance between the camera and the target in world coordinates.
                                                                        // a is a constant values sets in the editor. This allows increasing more the distance between the indicator and the object when the object is nearer to the camera and to reduce it less when is distant.
                                                                        ICON_NewPosition = new Vector3(TARGET_ScreenPos.x, TARGET_ScreenPos.y, 0.0f) - (new Vector3(TARGET_ScreenDir.x, TARGET_ScreenDir.y, 0.0f).normalized * ICON_DistanceFromTargetInPixels * ICON_DistanceCorrection);
                                                                    }
                                                                    else
                                                                    {
                                                                        ICON_NewPosition = new Vector3(TARGET_ScreenPos.x, TARGET_ScreenPos.y, 0.0f) - (new Vector3(TARGET_ScreenDir.x, TARGET_ScreenDir.y, 0.0f).normalized * ICON_DistanceFromTargetInPixels);
                                                                    }
                                                                    //----//

                                                                    ICON_NewRotationAroundZ = TARGET_ScreenAngle;

                                                                    //----//
                        
                                                                    break;

            //----//

                // Follow_Target_Inverse behavior is similar to Follow one except the indicator position is specular to the object than the previous one.
                case Icon.OnScreenPosition.Follow_Target_Inverse:   if(new Vector2(TARGET_ScreenDir.x, TARGET_ScreenDir.y).magnitude < ICON_AvoidCenter)
                                                                    {
                                                                        TARGET_ScreenDir = new Vector3(TARGET_ScreenPos.x + ICON_Decentration.x, TARGET_ScreenPos.y + ICON_Decentration.y, TARGET_ScreenPos.z) - new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
                                                                        TARGET_ScreenAngle = 180.0f + Vector3.SignedAngle(Vector3.right, new Vector3(Screen.width / 2, Screen.height / 2, 0.0f) - new Vector3(TARGET_ScreenPos.x + ICON_Decentration.x, TARGET_ScreenPos.y + ICON_Decentration.y, 0.0f), Vector3.forward);
                                                                    }

                                                                    if(ICON_Scale)
                                                                    {
                                                                        ICON_NewPosition = new Vector3(TARGET_ScreenPos.x, TARGET_ScreenPos.y, 0.0f) + (new Vector3(TARGET_ScreenDir.x, TARGET_ScreenDir.y, 0.0f).normalized * ICON_DistanceFromTargetInPixels * ICON_DistanceCorrection);
                                                                    }
                                                                    else
                                                                    {
                                                                        ICON_NewPosition = new Vector3(TARGET_ScreenPos.x, TARGET_ScreenPos.y, 0.0f) + (new Vector3(TARGET_ScreenDir.x, TARGET_ScreenDir.y, 0.0f).normalized * ICON_DistanceFromTargetInPixels);
                                                                    }

                                                                    //----//

                                                                    ICON_NewRotationAroundZ = 180.0f + TARGET_ScreenAngle;

                                                                    //----//

                                                                    break;
                                                                    //Same as Follow behavior but the direction between the camera and target sums instead of subtracting the position of the target. The angle is increased by 180 degrees.

            //----//

                // Relative_To_Target behavior lets the indicator in a fixed position relative to target.
                case Icon.OnScreenPosition.Relative_To_Target:      if(ICON_Scale)
                                                                    {
                                                                        ICON_NewPosition = new Vector3(TARGET_ScreenPos.x, TARGET_ScreenPos.y, 0.0f) - (Quaternion.Euler(0.0f, 0.0f, ICON_FixedAngle) * -Vector3.right * ICON_DistanceFromTargetInPixels * ICON_DistanceCorrection);
                                                                    }
                                                                    else
                                                                    {
                                                                        ICON_NewPosition = new Vector3(TARGET_ScreenPos.x, TARGET_ScreenPos.y, 0.0f) - (Quaternion.Euler(0.0f, 0.0f, ICON_FixedAngle) * -Vector3.right * ICON_DistanceFromTargetInPixels);
                                                                    }
                                                    
                                                                    if(ICON_UpdateOrientationInstantaneously || ICON_Restarted)
                                                                    {
                                                                        ICON_NewRotationAroundZ = Vector3.SignedAngle(Vector3.right, new Vector3(TARGET_ScreenPos.x, TARGET_ScreenPos.y, 0.0f) - ICON_NewPosition, Vector3.forward);
                                                                    }
                                                                    else
                                                                    {
                                                                        ICON_NewRotationAroundZ = Vector3.SignedAngle(Vector3.right, new Vector3(TARGET_ScreenPos.x, TARGET_ScreenPos.y, 0.0f) - ICON_RawImage.transform.position, Vector3.forward);
                                                                    }

                                                                    break;

            //----//

                //The indicator is in the same position (in screen coordinates) of the object.    
                case Icon.OnScreenPosition.Centered_On_Target:      if(new Vector2(TARGET_ScreenDir.x, TARGET_ScreenDir.y).magnitude < ICON_AvoidCenter)
                                                                    {
                                                                        TARGET_ScreenDir = new Vector3(TARGET_ScreenPos.x + ICON_Decentration.x, TARGET_ScreenPos.y + ICON_Decentration.y, TARGET_ScreenPos.z) - new Vector3(Screen.width / 2, Screen.height / 2, 0.0f);
                                                                        TARGET_ScreenAngle = 180.0f + Vector3.SignedAngle(Vector3.right, new Vector3(Screen.width / 2, Screen.height / 2, 0.0f) - new Vector3(TARGET_ScreenPos.x + ICON_Decentration.x, TARGET_ScreenPos.y + ICON_Decentration.y, 0.0f), Vector3.forward);
                                                                    }
                                                                    
                                                                    ICON_NewPosition = new Vector3(TARGET_ScreenPos.x, TARGET_ScreenPos.y, 0.0f);
                                                                    ICON_NewRotationAroundZ = TARGET_ScreenAngle;

                                                                    break;

            //----//

                default:                                    break;

            //----//

            }
        }

        UpdatePosition();
        UpdateOrientation();
    }

    //****//

    private void UpdateOrientation()
    {
        if(ICON_Rotation)
        {
            if(ICON_UpdateOrientationInstantaneously || ICON_Restarted)
            {
                ICON_RawImage.transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, ICON_NewRotationAroundZ + ICON_RotationCorrection);
            }
            else
            {
                ICON_RawImage.transform.GetComponent<RectTransform>().rotation = Quaternion.Slerp(ICON_RawImage.transform.GetComponent<RectTransform>().rotation, Quaternion.Euler(0.0f, 0.0f, ICON_NewRotationAroundZ + ICON_RotationCorrection), ICON_RotationSmoothFactor * Time.deltaTime);
            }
        }
        else
        {
            ICON_RawImage.transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, ICON_RotationCorrection);
        }
    }

    //****//

    private void UpdatePosition()
    {
        if(ICON_NewPosition.sqrMagnitude != Mathf.Infinity)
        {
            if(ICON_UpdatePositionInstantaneously || ICON_Restarted)
            {
                ICON_RawImage.transform.position = ICON_NewPosition;
            }
            else
            {
                ICON_RawImage.transform.position = Vector3.Lerp(ICON_RawImage.transform.position, ICON_NewPosition, ICON_MovementSmoothFactor * Time.deltaTime);
            }
        }

        //----//

        if(!SCRIPT_IsSlave)
        {
            ClampPosition();
        }

        //----//
    }

    //****//

    private void ClampPosition()
    {
        // CLAMP INDICATOR POSITION IN SCREEN

        if(ICON_Visibility == Icon.Show.Always || ICON_Visibility == Icon.Show.Off_Screen)
        {
            ICON_RawImage.transform.position = new Vector3(Mathf.Clamp(ICON_RawImage.transform.position.x, ICON_DistanceFromBorderInPixels, Screen.width - ICON_DistanceFromBorderInPixels), Mathf.Clamp(ICON_RawImage.transform.position.y, ICON_DistanceFromBorderInPixels, Screen.height - ICON_DistanceFromBorderInPixels), ICON_RawImage.transform.position.z);
        }
    }

    //****//

    private void CompassBehaviour()
    {
        // DIRECTION FROM COMPASS (ICON) TO TARGET (IN SCREEN SPACE COORDINATES)
        if(TARGET_ScreenPos.z < 0.0f && !CAM_Main.orthographic)
        {
            COMPASS_Dir = -Vector3.Reflect(TARGET_ScreenPos - ICON_RawImage.transform.position, -Vector3.forward);
        }
        else
        {
            COMPASS_Dir = TARGET_ScreenPos - ICON_RawImage.transform.position;
        }

        switch(COMPASS_Type)
        {
            case Icon.CompassType.Arrow:       // COMPASS POSITION

                                                ICON_RawImage.transform.position = new Vector3((Screen.width / 2) + (Screen.width / 2) * COMPASS_PosX, (Screen.height / 2) + (Screen.height / 2) * COMPASS_PosY, 0.0f);

                                                // COMPASS ORIENTATION
                                                
                                                if(ICON_UpdateOrientationInstantaneously || ICON_Restarted)
                                                {
                                                    ICON_RawImage.transform.rotation = Quaternion.LookRotation(COMPASS_Dir, -Vector3.forward) * Quaternion.Euler(COMPASS_OrientationCorrections.x, COMPASS_OrientationCorrections.y, COMPASS_OrientationCorrections.z);
                                                }
                                                else
                                                {
                                                    ICON_RawImage.transform.rotation = Quaternion.Lerp(ICON_RawImage.transform.rotation, Quaternion.LookRotation(COMPASS_Dir, -Vector3.forward), ICON_RotationSmoothFactor) * Quaternion.Euler(COMPASS_OrientationCorrections.x, COMPASS_OrientationCorrections.y, COMPASS_OrientationCorrections.z);
                                                }

                                                break;

            case Icon.CompassType.Horizontal:   float coord_X = 0.0f;

                                                CheckHorCompStatus();

                                                if(!COMPASS_AutoCalculateFromWorldComp)
                                                {
                                                    if(TARGET_ScreenPos.x > ((Screen.width / 2) + ((COMPASS_HorLenght * Screen.width) / 2)))
                                                    {
                                                        coord_X = (Screen.width / 2) + ((COMPASS_HorLenght * Screen.width) / 2);
                                                    }
                                                    else if(TARGET_ScreenPos.x < ((Screen.width / 2) - ((COMPASS_HorLenght * Screen.width) / 2)))
                                                    {
                                                        coord_X = (Screen.width / 2) - ((COMPASS_HorLenght * Screen.width) / 2);
                                                    }
                                                    else
                                                    {
                                                        coord_X = TARGET_ScreenPos.x;
                                                    }

                                                    if(TARGET_ScreenPos.z < 0.0f && !CAM_Main.orthographic)
                                                    {
                                                        coord_X = Screen.width - coord_X;
                                                    }

                                                    ICON_RawImage.transform.position = new Vector3(coord_X, (Screen.height / 2) + (Screen.height / 2) * COMPASS_PosY, 0.0f);
                                                }
                                                else
                                                {
                                                    if(!WC_Ref.IsInizializing())
                                                    {
                                                        Vector2 WC_Rect = WC_Ref.GetHorCompassRect();
                                                        Vector3 WC_Pos = WC_Ref.GetHorCompassPos();

                                                        if(TARGET_ScreenPos.x > WC_Pos.x + (WC_Rect.x / 2))
                                                        {
                                                            coord_X = WC_Pos.x + (WC_Rect.x / 2);
                                                        }
                                                        else if(TARGET_ScreenPos.x < WC_Pos.x - (WC_Rect.x / 2))
                                                        {
                                                            coord_X = WC_Pos.x - (WC_Rect.x / 2);
                                                        }
                                                        else
                                                        {
                                                            coord_X = TARGET_ScreenPos.x;
                                                        }

                                                        if(TARGET_ScreenPos.z < 0.0f && !CAM_Main.orthographic)
                                                        {
                                                            coord_X = Screen.width - coord_X;
                                                        }

                                                        ICON_RawImage.transform.position = new Vector3(coord_X, WC_Pos.y, 0.0f);
                                                    }
                                                }

                                                //----//

                                                if(ICON_Rotation)
                                                {
                                                    if(ICON_UpdateOrientationInstantaneously || ICON_Restarted)
                                                    {
                                                        ICON_RawImage.transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, Vector3.SignedAngle(Vector3.right, new Vector3(COMPASS_Dir.x, COMPASS_Dir.y, 0.0f), Vector3.forward));
                                                    }
                                                    else
                                                    {
                                                        ICON_RawImage.transform.GetComponent<RectTransform>().rotation = Quaternion.Lerp(ICON_RawImage.transform.GetComponent<RectTransform>().rotation, Quaternion.Euler(0.0f, 0.0f, Vector3.SignedAngle(Vector3.right, new Vector3(COMPASS_Dir.x, COMPASS_Dir.y, 0.0f), Vector3.forward)), ICON_RotationSmoothFactor * Time.deltaTime);
                                                    }
                                                }
                                                else
                                                {
                                                    ICON_RawImage.transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, ICON_RotationCorrection);
                                                }

                                                break;

            case Icon.CompassType.Vertical:     float coord_Y = 0.0f;

                                                if(TARGET_ScreenPos.y > ((Screen.height / 2) + ((COMPASS_VertLenght * Screen.height) / 2)))
                                                {
                                                    coord_Y = (Screen.height / 2) + ((COMPASS_VertLenght * Screen.height) / 2);
                                                }
                                                else if(TARGET_ScreenPos.y < ((Screen.height / 2) - ((COMPASS_VertLenght * Screen.height) / 2)))
                                                {
                                                    coord_Y = (Screen.height / 2) - ((COMPASS_VertLenght * Screen.height) / 2);
                                                }
                                                else
                                                {
                                                    coord_Y = TARGET_ScreenPos.y;
                                                }

                                                if(TARGET_ScreenPos.z < 0.0f && !CAM_Main.orthographic)
                                                {
                                                    coord_Y = Screen.height - coord_Y;
                                                }

                                                ICON_RawImage.transform.position = new Vector3((Screen.width / 2) + (Screen.width / 2) * COMPASS_PosX, coord_Y, 0.0f);

                                                //----//

                                                if(ICON_Rotation)
                                                {
                                                    if(ICON_UpdateOrientationInstantaneously || ICON_Restarted)
                                                    {
                                                        ICON_RawImage.transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, Vector3.SignedAngle(Vector3.right, new Vector3(COMPASS_Dir.x, COMPASS_Dir.y, 0.0f), Vector3.forward) + ICON_RotationCorrection);
                                                    }
                                                    else
                                                    {
                                                        ICON_RawImage.transform.GetComponent<RectTransform>().rotation = Quaternion.Lerp(ICON_RawImage.transform.GetComponent<RectTransform>().rotation, Quaternion.Euler(0.0f, 0.0f, Vector3.SignedAngle(Vector3.right, new Vector3(COMPASS_Dir.x, COMPASS_Dir.y, 0.0f), Vector3.forward) + ICON_RotationCorrection), ICON_RotationSmoothFactor * Time.deltaTime);
                                                    }
                                                }
                                                else
                                                {
                                                    ICON_RawImage.transform.GetComponent<RectTransform>().rotation = Quaternion.Euler(0.0f, 0.0f, ICON_RotationCorrection);
                                                }

                                                break;    
        
        }
    }

    //****//

    private void UpdateIndicatorAndTextScale()
    {
        Vector3 ICON_NewScale = new Vector3(ICON_MaxSize, ICON_MaxSize, 1.0f);
        Vector3 DT_NewScale = new Vector3(DT_MaxSize, DT_MaxSize, 1.0f);
        Vector3 CT_NewScale = new Vector3(CT_MaxSize, CT_MaxSize, 1.0f);

        if(ICON_Scale)
        {
            float ICON_NewScaleValue = ICON_MaxSize * ICON_DistanceCorrection;
            ICON_NewScale = new Vector3(ICON_NewScaleValue, ICON_NewScaleValue, 1.0f);

            float DT_NewScaleValue = DT_MaxSize * ICON_DistanceCorrection;
            DT_NewScale = new Vector3(DT_NewScaleValue, DT_NewScaleValue, 1.0f);

            float CT_NewScaleValue = CT_MaxSize * ICON_DistanceCorrection;
            CT_NewScale = new Vector3(CT_NewScaleValue, CT_NewScaleValue, 1.0f);
        }

        ICON_RawImage.transform.localScale = ICON_NewScale;
        DT_GO.transform.localScale = DT_NewScale;
        CT_GO.transform.localScale = CT_NewScale;
    }

    //****//

    private void UpdateIndicatorAndTextColor()
    {
        if(ICON_ChangeColor)
        {
            if(ICON_UpdateColorInstantaneously || ICON_Restarted)
            {
                if(TARGET_ScreenPos.z >= 0)
                {
                    ICON_RawImage.color = new Vector4(ICON_AheadColor.r, ICON_AheadColor.g, ICON_AheadColor.b, ICON_RawImage.color.a);
                }
                else
                {
                    ICON_RawImage.color = new Vector4(ICON_RearColor.r, ICON_RearColor.g, ICON_RearColor.b, ICON_RawImage.color.a);
                } 
            }
            else
            {
                if(TARGET_ScreenPos.z >= 0)
                {
                    ICON_RawImage.color = Vector4.Lerp(ICON_RawImage.color, new Vector4(ICON_AheadColor.r, ICON_AheadColor.g, ICON_AheadColor.b, ICON_RawImage.color.a), ICON_ColorSmoothFactor * Time.deltaTime);
                }
                else
                {
                    ICON_RawImage.color = Vector4.Lerp(ICON_RawImage.color,new Vector4(ICON_RearColor.r, ICON_RearColor.g, ICON_RearColor.b, ICON_RawImage.color.a), ICON_ColorSmoothFactor * Time.deltaTime);
                } 
            }
        }
        else
        {
             ICON_RawImage.color = new Vector4(ICON_Color.r, ICON_Color.g, ICON_Color.b, ICON_RawImage.color.a);
        }

        CT_GO.transform.GetComponent<TextMeshProUGUI>().color = new Vector4(CT_Color.r, CT_Color.g, CT_Color.b, CT_GO.transform.GetComponent<TextMeshProUGUI>().color.a);
        DT_GO.transform.GetComponent<TextMeshProUGUI>().color = new Vector4(DT_Color.r, DT_Color.g, DT_Color.b, DT_GO.transform.GetComponent<TextMeshProUGUI>().color.a);

    }

    //****//

    private void UpdateIndicatorAndTextAlpha()
    {
        bool ICON_ForceFade = false;

        if(ICON_Style != Icon.Style.Compass && ((TARGET_IsOnScreen && ICON_Visibility == Icon.Show.Off_Screen) || (!TARGET_IsOnScreen && ICON_Visibility == Icon.Show.On_Screen)))
        {
            ICON_ForceFade = true;

            if(ICON_FadeSmoothFactor <= 0.0f && !ICON_UpdateAlphaInstantaneously)
            {
                ICON_UpdateAlphaInstantaneously = true;
            }
        }

        if(ICON_UpdateAlphaInstantaneously)
        {
            if((ICON_Fade && ((TARGET_DistanceFromCamera > TARGET_MaxVisibleDistance) || (TARGET_DistanceFromCamera < TARGET_MinVisibleDistanceOnScreen && TARGET_IsOnScreen))) || ICON_ForceFade)
            {
                ICON_RawImage.color = new Vector4(ICON_RawImage.color.r, ICON_RawImage.color.g, ICON_RawImage.color.b, 0.0f);
                CT_GO.GetComponent<TextMeshProUGUI>().color = new Vector4(CT_GO.GetComponent<TextMeshProUGUI>().color.r, CT_GO.GetComponent<TextMeshProUGUI>().color.g, CT_GO.GetComponent<TextMeshProUGUI>().color.b, 0.0f);
                DT_GO.GetComponent<TextMeshProUGUI>().color = new Vector4(DT_GO.GetComponent<TextMeshProUGUI>().color.r, DT_GO.GetComponent<TextMeshProUGUI>().color.g, DT_GO.GetComponent<TextMeshProUGUI>().color.b, 0.0f);
            }
            else
            {
                ICON_RawImage.color = new Vector4(ICON_RawImage.color.r, ICON_RawImage.color.g, ICON_RawImage.color.b, 1.0f);
                CT_GO.GetComponent<TextMeshProUGUI>().color = new Vector4(CT_GO.GetComponent<TextMeshProUGUI>().color.r, CT_GO.GetComponent<TextMeshProUGUI>().color.g, CT_GO.GetComponent<TextMeshProUGUI>().color.b, 1.0f);
                DT_GO.GetComponent<TextMeshProUGUI>().color = new Vector4(DT_GO.GetComponent<TextMeshProUGUI>().color.r, DT_GO.GetComponent<TextMeshProUGUI>().color.g, DT_GO.GetComponent<TextMeshProUGUI>().color.b, 1.0f);
            } 
        }
        else
        {
            if((ICON_Fade && ((TARGET_DistanceFromCamera > TARGET_MaxVisibleDistance) || (TARGET_DistanceFromCamera < TARGET_MinVisibleDistanceOnScreen && TARGET_IsOnScreen))) || ICON_ForceFade)
            {
                ICON_RawImage.color = Vector4.Lerp(ICON_RawImage.color, new Vector4(ICON_RawImage.color.r, ICON_RawImage.color.g, ICON_RawImage.color.b, 0.0f), ICON_FadeSmoothFactor * Time.deltaTime);
                CT_GO.GetComponent<TextMeshProUGUI>().color = Vector4.Lerp(CT_GO.GetComponent<TextMeshProUGUI>().color, new Vector4(CT_GO.GetComponent<TextMeshProUGUI>().color.r, CT_GO.GetComponent<TextMeshProUGUI>().color.g, CT_GO.GetComponent<TextMeshProUGUI>().color.b, 0.0f), ICON_FadeSmoothFactor * Time.deltaTime);
                DT_GO.GetComponent<TextMeshProUGUI>().color = Vector4.Lerp(DT_GO.GetComponent<TextMeshProUGUI>().color, new Vector4(DT_GO.GetComponent<TextMeshProUGUI>().color.r, DT_GO.GetComponent<TextMeshProUGUI>().color.g, DT_GO.GetComponent<TextMeshProUGUI>().color.b, 0.0f), ICON_FadeSmoothFactor * Time.deltaTime);
            }
            else
            {
                ICON_RawImage.color = Vector4.Lerp(ICON_RawImage.color, new Vector4(ICON_RawImage.color.r, ICON_RawImage.color.g, ICON_RawImage.color.b, 1.0f), ICON_FadeSmoothFactor * Time.deltaTime);
                CT_GO.GetComponent<TextMeshProUGUI>().color = Vector4.Lerp(CT_GO.GetComponent<TextMeshProUGUI>().color, new Vector4(CT_GO.GetComponent<TextMeshProUGUI>().color.r, CT_GO.GetComponent<TextMeshProUGUI>().color.g, CT_GO.GetComponent<TextMeshProUGUI>().color.b, 1.0f), ICON_FadeSmoothFactor * Time.deltaTime);
                DT_GO.GetComponent<TextMeshProUGUI>().color = Vector4.Lerp(DT_GO.GetComponent<TextMeshProUGUI>().color, new Vector4(DT_GO.GetComponent<TextMeshProUGUI>().color.r, DT_GO.GetComponent<TextMeshProUGUI>().color.g, DT_GO.GetComponent<TextMeshProUGUI>().color.b, 1.0f), ICON_FadeSmoothFactor * Time.deltaTime);
            } 
        }
    }

    //****//

    private void GetVariablesFromMasterDI()
    {
        if(DI_Master != null)
        {
            if(DI_Master.gameObject == this.gameObject)
            {
                ICON_Active = DI_Master.ICON_Active;

                ICON_Style = DI_Master.ICON_Style;
                ICON_Eccentricity = DI_Master.ICON_Eccentricity;

                ICON_AvoidCenter = DI_Master.ICON_AvoidCenter;
                ICON_Decentration = DI_Master.ICON_Decentration;

                ICON_Scale = DI_Master.ICON_Scale;
                ICON_DistanceCurve = DI_Master.ICON_DistanceCurve;
                ICON_DistanceCurveMaxDistance = DI_Master.ICON_DistanceCurveMaxDistance;

                ICON_Fade = DI_Master.ICON_Fade;
                TARGET_MinVisibleDistanceOnScreen = DI_Master.TARGET_MinVisibleDistanceOnScreen;
                TARGET_MaxVisibleDistance = DI_Master.TARGET_MaxVisibleDistance;

                ICON_DistanceCorrection = DI_Master.ICON_DistanceCorrection;
                ICON_TooDistant = DI_Master.ICON_TooDistant;
            }
            else
            {
                SCRIPT_IsSlave = false;
            }
        }
        else
        {
            SCRIPT_IsSlave = false;
        }
    }

    //****//

    private void GetVariablesFromDIManager()
    {
        // The screen ratio could change during the esecution (maybe the player wants to change the resolution), so the script updates this float every frame.
        ANGLE_ScreenRatio = DIM_Ref.GetAngleRatio();

        // It's the direction between the camera and the target in world coordinates.
        TARGET_WorldDir = DIM_Ref.GetCameraTargetWorldDir();

        // It's the position of the target in screen coordinates.
        TARGET_ScreenPos = DIM_Ref.GetTargetScreenPos();

        // It's the direction between the camera and the target in screen coordinates.
        TARGET_ScreenDir = DIM_Ref.GetCameraTargetScreenDir();

        // It's the angle between Vector3.right and the direction from the camera to the target (in screen coordinates).
        TARGET_ScreenAngle = DIM_Ref.GetAngleScreen();

        // This bool variable is true when the target os in front of the camera (even if over the camere far plane).
        TARGET_IsOnScreen = DIM_Ref.GetIsTargetInFrontOfCamera();
    }

    //****//

    private bool IsTargetTooDistant()
    {
        // This function returns true if the target is too distant from the camera and it has already faded away or became too small to be visible.

        if(!ICON_Fade)
        {
            if(!ICON_Scale || (ICON_Scale && TARGET_DistanceFromCamera < ICON_DistanceCurveMaxDistance))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if(/*(*/TARGET_DistanceFromCamera > TARGET_MaxVisibleDistance /*|| (TARGET_DistanceFromCamera < TARGET_MinVisibleDistanceOnScreen && ICON_Visibility == Icon.Show.On_Screen))*/ && ICON_RawImage.color.a <= 0.01f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    //****//

}

//****//

public static class Icon
{

    public enum OnScreenPosition{Follow_Target, Follow_Target_Inverse, Relative_To_Target, Centered_On_Target};
    public enum OffScreenPosition{Along_Screen_Border, Circular_Rotation};
    public enum CompassType{Arrow, Horizontal, Vertical};
    public enum Style{Indicator, Compass};
    public enum Show{Always, On_Screen, Off_Screen};
}

//****//

public static class IconText
{
    public enum Unit{Meters, Kilometers, Feet, Miles};
    public enum TextPosition{Around_Icon, Around_Icon_Inverse, Fixed_Position};
    public enum DistancePosition{Around_Icon, Around_Icon_Inverse, Fixed_Position};
}