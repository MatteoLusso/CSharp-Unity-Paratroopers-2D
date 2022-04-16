/*
   ___ ___  ___   ___ ___ ___  _   _ ___    _   _        
  | _ \ _ \/ _ \ / __| __|   \| | | | _ \  /_\ | |       
  |  _/   / (_) | (__| _|| |) | |_| |   / / _ \| |__     
  |_| |_|_\\___/ \___|___|___/ \___/|_|_\/_/ \_\____|    
                                                        
 ___ __  __   _   ___ ___ _  _   _ _____ ___ ___  _  _ 
|_ _|  \/  | /_\ / __|_ _| \| | /_\_   _|_ _/ _ \| \| |
 | || |\/| |/ _ \ (_ || || .` |/ _ \| |  | | (_) | .` |
|___|_|  |_/_/ \_\___|___|_|\_/_/ \_\_| |___\___/|_|\_|
                                                           

                                       DI WORLD COMPASS

                                   Author: Matteo Lusso
                                                 © 2021

*/

/*

DIWorldCompass is a C# script that generates and handles the world compass.
If you want to show a classic circular compass that points to the north or
a horizontal bar with the cardinal points you have to add this script to
your main camera.

The horizontal compass works together with the DynamicIcons script to show
an indicator directly on the horizontal compass.

*/

using UnityEngine;
using UnityEngine.UI;
using DLL_MathExt;

public class DIWorldCompass : MonoBehaviour
{
    //====================================================//

    //┌ NORTH ORIENTATION
    //|
    //├─> PUBLIC:
    //|
            public WorldCompass.Direction.Forward OR_Camera;
            public WorldCompass.Direction.Forward OR_Player;
            public WorldCompass.Direction.North OR_North;
            public WorldCompass.Direction.Zenit OR_Zenit;
            public bool OR_InverseZenit;

    //====================================================//

    //┌ CIRCULAR COMPASS
    //|
    //├─> PUBLIC:
    //|
            public bool CC_ShowCompass;
            public bool CC_IstantaneousMovement;

            [Space]

            public WorldCompass.Circular.Behavior CC_Behavior;

            [Space]

            public GameObject CC_PlayerGO;

            [Space]

            public float CC_Size;
            public float CC_HorRelativePos;
            public float CC_VertRelativePos;
            public float CC_Arrow1RotCorrection;
            public float CC_Arrow2RotCorrection;
            public float CC_BackRotCorrection;
            public float CC_Speed;

            [Space]

            public Vector3 CC_RotationCorrection;

            [Space]

            public Color CC_BackColor;
            public Color CC_Arrow1Color;
            public Color CC_Arrow2Color;

            [Space]
        
            public Texture2D CC_Arrow1Texture;
            public Texture2D CC_Arrow2Texture;
            public Texture2D CC_BackTexture;

            [Space][Space]
    //|   
    //└─> PRIVATE:
            private bool CC_BackVis, CC_Arrow1Vis, CC_Arrow2Vis;
            private RawImage CC_Arrow1, CC_Arrow2,  CC_Back;
            private GameObject CC_GO, CC_Arrow1GO, CC_Arrow2GO, CC_BackGO;

    //====================================================//

    [Space][Space]

    //====================================================//

    //┌ HORIZONTAL COMPASS
    //|
    //├─> PUBLIC:
    //|
            public bool HC_ShowHorCompass = false;
            public bool HC_IstantaneousMovement = true;
            public bool HC_AdaptToScreenWidth = false;

            [Space]

            public float HC_Speed = 2.0f;
            public float HC_VerticalPosition = 0.0f;
            public float HC_RotationCorrection = 0.0f;
            public float HC_LengthFixed = 500.0f;
            public float HC_HeightFixed = 80.0f;
            public float HC_HorRelativeLenght = 1.0f;
            public float HC_LengthPercent = 0.5f;
            public float HC_BackRectRelLenght = 1.0f;
            public float HC_BackRectRelHeight = 1.0f;
            public float HC_OverRectRelLenght = 1.0f;
            public float HC_OverRectRelHeight = 1.0f;

