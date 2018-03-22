using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _audioController : MonoBehaviour {

    private AudioSource fireAudio;
    private AudioSource waterAudio;
    private AudioSource walkAudio;

	// Use this for initialization
	void Start () {
        waterAudio = GameObject.Find("Water Audio").GetComponent<AudioSource>();
        fireAudio = GameObject.Find("Fire Audio").GetComponent<AudioSource>();
        walkAudio = GameObject.Find("Walk Audio").GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool FireAudio{ set{
            if (value && !fireAudio.isPlaying)
            {
                fireAudio.Play();
            }
            else if (!value && fireAudio.isPlaying)
            {
                fireAudio.Stop();
            }
        }
    }

    public bool WaterAudio{ set{
            if (value && !waterAudio.isPlaying)
            {
                waterAudio.Play();
            }
            else if (!value && waterAudio.isPlaying)
            {
                waterAudio.Stop();
            }
        }
    }

    public bool WalkAudio{ set{
            if (value && !walkAudio.isPlaying)
            {
                walkAudio.Play();
            }
            else if (!value && walkAudio.isPlaying)
            {
                walkAudio.Stop();
            }
        }
    }
}
