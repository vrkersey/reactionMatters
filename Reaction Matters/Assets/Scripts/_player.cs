using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _player : MonoBehaviour {

    Vector3 start_position;
    Rigidbody rb;


    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    private float minimumX = -360F;
    private float maximumX = 360F;

    private float minimumY = -60F;
    private float maximumY = 60F;

    float rotationX = 0F;
    float rotationY = 0F;
    Quaternion originalRotation;



    // Use this for initialization
    void Start () {
        originalRotation = transform.localRotation;

        start_position = this.transform.position;
        rb = this.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
	}
	
	// Update is called once per frame
	void Update () {

        Mouse_Input();
        Keyboard_Input();
    }

    private void Mouse_Input()
    {
        // Read the mouse input axis
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

        rotationX = ClampAngle(rotationX, minimumX, maximumX);
        rotationY = ClampAngle(rotationY, minimumY, maximumY);

        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

        transform.localRotation = originalRotation * xQuaternion * yQuaternion;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle <= -360F)
            angle += 360F;
        if (angle >= 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    private void Keyboard_Input()
    {

        //movements
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 lookDir = this.transform.forward;
            lookDir.y = 0f;
            rb.AddForce(lookDir, ForceMode.Impulse);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Vector3 lookDir = this.transform.forward;
            lookDir.y = 0f;
            rb.AddForce(-lookDir, ForceMode.Impulse);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Vector3 lookDir = this.transform.right;
            lookDir.y = 0f;
            rb.AddForce(-lookDir, ForceMode.Impulse);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Vector3 lookDir = this.transform.right;
            lookDir.y = 0f;
            rb.AddForce(lookDir, ForceMode.Impulse);
        }
        else
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
