/*
   ___ ___  ___   ___ ___ ___  _   _ ___    _   _        
  | _ \ _ \/ _ \ / __| __|   \| | | | _ \  /_\ | |       
  |  _/   / (_) | (__| _|| |) | |_| |   / / _ \| |__     
  |_| |_|_\\___/ \___|___|___/ \___/|_|_\/_/ \_\____|    
                                                        
 ___ __  __   _   ___ ___ _  _   _ _____ ___ ___  _  _ 
|_ _|  \/  | /_\ / __|_ _| \| | /_\_   _|_ _/ _ \| \| |
 | || |\/| |/ _ \ (_ || || .` |/ _ \| |  | | (_) | .` |
|___|_|  |_/_/ \_\___|___|_|\_/_/ \_\_| |___\___/|_|\_|
                                                           

                                       DI CUSTOM EDITOR

                                   Author: Matteo Lusso
                                                 © 2021

*/

using System;
using UnityEngine;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(DynamicIcons))]
public class DICustomEditor : Editor
{
    private DynamicIcons DI;
    private string[] IconShowStrings = new string[System.Enum.GetNames(typeof(Icon.Show)).Length];
    private string[] IconStyleStrings = new string[System.Enum.GetNames(typeof(Icon.Style)).Length];
    private string[] IconOnScreenStrings = new string[System.Enum.GetNames(typeof(Icon.OnScreenPosition)).Length];
    private string[] IconOffScreenStrings = new string[System.Enum.GetNames(typeof(Icon.OffScreenPosition)).Length];
    private string[] CompassTypeStrings = new string[System.Enum.GetNames(typeof(Icon.CompassType)).Length];

