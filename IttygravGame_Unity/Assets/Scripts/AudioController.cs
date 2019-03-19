using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
    public float MusicVolume = 0.50f;
    
	// Use this for initialization
	void Start () {
        GameObject AS = GameObject.FindGameObjectWithTag("AudioSource");
        if (AS && AS != gameObject) Destroy(gameObject);
        else GameObject.DontDestroyOnLoad(gameObject);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        GetComponent<AudioSource>().volume = MusicVolume;
    }
    public void MuteVolume()
    {
        GetComponent<AudioSource>().mute = !GetComponent<AudioSource>().mute;
        
    }
}
