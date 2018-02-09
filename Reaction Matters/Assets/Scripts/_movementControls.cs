using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _movementControls : MonoBehaviour {

    Vector3 start_position;
    Rigidbody rb;

    private float movementSpeed;    

    private float sensitivityX;
    private float sensitivityY;

    private bool lockMovement = true;

    private float minimumX = -360F;
    private float maximumX = 360F;

    private float minimumY = -90F;
    private float maximumY = 60F;

    float rotationX = 0F;
    float rotationY = 0F;
    Quaternion originalRotation;

    GameObject player;

    // Use this for initialization
    void Start () {
        movementSpeed = GameObject.Find("Level Settings").GetComponent<_gameSettings>().movementSpeed;
        sensitivityX = GameObject.Find("Level Settings").GetComponent<_gameSettings>().sensitivity;
        sensitivityY = GameObject.Find("Level Settings").GetComponent<_gameSettings>().sensitivity;


        player = this.transform.parent.gameObject;

        originalRotation = transform.localRotation;

        start_position = this.transform.position;
        this.
        rb = player.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
	}
	
	// Update is called once per frame
	void Update () {

        Mouse_Input();
        Keyboard_Input();
    }

    private void Mouse_Input()
    {
        if (lockMovement)
            return;

        float rControllerX = Math.Abs(Input.GetAxis("RightJoystickHorizontal")) > 0.05 ? Input.GetAxis("RightJoystickHorizontal") : 0;
        float rControllerY = Math.Abs(Input.GetAxis("RightJoystickVertical")) > 0.05 ? Input.GetAxis("RightJoystickVertical") : 0;

        // Read the mouse input axis
        rotationX += (Input.GetAxis("Mouse X") + rControllerX) * sensitivityX;
        rotationY += (Input.GetAxis("Mouse Y") - rControllerY) * sensitivityY;

        //Debug.Log(Input.GetAxis("RightJoystickHorizontal"));

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

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //movements Keyboard
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 lookDir = this.transform.forward;
            lookDir.y = start_position.y;
            rb.AddForce(lookDir * movementSpeed, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 lookDir = this.transform.forward;
            lookDir.y = start_position.y;
            rb.AddForce(-lookDir * movementSpeed, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 lookDir = this.transform.right;
            lookDir.y = start_position.y;
            rb.AddForce(-lookDir * movementSpeed, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 lookDir = this.transform.right;
            lookDir.y = start_position.y;
            rb.AddForce(lookDir * movementSpeed, ForceMode.Impulse);
        }

        // left joystick movement
        if (Math.Abs(Input.GetAxis("LeftJoystickVertical")) > 0)
        {
            Vector3 lookDir = this.transform.forward;
            lookDir.y = start_position.y;
            rb.AddForce(lookDir * movementSpeed * (-Input.GetAxis("LeftJoystickVertical")), ForceMode.Impulse);
        }

        if (Math.Abs(Input.GetAxis("LeftJoystickHorizontal")) > 0)
        {
            Vector3 lookDir = this.transform.right;
            lookDir.y = start_position.y;
            rb.AddForce(lookDir * movementSpeed * Input.GetAxis("LeftJoystickHorizontal"), ForceMode.Impulse);
        }
    }

    public bool LockMovement { set { lockMovement = value; } }
}
