using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _audioController : MonoBehaviour {
    public AudioClip[] elements;
    public AudioClip[] voiceOvers;
    public AudioClip[] levelMusic;
    public AudioClip lastMinuteMusic;
    public AudioClip creditMusic;

    private Dictionary<string, AudioClip> elementsList = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> voiceOversList = new Dictionary<string, AudioClip>();

    private AudioSource fireAudio;
    private AudioSource waterAudio;
    private AudioSource walkAudio;
    private AudioSource narator;
    private AudioSource music;
    private int musicIndex = -1;
    private bool playingLastMinute = false;

    public bool StopMusic { get; set; }

	// Use this for initialization
	void Start () {
        waterAudio = GameObject.Find("Water Audio").GetComponent<AudioSource>();
        fireAudio = GameObject.Find("Fire Audio").GetComponent<AudioSource>();
        walkAudio = GameObject.Find("Walk Audio").GetComponent<AudioSource>();

        GameObject musicObject = GameObject.Find("_Music");
        music = musicObject == null ? null : musicObject.GetComponent<AudioSource>();

        narator = gameObject.GetComponent<AudioSource>();

        foreach (AudioClip a in elements)
            elementsList.Add(a.name, a);
        foreach (AudioClip a in voiceOvers)
            voiceOversList.Add(a.name, a);

        playVoice("System_Rebooting");
        playVoice("Tools_Activated");
        playVoice("Toxic_atmosphere_detected");
        playVoice("Location_System_Corrupted");
    }
    
    void Update () {
		if (music != null && !music.isPlaying && !StopMusic)
        {
            musicIndex = (musicIndex + 1) % levelMusic.Length;
            music.clip = levelMusic[musicIndex];
            music.Play();
        }
	}

    public void playLastMinute()
    {
        if (!playingLastMinute)
            StartCoroutine(fadePlay(lastMinuteMusic));
        playingLastMinute = true;
    }

    public void playCredits()
    {

    }

    public void playElement(string name)
    {
        AudioClip ac;
        if (elementsList.TryGetValue(name, out ac))
        {
            StartCoroutine(waitForSound(ac));
        }
    }

    public void playVoice(string name)
    {
        AudioClip ac;
        if (voiceOversList.TryGetValue(name, out ac))
        {
            StartCoroutine(waitForSound(ac));
        }
    }

    IEnumerator fadePlay(AudioClip clip)
    {
        float startVolume = music.volume;   
        while (music.volume > 0)
        {
            music.volume -= .001f;
            yield return new WaitForSeconds(.02f);
        }
        music.clip = clip;
        music.volume = startVolume;
        music.Play();
        yield return null;
    }

    IEnumerator waitForSound(AudioClip clip)
    {
        //Wait Until Sound has finished playing
        while (narator.isPlaying)
        {
            yield return new WaitForSeconds(1f);
        }

        narator.clip = clip;
        narator.Play();
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