    override public void OnInspectorGUI()
    {
        DI = target as DynamicIcons;

        //----// Enum strings

        IconShowStrings = System.Enum.GetNames(typeof(Icon.Show));
        IconStyleStrings = System.Enum.GetNames(typeof(Icon.Style));
        IconOnScreenStrings = System.Enum.GetNames(typeof(Icon.OnScreenPosition));
        IconOffScreenStrings = System.Enum.GetNames(typeof(Icon.OffScreenPosition));
        CompassTypeStrings = System.Enum.GetNames(typeof(Icon.CompassType));

        //----// Display raw editor

        if(DI.DEBUG_ShowFullEditor)
        {
            goto DisplayFullEditor;
        }

        //----// Common fields       

        EditorGUILayout.LabelField("ICON STATUS:", EditorStyles.boldLabel);

        if(DI.SCRIPT_IsSlave)
        {
            DI.SCRIPT_IsSlave = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Depending on other Dynamic Icons script?", "If true, the script will get some variables values from another DynamicIcon script."), DI.SCRIPT_IsSlave);
            DI.DI_Master = (DynamicIcons)EditorGUILayout.ObjectField(new GUIContent("└ Master script:", "Use a squared texture or the icon will not keep its aspect ratio."), DI.DI_Master, typeof(DynamicIcons), true);
        }
        else
        {
            DI.SCRIPT_IsSlave = (bool)EditorGUILayout.Toggle(new GUIContent(" - Depending on other Dynamic Icons script?", "If true, the script will get some variables values from another DynamicIcon script to reduce calculations."), DI.SCRIPT_IsSlave);
        }

        EditorGUILayout.Space();

        DI.ICON_ScriptName = (string)EditorGUILayout.TextField(new GUIContent("┌ Script identifier", "It's the script name, use it to distinguish from the other same scripts attached to the GameObject."), DI.ICON_ScriptName);
        DI.SCRIPT_Order = (int)EditorGUILayout.IntField(new GUIContent("├ Visibility order", "Higher number put the icon over the lower number icons. Icon with order number -1 will be covered by icons with order number 0, 0 by 1, and so on."), DI.SCRIPT_Order);
        DI.ICON_Active = (bool)EditorGUILayout.Toggle(new GUIContent("└ Active?", "If true, the indicator will be visibile."), DI.ICON_Active);

        if(DI.ICON_Active)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ICON TEXTURE:", EditorStyles.boldLabel);

            DI.ICON_Texture2D = (Texture2D)EditorGUILayout.ObjectField(new GUIContent(" - Texture:", "Use a squared texture or the icon will not keep its aspect ratio."), DI.ICON_Texture2D, typeof(Texture2D), false);
            DI.ICON_RotationCorrection = (float)EditorGUILayout.Slider(new GUIContent(" - Base rotation:", "Set an angle in degrees to correct the texture rotation."), DI.ICON_RotationCorrection, 0.0f, 360.0f);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ICON STYLE:", EditorStyles.boldLabel);
                
            DI.ICON_Style = (Icon.Style)GUILayout.Toolbar((int)DI.ICON_Style, IconStyleStrings);

            //----//

            switch(DI.ICON_Style)
            {
                case Icon.Style.Indicator:  EditorGUILayout.Space();

                                            if(DI.ICON_Visibility == Icon.Show.Always)
                                            {
                                                EditorGUILayout.LabelField("ICON IS VISIBLE:", EditorStyles.boldLabel);
                                            }
                                            else
                                            {
                                                EditorGUILayout.LabelField("ICON IS VISIBLE ONLY WHEN TARGET IS:", EditorStyles.boldLabel);
                                            }

                                            DI.ICON_Visibility = (Icon.Show)GUILayout.Toolbar((int)DI.ICON_Visibility, IconShowStrings);

                                            //----//

                                            if(DI.ICON_Visibility == Icon.Show.Always || DI.ICON_Visibility == Icon.Show.On_Screen)
                                            {
                                                EditorGUILayout.Space();
                                                EditorGUILayout.LabelField("ON-SCREEN BEHAVIOR:", EditorStyles.boldLabel);

                                                DI.ICON_OnScreenBehavior = (Icon.OnScreenPosition)GUILayout.Toolbar((int)DI.ICON_OnScreenBehavior, IconOnScreenStrings);
                                            }

                                            //----//

                                            if(DI.ICON_Visibility == Icon.Show.Always || DI.ICON_Visibility == Icon.Show.Off_Screen)
                                            {
                                                EditorGUILayout.Space();
                                                EditorGUILayout.LabelField("OFF-SCREEN BEHAVIOR:", EditorStyles.boldLabel);

                                                DI.ICON_OffScreenBehavior = (Icon.OffScreenPosition)GUILayout.Toolbar((int)DI.ICON_OffScreenBehavior, IconOffScreenStrings);

                                                if(DI.ICON_OffScreenBehavior == Icon.OffScreenPosition.Circular_Rotation)
                                                {
                                                    DI.ICON_Eccentricity = (float)EditorGUILayout.Slider(new GUIContent(" - Circle eccentricity: ", "If this value is zero the icon will follow a circular pattern around the screen center when the target's GameObject is not visible. Set to 1 to get the icon to follow an elliptical pattern."), DI.ICON_Eccentricity, 0.0f, 1.0f);
                                                }
                                            }

                                            //----//

                                            EditorGUILayout.Space();
                                            EditorGUILayout.LabelField("POSITION SETTINGS:", EditorStyles.boldLabel);

                                            if(DI.ICON_Visibility == Icon.Show.Always)
                                            {
                                                DI.ICON_DistanceFromBorderInPixels = (float)EditorGUILayout.FloatField(new GUIContent("┌ Distance from screen border:", "Set the the indicator distance (in pixels) from the screen limits."), DI.ICON_DistanceFromBorderInPixels);
                                                if(DI.ICON_OnScreenBehavior != Icon.OnScreenPosition.Centered_On_Target)
                                                {
                                                    DI.ICON_DistanceFromTargetInPixels = (float)EditorGUILayout.FloatField(new GUIContent("├ Distance from target:", "Set the the indicator distance (in pixels) from the target."), DI.ICON_DistanceFromTargetInPixels);
                                                }
                                            }
                                            else if(DI.ICON_Visibility == Icon.Show.Off_Screen)
                                            {
                                                DI.ICON_DistanceFromBorderInPixels = (float)EditorGUILayout.FloatField(new GUIContent("┌ Distance from screen border:", "Set the the indicator distance (in pixels) from the screen limits."), DI.ICON_DistanceFromBorderInPixels);
                                            }
                                            else if(DI.ICON_Visibility == Icon.Show.On_Screen)
                                            {
                                                if(DI.ICON_OnScreenBehavior != Icon.OnScreenPosition.Centered_On_Target)
                                                {
                                                    DI.ICON_DistanceFromTargetInPixels = (float)EditorGUILayout.FloatField(new GUIContent("┌ Distance from target:", "Set the the indicator distance (in pixels) from the target."), DI.ICON_DistanceFromTargetInPixels);
                                                }
                                            }

                                            if(DI.ICON_OnScreenBehavior == Icon.OnScreenPosition.Relative_To_Target)
                                            {
                                                DI.ICON_FixedAngle = (float)EditorGUILayout.Slider(new GUIContent("├ Fixed angle:"), DI.ICON_FixedAngle, 0.0f, 360.0f);
                                            }

                                            if(!DI.ICON_UpdatePositionInstantaneously)
                                            {
                                                DI.ICON_UpdatePositionInstantaneously = (bool)EditorGUILayout.Toggle(new GUIContent("├ Move instantaneously?", "If true, the indicator moves instantaneously to the new position."), DI.ICON_UpdatePositionInstantaneously);
                                                DI.ICON_MovementSmoothFactor = (float)EditorGUILayout.FloatField(new GUIContent("└─ Movement speed:", "Set the movement speed of the icon.\nHigher values increase the speed, lower reduce it"), DI.ICON_MovementSmoothFactor);
                                            }
                                            else
                                            {
                                                DI.ICON_UpdatePositionInstantaneously = (bool)EditorGUILayout.Toggle(new GUIContent("└ Move instantaneously?", "If true, the indicator moves instantaneously to the new position."), DI.ICON_UpdatePositionInstantaneously);
                                            }

                                            //----//

                                            if((DI.ICON_OnScreenBehavior == Icon.OnScreenPosition.Follow_Target || DI.ICON_OnScreenBehavior == Icon.OnScreenPosition.Follow_Target_Inverse) && DI.ICON_Visibility != Icon.Show.Off_Screen)
                                            {
                                                EditorGUILayout.Space();
                                                EditorGUILayout.LabelField("ANTI-OVERLAP SETTINGS:", EditorStyles.boldLabel);

                                                DI.ICON_AvoidCenter = (float)EditorGUILayout.FloatField(new GUIContent("┌ Start to avoid center at:", "When the distance in pixels between the screen center and the target is less than this value, for the next calculations the screen center will be decentered by a custom amount.\n\nThis avoids the indicator overlaps the target when the target is near the screen center."), DI.ICON_AvoidCenter);
                                                DI.ICON_Decentration = (Vector2)EditorGUILayout.Vector2Field(new GUIContent("└ Decentration:", "Use those values to modify the coordinates of the screen center and avoid the indicator overlaps the target."), DI.ICON_Decentration);
                                            }

                                            break;

                //----//

                case Icon.Style.Compass:    EditorGUILayout.Space();
                                            EditorGUILayout.LabelField("COMPASS TYPE:", EditorStyles.boldLabel);
                
                                            DI.COMPASS_Type = (Icon.CompassType)GUILayout.Toolbar((int)DI.COMPASS_Type, CompassTypeStrings);
                                            
                                            switch(DI.COMPASS_Type)
                                            {
                                                case Icon.CompassType.Arrow:        EditorGUILayout.Space();
                                                                                    EditorGUILayout.LabelField("COMPASS POSITION SETTINGS:", EditorStyles.boldLabel);

                                                                                    DI.COMPASS_PosX = (float)EditorGUILayout.Slider(new GUIContent("┌ Horizontal position from screen center: ", "Use the slider to move the compass left or right in a fixed position."), DI.COMPASS_PosX, -1.0f, 1.0f);
                                                                                    DI.COMPASS_PosY = (float)EditorGUILayout.Slider(new GUIContent("└ Vertical position from screen center:", "Use the slider to move the compass up or down in a fixed position."), DI.COMPASS_PosY, -1.0f, 1.0f);
                                                                                    
                                                                                    EditorGUILayout.Space();

                                                                                    DI.COMPASS_OrientationCorrections = (Vector3)EditorGUILayout.Vector3Field(new GUIContent(" - Rotation correction: ", "Use those values to rotate the icon around the x and y axis until it aims to the target."), DI.COMPASS_OrientationCorrections);

                                                                                    break;

                                                case Icon.CompassType.Horizontal:   EditorGUILayout.Space();
                                                                                    EditorGUILayout.LabelField("HORIZONTAL COMPASS POSITION SETTINGS:", EditorStyles.boldLabel);

                                                                                    if(DI.COMPASS_AutoCalculateFromWorldComp)
                                                                                    {
                                                                                        DI.COMPASS_AutoCalculateFromWorldComp = (bool)EditorGUILayout.Toggle(new GUIContent(" - Use world compass data?"), DI.COMPASS_AutoCalculateFromWorldComp);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        DI.COMPASS_AutoCalculateFromWorldComp = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Use world compass data?"), DI.COMPASS_AutoCalculateFromWorldComp);
                                                                                        DI.COMPASS_HorLenght = (float)EditorGUILayout.Slider(new GUIContent("└┬ Horizontal lenght:", " It's the length of the compass, calculated as a percentage of the horizontal resolution."), DI.COMPASS_HorLenght, 0.0f, 1.0f);
                                                                                        DI.COMPASS_PosY = (float)EditorGUILayout.Slider(new GUIContent("   └ Vertical position from screen center:", "Use the slider to move the compass up or down in a fixed position."), DI.COMPASS_PosY, -1.0f, 1.0f);
                                                                                    }

                                                                                    break;

                                                case Icon.CompassType.Vertical:     EditorGUILayout.Space();
                                                                                    EditorGUILayout.LabelField("VERTICAL COMPASS POSITION SETTINGS:", EditorStyles.boldLabel);

                                                                                    DI.COMPASS_VertLenght = (float)EditorGUILayout.Slider(new GUIContent("┌ Vertical lenght:", " It's the length of the compass, calculated as a percentage of the vertical resolution."), DI.COMPASS_VertLenght, 0.0f, 1.0f);
                                                                                    DI.COMPASS_PosX = (float)EditorGUILayout.Slider(new GUIContent("└ Horizontal position from screen center:", "Use the slider to move the compass left or right in a fixed position."), DI.COMPASS_PosX, -1.0f, 1.0f);

                                                                                    break;  
                                            }

                                            break;
            }

            //----//

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("SCALING SETTINGS:", EditorStyles.boldLabel);

            DI.ICON_MaxSize = (float)EditorGUILayout.FloatField(new GUIContent("┌ Max dimension:", "The max size the indicator can reach.\n\n This value is also the indicator base size (it ovverrides the RawImage x and y dimensions set in the editor)."), DI.ICON_MaxSize);

            if(DI.ICON_Scale)
            {
                DI.ICON_Scale = (bool)EditorGUILayout.Toggle(new GUIContent("└┬ Scale size?", "If true, the indicator will reduce its size based on the distance between the camera and the target."), DI.ICON_Scale);
                DI.ICON_DistanceCurve = EditorGUILayout.CurveField(new GUIContent("   ├ f(d):", "The y axis of this custom curve goes from 0 to 1, while the x-axis is the distance from the camera to the target. You can customize this curve between 0 and d[MAX].\n\nFor example: if your curve values 0.5 at a distance of 200 (f(200) = 0.5), the texture size will be reduced by 50% as the distance in pixels between the icon and the target.\nThat allows having an icon that changes dynamically its size and space from the target according to the distance between the camera and the target itself."), DI.ICON_DistanceCurve, Color.white, new Rect(0, 0, DI.ICON_DistanceCurveMaxDistance, 1));
                DI.ICON_DistanceCurveMaxDistance = (float)EditorGUILayout.FloatField(new GUIContent("   └ d[MAX]:", "It's the last value of the x-axis of the previous curve f(d). The associated value of the curve for d[MAX] should be 0 (f(d[MAX]) = 0)."), DI.ICON_DistanceCurveMaxDistance);
            }
            else
            {
                DI.ICON_Scale = (bool)EditorGUILayout.Toggle(new GUIContent("└ Scale size?", "If true, the indicator will reduce its size based on the distance between the camera and the target."), DI.ICON_Scale);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("FADING SETTINGS:", EditorStyles.boldLabel);

            if(DI.ICON_Fade)
            {
                DI.ICON_Fade = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Fade in/out?", "If it's true, the icon will fade away when the object is enought distant."), DI.ICON_Fade);

                //if(DI.ICON_Visibility == Icon.Show.Always || DI.ICON_Visibility == Icon.Show.On_Screen)
                //{
                DI.TARGET_MinVisibleDistanceOnScreen = (float)EditorGUILayout.FloatField(new GUIContent("└┬ Min distance to be visible:", "Set the minimum distance between camera and target, before which the indicator will fade out when the target is visible."), DI.TARGET_MinVisibleDistanceOnScreen);
                DI.TARGET_MaxVisibleDistance = (float)EditorGUILayout.FloatField(new GUIContent("   ├ Max distance to be visible:", "When the distance between the camera and the target is higher than this value, the indicator will fade out.\n\nThe previous curve will move from 0 to this value.\nThe distance between the indicator and the target as the texture size will change following the previous curve."), DI.TARGET_MaxVisibleDistance);

                if(!DI.ICON_UpdateAlphaInstantaneously)
                {
                    DI.ICON_UpdateAlphaInstantaneously = (bool)EditorGUILayout.Toggle(new GUIContent("   └┬ Instantaneous fading?", "If true, the indicator will appears and disappears instantaneously."), DI.ICON_UpdateAlphaInstantaneously);
                    DI.ICON_FadeSmoothFactor = (float)EditorGUILayout.FloatField(new GUIContent("      └─ Fading speed:", "Set the fading speed of the icon.\n\nHigher value increases the speed, lower reduce it."), DI.ICON_FadeSmoothFactor);
                }
                else
                {
                    DI.ICON_UpdateAlphaInstantaneously = (bool)EditorGUILayout.Toggle(new GUIContent("   └─ Instantaneous fading?", "If true, the indicator will appears and disappears instantaneously."), DI.ICON_UpdateAlphaInstantaneously);
                }
            }
            else
            {
                DI.ICON_Fade = (bool)EditorGUILayout.Toggle(new GUIContent("   Fade in/out?", "If it's true, the icon will fade away when the object is enought distant."), DI.ICON_Fade);
            }

            //----//

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ROTATION SETTINGS:", EditorStyles.boldLabel);
    
            if(DI.ICON_Style != Icon.Style.Compass || (DI.ICON_Style == Icon.Style.Compass && DI.COMPASS_Type != Icon.CompassType.Arrow))
            {
                if(DI.ICON_Rotation)
                {
                    DI.ICON_Rotation = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Point to target?", "If true, the indicator will not point to the target.\n\n Except for Up, Down and Center behaviours"), DI.ICON_Rotation);

                    if(!DI.ICON_UpdateOrientationInstantaneously)
                    {
                        DI.ICON_UpdateOrientationInstantaneously = (bool)EditorGUILayout.Toggle(new GUIContent("└┬ Instantaneous rotation?", "If true, the indicator rotation is istantaneous."), DI.ICON_UpdateOrientationInstantaneously);

                        DI.ICON_RotationSmoothFactor = (float)EditorGUILayout.FloatField(new GUIContent("   └─ Rotation speed:", "Set the rotation speed of the icon.\n\nHigher value increases the speed, lower reduce it."), DI.ICON_RotationSmoothFactor);
                    }
                    else
                    {
                        DI.ICON_UpdateOrientationInstantaneously = (bool)EditorGUILayout.Toggle(new GUIContent("└─ Instantaneous rotation?", "If true, the indicator rotation is istantaneous."), DI.ICON_UpdateOrientationInstantaneously);
                    }
                }
                else
                {
                    DI.ICON_Rotation = (bool)EditorGUILayout.Toggle(new GUIContent(" - Point to target?", "If true, the indicator will not point to the target.\n\n Except for Up, Down and Center behaviours"), DI.ICON_Rotation);
                }
            }

            else
            {
                if(!DI.ICON_UpdateOrientationInstantaneously)
                {
                    DI.ICON_UpdateOrientationInstantaneously = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Instantaneous rotation?", "If true, the indicator rotation is istantaneous."), DI.ICON_UpdateOrientationInstantaneously);

                    DI.ICON_RotationSmoothFactor = (float)EditorGUILayout.FloatField(new GUIContent("└─ Rotation speed:", "Set the rotation speed of the icon.\n\nHigher value increases the speed, lower reduce it."), DI.ICON_RotationSmoothFactor);
                }
                else
                {
                    DI.ICON_UpdateOrientationInstantaneously = (bool)EditorGUILayout.Toggle(new GUIContent(" - Instantaneous rotation?", "If true, the indicator rotation is istantaneous."), DI.ICON_UpdateOrientationInstantaneously);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("COLOR SETTINGS:", EditorStyles.boldLabel);

            if(DI.ICON_ChangeColor)
            {
                DI.ICON_ChangeColor = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Change icon color?", "If true, the icon will change its color based on target position."), DI.ICON_ChangeColor);

                DI.ICON_AheadColor = (Color)EditorGUILayout.ColorField(new GUIContent("└┬ Icon color (target ahead camera)"), DI.ICON_AheadColor);
                DI.ICON_RearColor = (Color)EditorGUILayout.ColorField(new GUIContent("   ├ Icon color (target rear camera)"), DI.ICON_RearColor);

                if(!DI.ICON_UpdateColorInstantaneously)
                {
                    DI.ICON_UpdateColorInstantaneously = (bool)EditorGUILayout.Toggle(new GUIContent("   ├ Instantaneous color changing?", "If true, the indicator will change color instantaneously."), DI.ICON_UpdateColorInstantaneously);
                    DI.ICON_ColorSmoothFactor = (float)EditorGUILayout.FloatField(new GUIContent("   └─ Color changing speed:", "Set the color changing speed of the icon.\n\nHigher value increases the speed, lower reduce it."), DI.ICON_ColorSmoothFactor);
                }
                else
                {
                    DI.ICON_UpdateColorInstantaneously = (bool)EditorGUILayout.Toggle(new GUIContent("   └ Instantaneous color changing?", "If true, the indicator will change color instantaneously."), DI.ICON_UpdateColorInstantaneously);
                }
            }
            else
            {
                DI.ICON_ChangeColor = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Change icon color?", "If true, the icon will change its color based on target position."), DI.ICON_ChangeColor);
                DI.ICON_Color = (Color)EditorGUILayout.ColorField(new GUIContent("└─Icon fixed color"), DI.ICON_Color);
            }

            //----//

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("CUSTOM TEXT SETTINGS:", EditorStyles.boldLabel);

            if(DI.ICON_ShowCustomText)
            {
                DI.ICON_ShowCustomText = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Show custom text?", "If true, a custom text will be shoed near the icon"), DI.ICON_ShowCustomText);

                DI.CT_String = (string)EditorGUILayout.TextField(new GUIContent("├─ Text to show:"), DI.CT_String);
                DI.CT_Color = (Color)EditorGUILayout.ColorField(new GUIContent("├─ Text color:"), DI.CT_Color);
                DI.CT_Font = (TMP_FontAsset)EditorGUILayout.ObjectField(new GUIContent("├─ Text font:"), DI.CT_Font, typeof(TMP_FontAsset), false);

                DI.CT_Behavior = (IconText.TextPosition)EditorGUILayout.EnumPopup(new GUIContent("├─ Text behaviour:", "Set how the the text will follow the icon."), DI.CT_Behavior);

                DI.CT_MaxSize = (float)EditorGUILayout.FloatField(new GUIContent("├─ Text size:", "The maximum dimension of the text"), DI.CT_MaxSize);

                if(DI.CT_Behavior == IconText.TextPosition.Fixed_Position)
                {
                    DI.CT_MinDistanceFromIcon = (float)EditorGUILayout.FloatField(new GUIContent("├─ Min distance from icon:", "Set the min distance in pixels between text and icon."), DI.CT_MinDistanceFromIcon);
                    DI.CT_MaxDistanceFromIcon = (float)EditorGUILayout.FloatField(new GUIContent("├─ Max distance from icon:", "Set the max distance in pixels between text and icon."), DI.CT_MaxDistanceFromIcon);

                    DI.CT_FixedAngle = (float)EditorGUILayout.Slider(new GUIContent("└─ Fixed angle:"), DI.CT_FixedAngle, 0.0f, 360.0f);
                }
                else
                {
                    DI.CT_MinDistanceFromIcon = (float)EditorGUILayout.FloatField(new GUIContent("├─ Min distance from icon:", "Set the min distance in pixels between text and icon."), DI.CT_MinDistanceFromIcon);
                    DI.CT_MaxDistanceFromIcon = (float)EditorGUILayout.FloatField(new GUIContent("└─ Max distance from icon:", "Set the distance in pixels between text and icon."), DI.CT_MaxDistanceFromIcon);
                }
            }
            else
            {
                DI.ICON_ShowCustomText = (bool)EditorGUILayout.Toggle(new GUIContent(" - Show custom text?", "If true, a custom text will be shoed near the icon"), DI.ICON_ShowCustomText);
            }

            //----//

            /**/EditorGUILayout.Space();
            /**/EditorGUILayout.LabelField("DISTANCE TEXT SETTINGS:", EditorStyles.boldLabel);

            if(DI.ICON_ShowDistance)
            {
                DI.ICON_ShowDistance = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Show distance?", "If true, the distance from the target will be visible."), DI.ICON_ShowDistance);

                DI.DT_Unit = (IconText.Unit)EditorGUILayout.EnumPopup(new GUIContent("├─ Distance unit:", "Set the distance unit. By default 1 Unity unit = 1 meter."), DI.DT_Unit);
                DI.DT_CustomUnitSize = (float)EditorGUILayout.FloatField(new GUIContent("├─ Unit size:", "By default 1 Unity unit = 1 meter. If not true change this value.\n\nExample: if 1 Unity unit equals to 7 meters for you, you should set this value to 7."), DI.DT_CustomUnitSize);
                DI.DT_Color = (Color)EditorGUILayout.ColorField(new GUIContent("├─ Text color:"), DI.DT_Color);
                DI.DT_Font = (TMP_FontAsset)EditorGUILayout.ObjectField(new GUIContent("├─ Text font:"), DI.DT_Font, typeof(TMP_FontAsset), false);
                
                DI.DT_Behavior = (IconText.DistancePosition)EditorGUILayout.EnumPopup(new GUIContent("├─ Text behaviour:", "Set how the the text will follow the icon."), DI.DT_Behavior);

                DI.DT_MaxSize = (float)EditorGUILayout.FloatField(new GUIContent("├─ Text size:", "The maximum dimension of the text"), DI.DT_MaxSize);

                if(DI.DT_Behavior == IconText.DistancePosition.Fixed_Position)
                {
                    DI.DT_MinDistanceFromIcon = (float)EditorGUILayout.FloatField(new GUIContent("├─ Min distance from icon:", "Set the min distance in pixels between text and icon."), DI.DT_MinDistanceFromIcon);
                    DI.DT_MaxDistanceFromIcon = (float)EditorGUILayout.FloatField(new GUIContent("├─ Max distance from icon:", "Set the max distance in pixels between text and icon."), DI.DT_MaxDistanceFromIcon);

                    DI.DT_FixedAngle = (float)EditorGUILayout.Slider(new GUIContent("└─ Fixed angle:"), DI.DT_FixedAngle, 0.0f, 360.0f);
                }
                else
                {
                    DI.DT_MinDistanceFromIcon = (float)EditorGUILayout.FloatField(new GUIContent("├─ Min distance from icon:", "Set the min distance in pixels between text and icon."), DI.DT_MinDistanceFromIcon);
                    DI.DT_MaxDistanceFromIcon = (float)EditorGUILayout.FloatField(new GUIContent("└─ Max distance from icon", "Set the distance in pixels between text and icon."), DI.DT_MaxDistanceFromIcon);
                }
            }
            else
            {
                DI.ICON_ShowDistance = (bool)EditorGUILayout.Toggle(new GUIContent(" - Show distance?", "If true, the distance from the target will be visible."), DI.ICON_ShowDistance);
            }
        }

        //----//

        /**/EditorGUILayout.Space();
        /**/EditorGUILayout.LabelField("DEBUG:", EditorStyles.boldLabel);

        DI.DEBUG_ShowFullEditor = (bool)EditorGUILayout.Toggle(new GUIContent(" - Show raw editor?", "Show and access to full raw editor variables"), DI.DEBUG_ShowFullEditor);

        goto Skip;

            DisplayFullEditor:

            DrawDefaultInspector();

        Skip:

        if(GUI.changed)
        {
		    EditorUtility.SetDirty(target);
        }
 
    }
}