            [Space]

            public Color HC_CompColor;
            public Color HC_BackColor;
            public Color HC_OverColor;

            [Space]

            public Texture2D HC_CompTexture;
            public Texture2D HC_BackTexture;
            public Texture2D HC_OverTexture;

            [Space][Space]
    //|   
    //└─> PRIVATE:
            private bool HC_CompVis, HC_BackVis, HC_OverVis;
            private RawImage HC_Comp, HC_Back, HC_Over;
            private GameObject DI_UI, HC_GO, HC_CompGO, HC_BackGO, HC_OverGO;

    //====================================================//

    //┌ REFERENCES
    //|
    //└─> PUBLIC:
            public bool DEBUG_ShowFullEditor;
    //|   
    //└─> PRIVATE:
            private bool WC_Inizialize = true;

    //====================================================//

    void Awake()
    {
       WC_Inizialize = WCInitializing(WC_Inizialize); 
    }

    private bool WCInitializing(bool WC_Start)
    {
        if(WC_Start)
        {
            // As for DynamicIcons script, if the DI Canvas doesn't exist, it's generated.
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

        // The hierarchy of the horizontal compass is generated.
        // A GameObject contains the references to the background, compass and overlay.
        // Background and overlay texture have the same position of the compass one and
        // their size is relative of the compass one.
        // To obtain the best result all the three texture should have the same resolution.

        HC_GO = new GameObject("World Horizontal Compass");
        HC_GO.transform.parent = DI_UI.transform;
        HC_GO.AddComponent<DIDestroyer>();
        HC_GO.GetComponent<DIDestroyer>().SetWC(this.GetComponent<DIWorldCompass>());

        //----//

        HC_BackGO = new GameObject("Background Texture");
        HC_BackGO.AddComponent<RawImage>();
        HC_BackGO.transform.SetParent(HC_GO.transform, false);

        HC_Back = HC_BackGO.GetComponent<RawImage>();

        //----//

        HC_CompGO = new GameObject("Compass Texture");
        HC_CompGO.AddComponent<RawImage>();
        HC_CompGO.transform.SetParent(HC_GO.transform, false);

        HC_Comp = HC_CompGO.GetComponent<RawImage>();

        //----//

        HC_OverGO = new GameObject("Overlay Texture");
        HC_OverGO.AddComponent<RawImage>();
        HC_OverGO.transform.SetParent(HC_GO.transform, false);

        HC_Over = HC_OverGO.GetComponent<RawImage>();

        //----//

        // The hierarchy of the circular compass is generated.
        // A GameObject contains the references to the background, the first arrow and the second arrow texture.
        // The first arrow shows the player GameObject forward axis direction, while the second one the 
        // main camera forward axis direction.

        CC_GO = new GameObject("World Circular Compass");
        CC_GO.transform.parent = DI_UI.transform;
        CC_GO.AddComponent<DIDestroyer>();
        CC_GO.GetComponent<DIDestroyer>().SetWC(this.GetComponent<DIWorldCompass>());

        //----//

        CC_BackGO = new GameObject("Background Texture");
        CC_BackGO.AddComponent<RawImage>();
        CC_BackGO.transform.SetParent(CC_GO.transform, false);

        CC_Back = CC_BackGO.GetComponent<RawImage>();

        //----//

        CC_Arrow1GO = new GameObject("Player Dir Arrow Texture");
        CC_Arrow1GO.AddComponent<RawImage>();
        CC_Arrow1GO.transform.SetParent(CC_GO.transform, false);

        CC_Arrow1 = CC_Arrow1GO.GetComponent<RawImage>();

        //----//

        CC_Arrow2GO = new GameObject("Camera Dir Arrow Texture");
        CC_Arrow2GO.AddComponent<RawImage>();
        CC_Arrow2GO.transform.SetParent(CC_GO.transform, false);

        CC_Arrow2 = CC_Arrow2GO.GetComponent<RawImage>();
        }

        return false;
    }

