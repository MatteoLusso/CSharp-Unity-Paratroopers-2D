/*
   ___ ___  ___   ___ ___ ___  _   _ ___    _   _        
  | _ \ _ \/ _ \ / __| __|   \| | | | _ \  /_\ | |       
  |  _/   / (_) | (__| _|| |) | |_| |   / / _ \| |__     
  |_| |_|_\\___/ \___|___|___/ \___/|_|_\/_/ \_\____|    
                                                        
 ___ __  __   _   ___ ___ _  _   _ _____ ___ ___  _  _ 
|_ _|  \/  | /_\ / __|_ _| \| | /_\_   _|_ _/ _ \| \| |
 | || |\/| |/ _ \ (_ || || .` |/ _ \| |  | | (_) | .` |
|___|_|  |_/_/ \_\___|___|_|\_/_/ \_\_| |___\___/|_|\_|
                                                           

                         DI WORLD COMPASS CUSTOM EDITOR

                                   Author: Matteo Lusso
                                                 © 2021

*/

using System;
using UnityEngine;
using UnityEditor;
using DLL_EditorExt;

[CustomEditor(typeof(DIWorldCompass))]
public class DIWCCustomEditor : Editor
{
    private DIWorldCompass WC;

    private string[] CircularBehaviorStrings = new string[System.Enum.GetNames(typeof(WorldCompass.Circular.Behavior)).Length];

