using System;
using System.Collections.Generic;
using UnityEngine;



public class AudioMng : MonoBehaviour
{
    private Dictionary<string,AudioClip> audioCache = new Dictionary<string, AudioClip>();

    public AudioSource AudioSource;

    private string currentMusicName = "";

    private bool musicPaused;

    public bool MusicOn
    {
        get { return _musicOn; }
        set
        {
            _musicOn = value;

            if (!_musicOn)
            {
                PauseMusic();
            }
            else
            {
               ResumeMusic();
            }
        }
    }

    private bool _musicOn = true;

	public bool SoundOn{
		get{ return _soundOn;}
		set{ _soundOn = value;}
	}

	private bool _soundOn;

    void Awake()
    {
        AudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayMusic(string musicPath, float volume = 0.5f, bool loop = true)
    {
        if (!_musicOn)
        {
            return;
        }

        if (!currentMusicName.Equals(musicPath))
        {
            AudioClip clip = LoadAudioClip(musicPath);
            AudioSource.clip = clip;
            AudioSource.volume = volume;
            AudioSource.loop = loop;

            currentMusicName = musicPath;

            AudioSource.Play();
        }
    }

    public void PauseMusic()
    {
        if (AudioSource.isPlaying)
        {
            musicPaused = true;
            AudioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (musicPaused)
        {
            musicPaused = false;
            AudioSource.UnPause();
        }
    }

    public void PlaySound(string soundPath,float volume = 0.5f)
    {
		if (!_soundOn)
        {
            return;
        }

        AudioClip clip = LoadAudioClip(soundPath);

//        GameObject go = new GameObject();
//        go.name = "Audio:" + soundPath;
//
//        UnityEngine.AudioSource source = go.AddComponent<AudioSource>();
//        source.PlayOneShot(clip);

        AudioSource.PlayOneShot(clip);
    }

    public AudioClip LoadAudioClip(string path)
    {
        AudioClip clip = null;
        if (audioCache.ContainsKey(path))
        {
            clip = audioCache[path];
        }
        else
        {
            clip = Resources.Load<AudioClip>(path);
            if (clip == null)
            {
                Debug.LogError("找不到路径" + path + "的音效");
                return clip;
            }
            audioCache.Add(path,clip);
        }
        return clip;
    }
}