    private float CalculateCompassRotation(Transform OBJ_Trans, WorldCompass.Direction.Forward OBJ_Forward)
    {

        if((int)OR_North == (int)OR_Zenit)
        {
            throw new UnityException("North and zenit directions can't be equals.");
        }

        float ROT_Angle = 0.0f;
        Vector3 DIR_Forward = Vector3.zero;
        Vector3 DIR_North = Vector3.zero;
        Vector3 DIR_Zenit = Vector3.zero;

        switch(OBJ_Forward)
        {
            case WorldCompass.Direction.Forward.X:  DIR_Forward = OBJ_Trans.right;
                                                    break;

            case WorldCompass.Direction.Forward.Y:  DIR_Forward = OBJ_Trans.up;
                                                    break;

            case WorldCompass.Direction.Forward.Z:  DIR_Forward = OBJ_Trans.forward;
                                                    break;
        }

        switch(OR_North)
        {
            case WorldCompass.Direction.North.X:    DIR_North = Vector3.right;
                                                    break;

            case WorldCompass.Direction.North.Y:    DIR_North = Vector3.up;
                                                    break;

            case WorldCompass.Direction.North.Z:    DIR_North = Vector3.forward;
                                                    break;
        }

        switch(OR_Zenit)
        {
            case WorldCompass.Direction.Zenit.X:    DIR_Zenit = Vector3.right;
                                                    break;

            case WorldCompass.Direction.Zenit.Y:    DIR_Zenit = Vector3.up;
                                                    break;

            case WorldCompass.Direction.Zenit.Z:    DIR_Zenit = Vector3.forward;
                                                    break;
        }

        if(OR_InverseZenit)
        {
            DIR_Zenit = -DIR_Zenit;
        }

        ROT_Angle = Vector3.SignedAngle(DIR_North, DIR_Forward, DIR_Zenit);

        return ROT_Angle;
    }

