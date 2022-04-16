using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float speed;
    public float rot_Stick;
    public float rot_Mouse;
    //public float smooth_Mouse;
    public float smooth_Axis;
    public float smooth_Button;

    private float input_W;
    private float input_S;
    private float input_A;
    private float input_D;
    private float input_Vertical1;
    private float input_Horizontal1;
    private float input_Vertical2;
    private float input_Horizontal2;
    private float input_LB;
    private float input_RB;
    //private float mouse_X = 0.0f;
    //private float mouse_Y = 0.0f;

    void Start()
    {
        input_Vertical1 = 0.0f;
        input_W = 0.0f;
        input_S = 0.0f;
        input_A = 0.0f;
        input_D = 0.0f;
        input_Horizontal1 = 0.0f;
        input_Vertical2 = 0.0f;
        input_Horizontal2 = 0.0f;
        input_LB = 0.0f;
        input_RB = 0.0f;
        //mouse_X = 0.0f;
        //mouse_Y = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Mouse X") != 0.0f)
        {
            this.transform.Rotate(new Vector3(0.0f, Input.GetAxis("Mouse X"), 0.0f) * rot_Mouse * Time.deltaTime);
        }
        /*else
        {
            mouse_X = Mathf.Lerp(mouse_X, 0.0f, smooth_Mouse * Time.deltaTime);
        }*/
        if(Input.GetAxis("Mouse Y") != 0.0f)
        {
            this.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0.0f, 0.0f) * rot_Mouse * Time.deltaTime);
        }
        /*else
        {
            mouse_Y = Mathf.Lerp(mouse_Y, 0.0f, smooth_Mouse * Time.deltaTime);
        }*/


        if(Input.GetKey(KeyCode.W))
        {
            input_W = Mathf.Lerp(input_W, 1.0f, smooth_Button * Time.deltaTime);
        }
        else
        {
            input_W = Mathf.Lerp(input_W, 0.0f, smooth_Button * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.S))
        {
            input_S = Mathf.Lerp(input_S, 1.0f, smooth_Button * Time.deltaTime);
        }
        else
        {
            input_S = Mathf.Lerp(input_S, 0.0f, smooth_Button * Time.deltaTime);
        }

        if(Mathf.Abs(Input.GetAxis("Vertical")) >= 0.15f)
        {
            input_Vertical1 = Mathf.Lerp(input_Vertical1, Input.GetAxis("Vertical"), smooth_Axis * Time.deltaTime);
        }
        else
        {
            input_Vertical1 = Mathf.Lerp(input_Vertical1, 0.0f, smooth_Axis * Time.deltaTime);
        }

        if(Mathf.Abs(Input.GetAxis("Horizontal")) >= 0.15f)
        {
            input_Horizontal1 = Mathf.Lerp(input_Horizontal1, Input.GetAxis("Horizontal"), smooth_Axis * Time.deltaTime);
        }
        else
        {
            input_Horizontal1 = Mathf.Lerp(input_Horizontal1, 0.0f, smooth_Axis * Time.deltaTime);
        }

        if(Mathf.Abs(Input.GetAxis("Vertical2")) >= 0.3f)
        {
            input_Vertical2 = Mathf.Lerp(input_Vertical2, Input.GetAxis("Vertical2"), smooth_Axis * Time.deltaTime);
        }
        else
        {
            input_Vertical2 = Mathf.Lerp(input_Vertical2, 0.0f, smooth_Axis * Time.deltaTime);
        }

        if(Mathf.Abs(Input.GetAxis("Horizontal2")) >= 0.8f)
        {
            input_Horizontal2 = Mathf.Lerp(input_Horizontal2, Input.GetAxis("Horizontal2"), smooth_Axis * Time.deltaTime);
        }
        else
        {
            input_Horizontal2 = Mathf.Lerp(input_Horizontal2, 0.0f, smooth_Axis * Time.deltaTime);
        }

        if(Input.GetButton("LB"))
        {
            input_LB = Mathf.Lerp(input_LB, 1.0f, smooth_Button * Time.deltaTime);
        }
        else
        {
            input_LB = Mathf.Lerp(input_LB, 0.0f, smooth_Button * Time.deltaTime);
        }

        if(Input.GetButton("RB"))
        {
            input_RB = Mathf.Lerp(input_RB, 1.0f, smooth_Button * Time.deltaTime);
        }
        else
        {
            input_RB = Mathf.Lerp(input_RB, 0.0f, smooth_Button * Time.deltaTime);
        }

        if(Input.GetKey(KeyCode.A))
        {
            input_A = Mathf.Lerp(input_A, 1.0f, smooth_Button * Time.deltaTime);
        }
        else
        {
            input_A = Mathf.Lerp(input_A, 0.0f, smooth_Button * Time.deltaTime);
        }

        if(Input.GetKey(KeyCode.D))
        {
            input_D = Mathf.Lerp(input_D, 1.0f, smooth_Button * Time.deltaTime);
        }
        else
        {
            input_D = Mathf.Lerp(input_D, 0.0f, smooth_Button * Time.deltaTime);
        }

        //----//

        this.transform.position += this.transform.forward * speed * (input_Vertical1 + (input_W - input_S)) * Time.deltaTime;

        this.transform.Rotate(new Vector3(-input_Vertical2, -(input_LB - input_RB), -input_Horizontal2 + (input_A - input_D)) * rot_Stick * Time.deltaTime);

        //this.transform.Rotate(new Vector3(0.0f, -(input_LB - input_RB), 0.0f) * rot * Time.deltaTime);
    }
}
