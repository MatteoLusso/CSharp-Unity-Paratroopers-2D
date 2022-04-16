using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BorderIcon : MonoBehaviour
{
    public RawImage icon_Sprite;

    [Space]

    public Vector2 icon_BorderCorrections;
    
    [Range(0.0f, 360.0f)]
    public float icon_AngleRotationCorrection;

    public float smooth_IconRotation;

    //----//

    private Vector2 icon_BorderLimits;

    //----//

    private float angle_Alpha = 0.0f;
    private float angle_Beta = 0.0f;
    private float angle_Gamma = 0.0f;

    //----//

    private Vector2 screen_Resolution = -Vector2.one;

    //----//

    private bool object_Visibile;

    //----//

    void LateUpdate()
    {
        //icon_BorderLimits = new Vector2(screen_Resolution.x * icon_DistanceFromBorder, screen_Resolution.y * icon_DistanceFromBorder);

        if(screen_Resolution.x != Screen.width || screen_Resolution.y != Screen.height)
        {
            screen_Resolution = new Vector2(Screen.width, Screen.height);
            angle_Alpha = Mathf.Atan2(Screen.height, Screen.width) * Mathf.Rad2Deg;
        }

        icon_BorderLimits = icon_BorderCorrections;

        if(!object_Visibile)
        {
            icon_Sprite.gameObject.SetActive(true);

            Vector2 icon_SideCoord = Vector2.zero;

            angle_Gamma = Vector3.SignedAngle(Vector3.right, Camera.main.transform.right, Vector3.forward);

            Vector3 dir_CameraObject = new Vector3(this.transform.position.x, this.transform.position.y, 0.0f) - new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f);
            angle_Beta = Vector3.SignedAngle(Vector3.right, dir_CameraObject, Vector3.forward);
            angle_Beta -= angle_Gamma;
            angle_Beta = Utilities.Angles.SignedAngleTo360Angle(angle_Beta);

            float UI_Det12;
            Vector2 UI_IconMinPos;
            Vector2 UI_IconMaxPos;
            Vector3 UI_CoeffLine1;
            Vector3 UI_CoeffLine2;
            Vector2 UI_IntersectionCoord = Vector2.zero;

            float UI_SidePercent;

            switch(Utilities.Angles.IconSide(angle_Beta, angle_Alpha))
            {
                case Utilities.Screen.Side.Right:   Debug.Log("L'oggetto è a destra");

                                                    UI_IconMinPos = Camera.main.ScreenToWorldPoint(new Vector2(screen_Resolution.x, 0.0f));
                                                    UI_IconMaxPos = Camera.main.ScreenToWorldPoint(new Vector2(screen_Resolution.x, screen_Resolution.y));

                                                    UI_CoeffLine1 = Utilities.Algebra.LinePassingThroughTwoPoints(UI_IconMinPos, UI_IconMaxPos);
                                                    UI_CoeffLine2 = Utilities.Algebra.LinePassingThroughTwoPoints(Camera.main.transform.position, this.transform.position);

                                                    UI_Det12 = Utilities.Algebra.Determinant(UI_CoeffLine1, UI_CoeffLine2);

                                                    if(UI_Det12 != 0.0f)
                                                    {
                                                        UI_IntersectionCoord = Utilities.Algebra.IntersectionPointBetweenTwoLines(UI_CoeffLine1, UI_CoeffLine2, UI_Det12);
                                                    }

                                                    UI_SidePercent = (UI_IntersectionCoord - UI_IconMinPos).magnitude / (UI_IconMaxPos - UI_IconMinPos).magnitude;

                                                    icon_SideCoord = new Vector2(screen_Resolution.x - icon_BorderLimits.x, icon_BorderLimits.y + (UI_SidePercent * (screen_Resolution.y - (2 * icon_BorderLimits.y))));

                                                    Debug.DrawLine(UI_IconMinPos, UI_IconMaxPos, Color.red);
                                                    Debug.DrawLine(Camera.main.transform.position, this.transform.position, Color.blue);

                                                    break;

                case Utilities.Screen.Side.Up:      //Debug.Log("L'oggetto è sopra");

                                                    UI_IconMinPos = Camera.main.ScreenToWorldPoint(new Vector2(0.0f, screen_Resolution.y));
                                                    UI_IconMaxPos = Camera.main.ScreenToWorldPoint(new Vector2(screen_Resolution.x, screen_Resolution.y));

                                                    UI_CoeffLine1 = Utilities.Algebra.LinePassingThroughTwoPoints(UI_IconMinPos, UI_IconMaxPos);
                                                    UI_CoeffLine2 = Utilities.Algebra.LinePassingThroughTwoPoints(Camera.main.transform.position, this.transform.position);

                                                    UI_Det12 = Utilities.Algebra.Determinant(UI_CoeffLine1, UI_CoeffLine2);

                                                    if(UI_Det12 != 0.0f)
                                                    {
                                                        UI_IntersectionCoord = Utilities.Algebra.IntersectionPointBetweenTwoLines(UI_CoeffLine1, UI_CoeffLine2, UI_Det12);
                                                    }

                                                    UI_SidePercent = (UI_IntersectionCoord - UI_IconMinPos).magnitude / (UI_IconMaxPos - UI_IconMinPos).magnitude;

                                                    icon_SideCoord = new Vector2(icon_BorderLimits.x + (UI_SidePercent * (screen_Resolution.x - (2 * icon_BorderLimits.x))), screen_Resolution.y - icon_BorderLimits.y);
                                                    //icon_SideCoord = new Vector2(300 + UI_SidePercent * 200.0f, 300);

                                                    Debug.Log(icon_SideCoord);

                                                    Debug.DrawLine(UI_IconMinPos, UI_IconMaxPos, Color.red);
                                                    Debug.DrawLine(Camera.main.transform.position, this.transform.position, Color.blue);

                                                    break;

                case Utilities.Screen.Side.Left:    Debug.Log("L'oggetto è a sinistra");
                                                    
                                                    UI_IconMinPos = Camera.main.ScreenToWorldPoint(new Vector2(0.0f, 0.0f));
                                                    UI_IconMaxPos = Camera.main.ScreenToWorldPoint(new Vector2(0.0f, screen_Resolution.y));

                                                    UI_CoeffLine1 = Utilities.Algebra.LinePassingThroughTwoPoints(UI_IconMinPos, UI_IconMaxPos);
                                                    UI_CoeffLine2 = Utilities.Algebra.LinePassingThroughTwoPoints(Camera.main.transform.position, this.transform.position);

                                                    UI_Det12 = Utilities.Algebra.Determinant(UI_CoeffLine1, UI_CoeffLine2);

                                                    if(UI_Det12 != 0.0f)
                                                    {
                                                        UI_IntersectionCoord = Utilities.Algebra.IntersectionPointBetweenTwoLines(UI_CoeffLine1, UI_CoeffLine2, UI_Det12);
                                                    }

                                                    UI_SidePercent = (UI_IntersectionCoord - UI_IconMinPos).magnitude / (UI_IconMaxPos - UI_IconMinPos).magnitude;

                                                    icon_SideCoord = new Vector2(icon_BorderLimits.x, icon_BorderLimits.y + (UI_SidePercent * (screen_Resolution.y - (2 * icon_BorderLimits.y))));

                                                    Debug.DrawLine(UI_IconMinPos, UI_IconMaxPos, Color.red);
                                                    Debug.DrawLine(Camera.main.transform.position, this.transform.position, Color.blue);

                                                    break;

                case Utilities.Screen.Side.Down:    Debug.Log("L'oggetto è sotto");

                                                    UI_IconMinPos = Camera.main.ScreenToWorldPoint(new Vector2(0.0f, 0.0f));
                                                    UI_IconMaxPos = Camera.main.ScreenToWorldPoint(new Vector2(screen_Resolution.x, 0.0f));

                                                    UI_CoeffLine1 = Utilities.Algebra.LinePassingThroughTwoPoints(UI_IconMinPos, UI_IconMaxPos);
                                                    UI_CoeffLine2 = Utilities.Algebra.LinePassingThroughTwoPoints(Camera.main.transform.position, this.transform.position);

                                                    UI_Det12 = Utilities.Algebra.Determinant(UI_CoeffLine1, UI_CoeffLine2);

                                                    if(UI_Det12 != 0.0f)
                                                    {
                                                        UI_IntersectionCoord = Utilities.Algebra.IntersectionPointBetweenTwoLines(UI_CoeffLine1, UI_CoeffLine2, UI_Det12);
                                                    }

                                                    UI_SidePercent = (UI_IntersectionCoord - UI_IconMinPos).magnitude / (UI_IconMaxPos - UI_IconMinPos).magnitude;

                                                    icon_SideCoord = new Vector2(icon_BorderLimits.x + (UI_SidePercent * (screen_Resolution.x - (2 * icon_BorderLimits.x))), icon_BorderLimits.y);

                                                    Debug.DrawLine(UI_IconMinPos, UI_IconMaxPos, Color.red);
                                                    Debug.DrawLine(Camera.main.transform.position, this.transform.position, Color.blue);

                                                    break;

                default:                            break;                                                                                                            
            }

            icon_Sprite.transform.position = icon_SideCoord;
            icon_Sprite.GetComponent<RectTransform>().rotation = Quaternion.Lerp(icon_Sprite.GetComponent<RectTransform>().rotation, Quaternion.Euler(0.0f, 0.0f, angle_Beta + icon_AngleRotationCorrection), smooth_IconRotation * Time.deltaTime);
        }
        else
        {
            icon_Sprite.gameObject.SetActive(false);
        }
    }

    void OnBecameInvisible()
    {
        object_Visibile = false;
    }

    void OnBecameVisible()
    {
        object_Visibile = true;
    }
}
