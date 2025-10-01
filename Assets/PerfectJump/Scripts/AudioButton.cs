﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioButton : MonoBehaviour
{
    public bool efx;
    public Sprite musicOnSprite, musicOffSprite, efxOnSprite, efxOffSprite;
    public Image spriteButton;


    //set button sprite
    void Start()
    {
        SetButton();
    }

    void OnEnable()
    {
        StartCoroutine(RefreshNextFrame());
    }

    IEnumerator RefreshNextFrame()
    {
        yield return null;
        SetButton();
    }

    public void MusicButtonClicked()
    {
        AudioManager.Instance.MuteMusic();
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
        SetButton();
    }

    public void EfxButtonClicked()
    {
        AudioManager.Instance.MuteEfx();
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
        SetButton();
    }

    void SetButton()
    {
        if ((!AudioManager.Instance.IsMusicMute() && !efx) || (!AudioManager.Instance.IsEfxMute() && efx))
            if (efx)
                spriteButton.sprite = efxOnSprite;
            else
                spriteButton.sprite = musicOnSprite;
        else
            if (efx)
            spriteButton.sprite = efxOffSprite;
        else
            spriteButton.sprite = musicOffSprite;
    }
}