    override public void OnInspectorGUI()
    {
        WC = target as DIWorldCompass;

        CircularBehaviorStrings = System.Enum.GetNames(typeof(WorldCompass.Circular.Behavior));

        if(WC.DEBUG_ShowFullEditor)
        {
            goto DisplayFullEditor;
        }

        EditorGUILayout.LabelField("NORTH SETTINGS COMPASS:", EditorStyles.boldLabel);

        WC.OR_Camera = (WorldCompass.Direction.Forward)EditorGUILayout.EnumPopup(new GUIContent("┌ Camera direction:", "Set the camera axis from wich is calculated to compass orientation."), WC.OR_Camera);
        if(WC.CC_PlayerGO != null && WC.CC_ShowCompass)
        {
            WC.OR_Player = (WorldCompass.Direction.Forward)EditorGUILayout.EnumPopup(new GUIContent("├ Player direction:", "Set the player axis from wich is calculated to compass orientation."), WC.OR_Player);
        }
        WC.OR_North = (WorldCompass.Direction.North)EditorGUILayout.EnumPopup(new GUIContent("├ North direction:", "Set the north direction."), WC.OR_North);
        WC.OR_Zenit = (WorldCompass.Direction.Zenit)EditorGUILayout.EnumPopup(new GUIContent("├ Zenit direction:", "Set the zenit direction."), WC.OR_Zenit);
        WC.OR_InverseZenit = (bool)EditorGUILayout.Toggle(new GUIContent("└ Inverse zenit direction?", "If true, the circular compass will be visibile."), WC.OR_InverseZenit);
        

        EditorGUILayout.LabelField("CIRCULAR COMPASS:", EditorStyles.boldLabel);
        
        if(WC.CC_ShowCompass)
        {
            WC.CC_ShowCompass = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Visible?", "If true, the circular compass will be visibile."), WC.CC_ShowCompass);
            WC.CC_PlayerGO = (GameObject)EditorGUILayout.ObjectField(new GUIContent("└ Player Gameobject:", "If you need to show where the player character is pointing, add its GameObject to this field."), WC.CC_PlayerGO, typeof(GameObject), true);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("TEXTURES:", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            WC.CC_BackTexture = DLL_EditorExt.CustomGUI.CustomField("BACKGROUND:\n\n• Texture", WC.CC_BackTexture, 100, 110, TextAnchor.MiddleCenter);
            if(WC.CC_PlayerGO != null)
            {
                WC.CC_Arrow1Texture = DLL_EditorExt.CustomGUI.CustomField("PLAYER ARROW:\n\n• Texture", WC.CC_Arrow1Texture, 100, 110, TextAnchor.MiddleCenter);
            }
            WC.CC_Arrow2Texture = DLL_EditorExt.CustomGUI.CustomField("CAMERA ARROW:\n\n• Texture", WC.CC_Arrow2Texture, 100, 110, TextAnchor.MiddleCenter);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            WC.CC_BackColor = DLL_EditorExt.CustomGUI.CustomField("• Color", WC.CC_BackColor, 20, 110, TextAnchor.MiddleCenter);
            if(WC.CC_PlayerGO != null)
            {
                WC.CC_Arrow1Color = DLL_EditorExt.CustomGUI.CustomField("• Color", WC.CC_Arrow1Color, 20, 110, TextAnchor.MiddleCenter);
            }
            WC.CC_Arrow2Color = DLL_EditorExt.CustomGUI.CustomField("• Color", WC.CC_Arrow2Color, 20, 110, TextAnchor.MiddleCenter);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("DIMENSIONS:", EditorStyles.boldLabel);

            WC.CC_Size = (float)EditorGUILayout.FloatField(new GUIContent("- Size:", "Set the the circular compass size. Background and arrows textures must have the same squared resolution."), WC.CC_Size);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("BEHAVIOR:", EditorStyles.boldLabel);

            WC.CC_Behavior = (WorldCompass.Circular.Behavior)GUILayout.Toolbar((int)WC.CC_Behavior, CircularBehaviorStrings);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("POSITION:", EditorStyles.boldLabel);

            WC.CC_HorRelativePos = (float)EditorGUILayout.Slider(new GUIContent("┌ Horizontal position from screen center: ", "Use this slider to move the compass horizontally."), WC.CC_HorRelativePos, -1.0f, 1.0f);
            WC.CC_VertRelativePos = (float)EditorGUILayout.Slider(new GUIContent("└ Vertical position from screen center: ", "Use this slider to move the compass vertically."), WC.CC_VertRelativePos, -1.0f, 1.0f);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ROTATION:", EditorStyles.boldLabel);

            if(WC.CC_IstantaneousMovement)
            {
                WC.CC_IstantaneousMovement = (bool)EditorGUILayout.Toggle(new GUIContent(" - Rotate instantaneously?", "If true, the compass rotate instantaneously."), WC.CC_IstantaneousMovement);
            }
            else
            {
                WC.CC_IstantaneousMovement = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Rotate instantaneously?", "If true, the compass rotate instantaneously."), WC.CC_IstantaneousMovement);
                WC.CC_Speed = (float)EditorGUILayout.FloatField(new GUIContent("└ Rotation speed:", "Set the compass rotation speed."), WC.CC_Speed);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("CORRECTIONS:", EditorStyles.boldLabel);

            WC.CC_BackRotCorrection = (float)EditorGUILayout.Slider(new GUIContent("┌ Background angle correction: ", "If the texture isn't point to the right cardinal point, you can rotate it around its forward axis and correct the position."), WC.CC_BackRotCorrection, 0.0f, 360.0f);
            WC.CC_Arrow1RotCorrection = (float)EditorGUILayout.Slider(new GUIContent("├ Player arrow angle correction: ", "If the texture isn't point to the right cardinal point, you can rotate it around its forward axis and correct the position."), WC.CC_Arrow1RotCorrection, 0.0f, 360.0f);
            WC.CC_Arrow2RotCorrection = (float)EditorGUILayout.Slider(new GUIContent("└ Camera arrow angle correction: ", "If the texture isn't point to the right cardinal point, you can rotate it around its forward axis and correct the position."), WC.CC_Arrow2RotCorrection, 0.0f, 360.0f);

            EditorGUILayout.Space();

            WC.CC_RotationCorrection = (Vector3)EditorGUILayout.Vector3Field(new GUIContent(" - Orientation: ", "Globally rotate the circular compass around its axis."), WC.CC_RotationCorrection);
        }
        else
        {
            WC.CC_ShowCompass = (bool)EditorGUILayout.Toggle(new GUIContent(" - Visible?", "If true, the circular compass will be visibile."), WC.CC_ShowCompass);
        }

        //****//

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("HORIZONTAL COMPASS:", EditorStyles.boldLabel);
        
        WC.HC_ShowHorCompass = (bool)EditorGUILayout.Toggle(new GUIContent("- Visible?", "If true, the horizontal compass will be visibile."), WC.HC_ShowHorCompass);

        if(WC.HC_ShowHorCompass)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("TEXTURES:", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            WC.HC_BackTexture = DLL_EditorExt.CustomGUI.CustomField("BACKGROUND:\n\n• Texture", WC.HC_BackTexture, 50, 300, TextAnchor.MiddleCenter);
            WC.HC_BackColor = DLL_EditorExt.CustomGUI.CustomField("\n\n• Color", WC.HC_BackColor, 50, 50, TextAnchor.MiddleCenter);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            WC.HC_CompTexture = DLL_EditorExt.CustomGUI.CustomField("CARDINAL:\n\n• Texture", WC.HC_CompTexture, 50, 300, TextAnchor.MiddleCenter);
            WC.HC_CompColor = DLL_EditorExt.CustomGUI.CustomField("\n\n• Color", WC.HC_CompColor, 50, 50, TextAnchor.MiddleCenter);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            WC.HC_OverTexture = DLL_EditorExt.CustomGUI.CustomField("OVERLAY:\n\n• Texture", WC.HC_OverTexture, 50, 300, TextAnchor.MiddleCenter);
            WC.HC_OverColor = DLL_EditorExt.CustomGUI.CustomField("\n\n• Color", WC.HC_OverColor, 50, 50, TextAnchor.MiddleCenter);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("DIMENSIONS:", EditorStyles.boldLabel);

            WC.HC_AdaptToScreenWidth = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Adapt to screen?", "If it's true, the ratio between the horizontal compass length and the screen width doesn't change."), WC.HC_AdaptToScreenWidth);
            WC.HC_HeightFixed = (float)EditorGUILayout.FloatField(new GUIContent("├ Fixed height:", "Set the horizontal compass fixed height."), WC.HC_HeightFixed);

            if(WC.HC_AdaptToScreenWidth)
            {
                WC.HC_LengthPercent = (float)EditorGUILayout.Slider(new GUIContent("├ Relative width: ", "Set the horizontal compass width as a percentage of the screen width."), WC.HC_LengthPercent, 0.0f, 1.0f);
            }
            else
            {
                WC.HC_LengthFixed = (float)EditorGUILayout.FloatField(new GUIContent("├ Fixed width:", "Set the horizontal compass fixed width."), WC.HC_LengthFixed);
            }

            WC.HC_HorRelativeLenght = (float)EditorGUILayout.Slider(new GUIContent("└ Texture stretching: ", "If you reduce this value, the cardinal points texture will be stretched without modifying the compass width."), WC.HC_HorRelativeLenght, 0.0f, 1.0f);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("POSITION:", EditorStyles.boldLabel);

            WC.HC_VerticalPosition = (float)EditorGUILayout.Slider(new GUIContent(" - Vertical position from screen center: ", "Use this slider to move the horizontal compass vertically."), WC.HC_VerticalPosition, -1.0f, 1.0f);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ROTATION:", EditorStyles.boldLabel);

            if(WC.HC_IstantaneousMovement)
            {
                WC.HC_IstantaneousMovement = (bool)EditorGUILayout.Toggle(new GUIContent(" - Rotate instantaneously?", "If true, the horizontal compass will rotate instantaneously."), WC.HC_IstantaneousMovement);
            }
            else
            {
                WC.HC_IstantaneousMovement = (bool)EditorGUILayout.Toggle(new GUIContent("┌ Rotate instantaneously?", "If true, the horizontal compass will rotate instantaneously."), WC.HC_IstantaneousMovement);
                WC.HC_Speed = (float)EditorGUILayout.FloatField(new GUIContent("└ Rotation speed:", "Set the horizontal compass rotation speed."), WC.HC_Speed);
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("CORRECTIONS:", EditorStyles.boldLabel);

            WC.HC_BackRectRelHeight = (float)EditorGUILayout.Slider(new GUIContent("┌ Background relative height: ", "Change the vertical size of the background texture as a percentage of the cardinal one."), WC.HC_BackRectRelHeight, 0.0f, 2.0f);
            WC.HC_BackRectRelLenght = (float)EditorGUILayout.Slider(new GUIContent("└ Background relative width: ", "Change the horizontal size of the background texture as a percentage of the cardinal one."), WC.HC_BackRectRelLenght, 0.0f, 2.0f);
            
            EditorGUILayout.Space();

            WC.HC_OverRectRelHeight = (float)EditorGUILayout.Slider(new GUIContent("┌ Overlay relative height: ", "Change the vertical size of the overlay texture as a percentage of the cardinal one."), WC.HC_OverRectRelHeight, 0.0f, 2.0f);
            WC.HC_OverRectRelLenght = (float)EditorGUILayout.Slider(new GUIContent("└ Overlay relative width: ", "Change the horizontal size of the overlay texture as a percentage of the cardinal one."), WC.HC_OverRectRelLenght, 0.0f, 2.0f);

            EditorGUILayout.Space();

            WC.HC_RotationCorrection = (float)EditorGUILayout.Slider(new GUIContent("- North correction: ", "Use this slider to align the horizontal compass with the world cardinal points."), WC.HC_RotationCorrection, 0.0f, 1.0f);

        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("DEBUG:", EditorStyles.boldLabel);

        WC.DEBUG_ShowFullEditor = (bool)EditorGUILayout.Toggle(new GUIContent(" - Show raw editor?", "Show and access to full raw editor variables"), WC.DEBUG_ShowFullEditor);

        //****//

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
