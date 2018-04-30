using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _meltController : MonoBehaviour {

    private BoxCollider theCollider;
    private Animator animator;
    private float speed = 0f;
    private float maxSpeed = 1;
    private float increase = 0.01f;
    private float heating = -.5f;
    private Vector3 colliderSize;
    private float ySize;

    // Use this for initialization
    void Start () {
        theCollider = GetComponent<BoxCollider>();
        animator = transform.GetComponentInChildren<Animator>();
        animator.speed = speed;
        colliderSize = theCollider.size;
        ySize = colliderSize.y;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Time.time - heating < .5f)
        {
            speed = Mathf.Lerp(speed, maxSpeed, increase);
        }
        else
        {
            speed = Mathf.Lerp(speed, 0, increase * 5);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > .7f)
        {
            theCollider.enabled = false;
            speed = 1;
        }
        if (theCollider.enabled)
        {
            colliderSize.y = ySize * (1 - animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            theCollider.size = colliderSize;
        }
        animator.speed = speed;
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.name == "Fire")
        {
            heating = Time.time;
            maxSpeed = .5f;
        }
        if (other.name == "Steam")
        {
            heating = Time.time;
            maxSpeed = 1;
        }
    }
}