    void LateUpdate()
    {
        // Thos flags are used to check if the input textures exist.
        if(CC_BackTexture != null)
        {
            CC_BackVis = true;
        }
        else
        {
            CC_BackVis = false;
        }

        if(CC_Arrow1Texture != null && CC_PlayerGO != null)
        {
            CC_Arrow1Vis = true;
        }
        else
        {
            CC_Arrow1Vis = false;
        }

        if(CC_Arrow2Texture != null)
        {
            CC_Arrow2Vis = true;
        }
        else
        {
            CC_Arrow2Vis = false;
        }

        //*****//

        // Circular compass behavior.
        if(CC_ShowCompass)
        {
            //----//

            CC_GO.SetActive(true);

            // The position depends on the actual screen resolution and the compass is anchored to the screen center.
            CC_GO.transform.position = new Vector3((Screen.width / 2) + (Screen.width / 2) * CC_HorRelativePos, (Screen.height / 2) + (Screen.height / 2) * CC_VertRelativePos, 0.0f);

            // The compass orientation is modified by a custom amount.
            CC_GO.transform.rotation = Quaternion.Euler(CC_RotationCorrection);

            CC_GO.transform.localScale = new Vector3(CC_Size, CC_Size, 0.0f);

            //----//

            // The circular compass textures and their colors updates every frame, so you can change them in every moment during gameplay. 
            if(CC_BackVis)
            {
                CC_BackGO.SetActive(true);
                CC_Back.texture = CC_BackTexture;
                CC_Back.color = CC_BackColor;
            }
            else
            {
                CC_BackGO.SetActive(false);
            }

            //----//

            if(CC_Arrow1Vis)
            {
                CC_Arrow1GO.SetActive(true);
                CC_Arrow1.texture = CC_Arrow1Texture;
                CC_Arrow1.color = CC_Arrow1Color;
            }
            else
            {
                CC_Arrow1GO.SetActive(false);
            }

            //----//

            if(CC_Arrow2Vis)
            {
                CC_Arrow2GO.SetActive(true);
                CC_Arrow2.texture = CC_Arrow2Texture;
                CC_Arrow2.color = CC_Arrow2Color;
            }
            else
            {
                CC_Arrow2GO.SetActive(false);
            }

            //----//

            switch(CC_Behavior)
            {
                // This case allows only the movement of the background and the player arrow textures.
                case WorldCompass.Circular.Behavior.Fixed_Arrow:        float CC_BackAngle = CalculateCompassRotation(this.transform, OR_Camera);
                                                                        if(CC_IstantaneousMovement)
                                                                        {
                                                                            if(CC_BackVis)
                                                                            {
                                                                                // The texture of the background rotates following the camera rotatation (in degrees) around the Y-axis.
                                                                                //CC_Back.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, this.transform.eulerAngles.y + CC_BackRotCorrection);
                                                                                CC_Back.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, CC_BackAngle + CC_BackRotCorrection);
                                                                            }

                                                                            if(CC_Arrow1Vis)
                                                                            {
                                                                                // The player arrow show where the player model is looking at. So it rotates as much as the angle between the camera and player model forward axes.
                                                                                //CC_Arrow1.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -Vector3.SignedAngle(this.transform.forward, CC_PlayerGO.transform.forward, Vector3.up) + CC_Arrow1RotCorrection);//rivedere
                                                                                CC_Arrow1.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -CalculateCompassRotation(CC_PlayerGO.transform, OR_Player) + CC_Arrow1RotCorrection + (CC_BackAngle + CC_BackRotCorrection));
                                                                            }
                                                                            
                                                                            if(CC_Arrow2Vis)
                                                                            {
                                                                                // The camera arrow doesn't rotate, except for its correction value.
                                                                                CC_Arrow2.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, CC_Arrow2RotCorrection);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            // Same as before, but the rotation are linearly interpolated.
                                                                            if(CC_BackVis)
                                                                            {
                                                                                CC_Back.transform.localRotation = Quaternion.Lerp(CC_Back.transform.localRotation, Quaternion.Euler(0.0f, 0.0f, CC_BackAngle + CC_BackRotCorrection), CC_Speed * Time.deltaTime);
                                                                            }
                                                                            
                                                                            if(CC_Arrow1Vis)
                                                                            {
                                                                                if((CC_Arrow1.transform.localRotation.z >= CC_Arrow1RotCorrection + (CC_Arrow1RotCorrection * 0.01f)) || (CC_Arrow1.transform.localRotation.z <= CC_Arrow1RotCorrection - (CC_Arrow1RotCorrection * 0.01f)))
                                                                                {
                                                                                    CC_Arrow1.transform.localRotation = Quaternion.Lerp(CC_Arrow1.transform.localRotation, Quaternion.Euler(0.0f, 0.0f, -CalculateCompassRotation(CC_PlayerGO.transform, OR_Player) + CC_Arrow1RotCorrection + (CC_BackAngle + CC_BackRotCorrection)), CC_Speed * Time.deltaTime);
                                                                                }
                                                                            }

                                                                            if(CC_Arrow2Vis)
                                                                            {
                                                                                if((CC_Arrow2.transform.localRotation.z >= CC_Arrow2RotCorrection + (CC_Arrow2RotCorrection * 0.01f)) || (CC_Arrow2.transform.localRotation.z <= CC_Arrow2RotCorrection - (CC_Arrow2RotCorrection * 0.01f)))
                                                                                {
                                                                                    CC_Arrow2.transform.localRotation = Quaternion.Lerp(CC_Arrow2.transform.localRotation, Quaternion.Euler(0.0f, 0.0f, CC_Arrow2RotCorrection), CC_Speed * Time.deltaTime);
                                                                                }
                                                                            }
                                                                        }

                                                                        break;

                //This case allows only the movement of camera and player arrows.
                case WorldCompass.Circular.Behavior.Fixed_Background:   if(CC_IstantaneousMovement)
                                                                        {
                                                                            if(CC_Arrow1Vis)
                                                                            {
                                                                                // The player arrow rotates following the player rotatation (in degrees) around the Y-axis.
                                                                                //CC_Arrow1.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -CC_PlayerGO.transform.eulerAngles.y + CC_Arrow1RotCorrection);
                                                                                CC_Arrow1.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -CalculateCompassRotation(CC_PlayerGO.transform, OR_Player) + CC_Arrow1RotCorrection);
                                                                            }
                                                                        
                                                                            if(CC_Arrow2Vis)
                                                                            {
                                                                                // The camera arrow rotates following the camera rotatation (in degrees) around the Y-axis.
                                                                                CC_Arrow2.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -CalculateCompassRotation(this.transform, OR_Camera) + CC_Arrow2RotCorrection);
                                                                            }

