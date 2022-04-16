/*
   ___ ___  ___   ___ ___ ___  _   _ ___    _   _        
  | _ \ _ \/ _ \ / __| __|   \| | | | _ \  /_\ | |       
  |  _/   / (_) | (__| _|| |) | |_| |   / / _ \| |__     
  |_| |_|_\\___/ \___|___|___/ \___/|_|_\/_/ \_\____|    
                                                        
 ___ __  __   _   ___ ___ _  _   _ _____ ___ ___  _  _ 
|_ _|  \/  | /_\ / __|_ _| \| | /_\_   _|_ _/ _ \| \| |
 | || |\/| |/ _ \ (_ || || .` |/ _ \| |  | | (_) | .` |
|___|_|  |_/_/ \_\___|___|_|\_/_/ \_\_| |___\___/|_|\_|
                                                           

                                           DI DESTROYER

                                   Author: Matteo Lusso
                                                 © 2021

*/

/*

DI DESTROYER is a very simple script that checks if the target still exists.
If not, the GameObject in "DI Canvas" that contains the icons and text will be destroyed.

DIDestroyer is automatically added as a component by DYNAMIC ICON.

*/

using UnityEngine;

public class DIDestroyer : MonoBehaviour
{
    private DynamicIcons DI_Script;
    private DIWorldCompass WC_Script;

    private void LateUpdate()
    {
        // If the related instance of DynamicIcons is removed from the game, the reference will become null
        // and the icon will be destroyed.
        if(DI_Script == null && WC_Script == null)
        {
            Destroy(this.gameObject);
        }
    }
    public void SetDI(DynamicIcons DI_Input)
    {
        DI_Script = DI_Input; 
    }

    public void SetWC(DIWorldCompass WC_Input)
    {
        WC_Script = WC_Input; 
    }
}
