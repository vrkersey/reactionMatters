using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _movementControls : MonoBehaviour {

    private Vector3 start_position;
    private Rigidbody rb;
    private float movementSpeed;    
    private float sensitivityX;
    private float sensitivityY;
    private float jumpMultiplier;
    private float minimumX = -360F;
    private float maximumX = 360F;
    private float minimumY = -90F;
    private float maximumY = 60F;
    private float rotationX = 0F;
    private float rotationY = 0F;
    private Quaternion originalRotation;
    private GameObject player;
    private bool audioStopped;
    private _gameSettings GM;
    private _audioController AM;

    // Use this for initialization
    void Start () {
        movementSpeed = GameObject.Find("_EventSystem").GetComponent<_gameSettings>().movementSpeed;
        sensitivityX = GameObject.Find("_EventSystem").GetComponent<_gameSettings>().sensitivity;
        sensitivityY = GameObject.Find("_EventSystem").GetComponent<_gameSettings>().sensitivity;
        jumpMultiplier = GameObject.Find("_EventSystem").GetComponent<_gameSettings>().jumpHeight;
        
        GM = GameObject.Find("_EventSystem").GetComponent<_gameSettings>();
        AM = GameObject.Find("_EventSystem").GetComponent<_audioController>();
        
        player = this.transform.parent.gameObject;

        originalRotation = transform.localRotation;

        rb = player.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (GM.isPaused || GM.isCrafting)
            return;
        Mouse_Input();
        Keyboard_Input();
    }

    private void Mouse_Input()
    {
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
        Vector3 velocity = rb.velocity;
        velocity.x = 0;
        velocity.z = 0;
        rb.velocity = velocity;

        //movements Keyboard
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 lookDir = this.transform.forward;
            lookDir.y = 0;
            rb.AddForce(lookDir * movementSpeed, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 lookDir = this.transform.forward;
            lookDir.y = 0;
            rb.AddForce(-lookDir * movementSpeed, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 lookDir = this.transform.right;
            lookDir.y = 0;
            rb.AddForce(-lookDir * movementSpeed, ForceMode.Impulse);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 lookDir = this.transform.right;
            lookDir.y = 0;
            rb.AddForce(lookDir * movementSpeed, ForceMode.Impulse);
        }
        

        // left joystick movement
        if (Math.Abs(Input.GetAxis("LeftJoystickVertical")) > 0)
        {
            Vector3 lookDir = this.transform.forward;
            lookDir.y = 0;
            rb.AddForce(lookDir * movementSpeed * (Input.GetAxis("LeftJoystickVertical")), ForceMode.Impulse);
        }

        if (Math.Abs(Input.GetAxis("LeftJoystickHorizontal")) > 0)
        {
            Vector3 lookDir = this.transform.right;
            lookDir.y = 0;
            rb.AddForce(lookDir * movementSpeed * Input.GetAxis("LeftJoystickHorizontal"), ForceMode.Impulse);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.W) ||
            Math.Abs(Input.GetAxis("LeftJoystickVertical")) > 0 || Math.Abs(Input.GetAxis("LeftJoystickHorizontal")) > 0)
        {
            AM.WalkAudio = true;
        }
        else
        {
            AM.WalkAudio = false;
            //StartCoroutine(FadeOut(walkAudio, .3f));
        }
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
