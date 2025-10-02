﻿using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager S_Instance = null;

    [Header ("Audio Sources")]
    public AudioSource EfxAudioSource;
    public AudioSource MusicAudioSource;

    [Header ("Background Music")]
    public AudioClip MenuMusicAudio;
    public AudioClip GameMusicAudio;

    [Header("Sound Effects")]
    public AudioClip ButtonClickAudio;
    public AudioClip JumpAudio;
    public AudioClip HitAudio;
    public AudioClip GameOverAudio;
    public AudioClip PerfectAudio;

    private bool _isMuteMusic;
    private bool _isMuteEfx;
    private AudioClip _lastRequestedMusicAudio;

    void Awake()
    {
        if (S_Instance == null)
            S_Instance = this;
        else if (S_Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        _isMuteMusic = PlayerPrefs.GetInt("MuteMusic") == 1;
        _isMuteEfx = PlayerPrefs.GetInt("MuteEfx") == 1;

        ApplyMuteStatesToAudioSources();
        PlayMusic(MenuMusicAudio);
    }

    public void PlayMusic(AudioClip clip)
    {
        _lastRequestedMusicAudio = clip;
        if (_isMuteMusic)
        {
            MusicAudioSource.clip = clip;
            MusicAudioSource.Pause();
            return;
        }

        MusicAudioSource.clip = clip;
        if (!MusicAudioSource.isPlaying)
            MusicAudioSource.Play();
    }

    private void StopMusic()
    {
        MusicAudioSource.Stop();
    }

    public void PlayEffectsAudio(AudioClip clip)
    {
        if (_isMuteEfx)
            return;

        EfxAudioSource.PlayOneShot(clip);
    }

    public void MuteMusic()
    {
        if (_isMuteMusic)
        {
            _isMuteMusic = false;
            ApplyMuteStatesToAudioSources();
            // resume last requested track or fallback to menu
            PlayMusic(_lastRequestedMusicAudio != null ? _lastRequestedMusicAudio : MenuMusicAudio);
            PlayerPrefs.SetInt("MuteMusic", 0);
        }
        else
        {
            _isMuteMusic = true;
            ApplyMuteStatesToAudioSources();
            MusicAudioSource.Pause();
            PlayerPrefs.SetInt("MuteMusic", 1);
        }
    }

    public void MuteEfx()
    {
        if (_isMuteEfx)
            PlayerPrefs.SetInt("MuteEfx", 0);
        else
            PlayerPrefs.SetInt("MuteEfx", 1);

        _isMuteEfx = !_isMuteEfx;
        ApplyMuteStatesToAudioSources();
    }

    public bool IsMusicMuted()
    {
        return _isMuteMusic;
    }

    public bool IsEfxMuted()
    {
        return _isMuteEfx;
    }

    private void ApplyMuteStatesToAudioSources()
    {
        if (MusicAudioSource != null)
            MusicAudioSource.mute = _isMuteMusic;
        if (EfxAudioSource != null)
            EfxAudioSource.mute = _isMuteEfx;
    }
}
