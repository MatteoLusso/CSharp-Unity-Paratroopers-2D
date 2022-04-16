using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICompass : MonoBehaviour
{
    public RawImage compass_Background;
    public float smooth_Compass;

    void Update()
    {
        compass_Background.rectTransform.rotation = Quaternion.Lerp(compass_Background.rectTransform.rotation, Quaternion.Inverse(Camera.main.transform.rotation), smooth_Compass * Time.deltaTime);
    }
}