                                                                            if(CC_BackVis)
                                                                            {
                                                                                // The camera arrow doesn't rotate, except for its correction value.
                                                                                CC_Back.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, CC_BackRotCorrection);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            // Same as before, but the rotation are linearly interpolated.
                                                                            if(CC_Arrow1Vis)
                                                                            {
                                                                                CC_Arrow1.transform.localRotation = Quaternion.Lerp(CC_Arrow1.transform.localRotation, Quaternion.Euler(0.0f, 0.0f, -CalculateCompassRotation(CC_PlayerGO.transform, OR_Player) + CC_Arrow1RotCorrection), CC_Speed * Time.deltaTime);
                                                                            }

                                                                            if(CC_Arrow2Vis)
                                                                            {
                                                                                CC_Arrow2.transform.localRotation = Quaternion.Lerp(CC_Arrow2.transform.localRotation, Quaternion.Euler(0.0f, 0.0f, -CalculateCompassRotation(this.transform, OR_Camera) + CC_Arrow2RotCorrection), CC_Speed * Time.deltaTime);
                                                                            }

                                                                            if((CC_Back.transform.localRotation.z >= CC_BackRotCorrection + (CC_BackRotCorrection * 0.01f)) || (CC_Back.transform.localRotation.z <= CC_BackRotCorrection - (CC_BackRotCorrection * 0.01f)))
                                                                            {
                                                                                CC_Back.transform.localRotation = Quaternion.Lerp(CC_Back.transform.localRotation, Quaternion.Euler(0.0f, 0.0f, CC_BackRotCorrection), CC_Speed * Time.deltaTime);
                                                                            }
                                                                        }

                                                                        break;
            }
        }
        else
        {
            CC_GO.SetActive(false);
        }

        //*****//

        // The horizontal compass textures and their colors are update every frame, so you can change them in every moment during gameplay. 
        if(HC_CompTexture != null)
        {
            HC_CompVis = true;
        }
        else
        {
            HC_CompVis = false;
        }

        if(HC_BackTexture != null)
        {
            HC_BackVis = true;
        }
        else
        {
            HC_BackVis = false;
        }

        if(HC_OverTexture != null)
        {
            HC_OverVis = true;
        }
        else
        {
            HC_OverVis = false;
        }

        //----//

        if(HC_ShowHorCompass)
        {
            HC_GO.SetActive(true);

            // The position depends on the actual screen resolution and the compass is anchored to the screen center. It can move only vertically.
            HC_GO.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 + (HC_VerticalPosition * (Screen.height / 2)), 0.0f);

            if(HC_CompVis)
            {
                HC_CompGO.SetActive(true);
                // The horizontal compass textures and their colors updates every frame, so you can change them in every moment during gameplay.
                HC_Comp.texture = HC_CompTexture;
                HC_Comp.color = HC_CompColor;

                if(HC_IstantaneousMovement)
                { 
                    // The cardinal points texture doesn't really move, but its horizontal UV Rect coordinates updates following the camera rotation around the Y-axis.
                    // The angle is divided for 360 degrees. The result goes from 0 to 1. To correct the orientation and aligning the texture with the north
                    // HC_RotationCorrection is added (it goes from 0 to 1).
                    // To show only a part of the texture the UV Rect width is reduced by HC_HorRelativeLenght. A higher value increases the cardinal points texture stretching.
                    //HC_Comp.uvRect = new Rect((this.transform.localEulerAngles.y / 360.0f) + HC_RotationCorrection, 0.0f, 1.0f - HC_HorRelativeLenght, 1.0f);
                    HC_Comp.uvRect = new Rect((CalculateCompassRotation(this.transform, OR_Camera) / 360.0f) + HC_RotationCorrection, 0.0f, 1.0f - HC_HorRelativeLenght, 1.0f);
                }
                else
                {
                    // To slowly move the texture when the camera moves, the script uses a custom Lerp function. For example, when an X value is 0.1 and the one to reach is 0.9,
                    // ShortestLerp(...) instead to increase the X value till 0.9, it reduces X till 0, then adds 1 to X and it continues to reduce X till it reaches 0.9.
                    //float HC_uvRectX = DLL_MathExt.Algebra.ShortestLerp(HC_Comp.uvRect.x, (this.transform.localEulerAngles.y / 360.0f) + HC_RotationCorrection, HC_RotationCorrection, 1.0f + HC_RotationCorrection, HC_Speed * Time.deltaTime);
                    float HC_uvRectX = DLL_MathExt.Algebra.ShortestLerp(HC_Comp.uvRect.x, (CalculateCompassRotation(this.transform, OR_Camera) / 360.0f) + HC_RotationCorrection, HC_RotationCorrection, 1.0f + HC_RotationCorrection, HC_Speed * Time.deltaTime);

                    HC_Comp.uvRect = new Rect(HC_uvRectX, 0.0f, 1.0f - HC_HorRelativeLenght, 1.0f);
                }

                if(HC_AdaptToScreenWidth)
                {
                    HC_Comp.rectTransform.sizeDelta = new Vector2(HC_LengthPercent * Screen.width, HC_HeightFixed);

                    HC_LengthFixed = HC_Comp.rectTransform.sizeDelta.x;
                }
                else
                {
                    HC_Comp.rectTransform.sizeDelta = new Vector2(HC_LengthFixed, HC_HeightFixed);

                    HC_LengthPercent = HC_Comp.rectTransform.sizeDelta.x / Screen.width;
                }
            }
            else
            {
                HC_CompGO.SetActive(false);
            }

            //----//

            if(HC_BackVis)
            {
                HC_BackGO.SetActive(true);
                HC_Back.texture = HC_BackTexture;
                HC_Back.color = HC_BackColor;

                HC_Back.rectTransform.sizeDelta = new Vector2(HC_Comp.rectTransform.rect.width * HC_BackRectRelLenght, HC_Comp.rectTransform.rect.height * HC_BackRectRelHeight);
                HC_Back.transform.position = HC_Comp.transform.position;
            }
            else
            {
                HC_BackGO.SetActive(false);
            }

            //----//

            if(HC_OverVis)
            {
                HC_OverGO.SetActive(true);
                HC_Over.texture = HC_OverTexture;
                HC_Over.color = HC_OverColor;

                HC_Over.rectTransform.sizeDelta = new Vector2(HC_Comp.rectTransform.rect.width * HC_OverRectRelLenght, HC_Comp.rectTransform.rect.height * HC_OverRectRelHeight);
                HC_Over.transform.position = HC_Comp.transform.position;
            }
            else
            {
                HC_OverGO.SetActive(false);
            }

            //----//
            
        }
        else
        {
            HC_GO.SetActive(false);
        }
    }

    //*****//

    public bool IsInizializing()
    {
        return WC_Inizialize;
    }

    //*****//

    public bool IsHorCompVisible()
    {
        return HC_ShowHorCompass;
    }

    //*****//

    public Vector2 GetHorCompassRect()
    {
        return HC_Comp.rectTransform.sizeDelta;
    }

    //*****//

    public Vector3 GetHorCompassPos()
    {
        return HC_Comp.transform.position;
    }
}

//*****//

public static class WorldCompass
{
    public static class Circular
    {
        public enum Behavior{Fixed_Arrow, Fixed_Background}
    }
    public static class Direction
    {
        public enum Zenit{X, Y, Z};
        public enum North{X, Y, Z};  
        public enum Forward{X, Y, Z}; 
    } 

}
