using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    public GameCore gameCore;

    //----//

    public Button button_NewLevel;
    public Button button_Menu;

    //----//

    public Toggle toggle_SeedSettings;

    //----//

    public GameObject parent_Menu;

    //----//

    public Dropdown drop_MountainSettings;
    public Dropdown drop_ForestSettings;
    public Dropdown drop_LakeSettings;
    public Dropdown drop_ErosionSettings;
    public Dropdown drop_RiverErosionSettings;
    public Dropdown drop_CitySettings;

    //----//

    public Scrollbar scroll_RiverSettings;

    //----//

    public TouchController touch_Controller;

    //----//

    void Update()
    {
        gameCore.world_RandomSeed = toggle_SeedSettings.isOn;

        gameCore.mountains_Size = (Enum.Size)drop_MountainSettings.value;
        gameCore.forests_Size = (Enum.Size)drop_ForestSettings.value;
        gameCore.lakes_Size = (Enum.Size)drop_LakeSettings.value;
        gameCore.cities_Size = (Enum.Size)drop_CitySettings.value;

        gameCore.erosion_GlobalForce = (Enum.ErosionForce)drop_ErosionSettings.value;
        gameCore.erosion_River = (Enum.ErosionForce)drop_RiverErosionSettings.value;

        gameCore.rivers_Straightness = scroll_RiverSettings.value * 100.0f;
        if(gameCore.rivers_Straightness <= 0.0f)
        {
            gameCore.rivers_Straightness = 1.0f;
        }

        if(parent_Menu.activeSelf)
        {
            touch_Controller.SetTouchState(false);
        }
        else
        {
            touch_Controller.SetTouchState(true);
        }

    }

    //----//

    private void ShowHideMenu()
    {
        if(parent_Menu.activeSelf)
        {
            parent_Menu.SetActive(false);
        }
        else
        {
            parent_Menu.SetActive(true);
        }
    }

    //----//

    public bool IsMenuVisible()
    {
        if(parent_Menu.activeSelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
