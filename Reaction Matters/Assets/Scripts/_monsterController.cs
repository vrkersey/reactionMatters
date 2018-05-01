using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _monsterController : MonoBehaviour {
    public AudioClip[] monsterSounds;

    private AudioSource audioSource;
    private AudioSource music;
    private float lastSound;
    private int index = 0;
	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        music = GameObject.Find("_Music").GetComponent<AudioSource>();
        lastSound = Time.fixedTime;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float rand = UnityEngine.Random.Range(0, 10000);
        //Debug.Log(rand);
        if (rand <= 1f && lastSound + 120 < Time.fixedTime) {
            Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            transform.position = playerPos;
            StartCoroutine(playSound(monsterSounds[index]));
            lastSound = Time.fixedTime;
            index = (index + 1) % monsterSounds.Length;
        }
    }

    private IEnumerator playSound(AudioClip audioClip)
    {
        Debug.Log("Starting monster audio");
        //fade out music
        float startVolume = music.volume;
        while(music.volume > startVolume / 4)
        {
            music.volume = Mathf.Lerp(music.volume, 0, startVolume / 8);
            yield return new WaitForSeconds(.1f);
        }

        //play monster
        audioSource.clip = audioClip;
        audioSource.Play();
        yield return new WaitForSeconds(audioClip.length);

        //fade up music
        while (music.volume < startVolume)
        {
            music.volume = Mathf.Lerp(music.volume, startVolume + .1f, startVolume / 8);
            yield return new WaitForSeconds(.1f);
        }
        Debug.Log("Ending monster audio");
    }
}
