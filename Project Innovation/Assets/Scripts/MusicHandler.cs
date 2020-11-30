﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    [FMODUnity.EventRef, SerializeField] private string restMusicPath;
    private FMOD.Studio.EventInstance restInstance;

    [FMODUnity.EventRef, SerializeField] private string fightMusicPath;
    private FMOD.Studio.EventInstance fightInstance;

    public enum MusicState
    {
        Rest,
        Fight
    }

    private void Start()
    {
        restInstance = restMusicPath.CreateSound();
        restInstance.setParameterByName("RestVolume", 1f);
        fightInstance = fightMusicPath.CreateSound();
        fightInstance.setParameterByName("")
    }

    private MusicState musicState;

    public MusicState State
    {
        get => musicState;
        set
        {
            if (value == musicState) return;
            musicState = value;
            SwitchMusic(musicState);
        }
    }

    [Serializable]
    private struct IntensityLevel
    {
        public float HealthLevel;
        public int Intensity;
    }

    [SerializeField] private List<IntensityLevel> intensityLevels;

    private void SwitchMusic(MusicState newState)
    {
        switch (newState)
        {
            case MusicState.Rest:
                restInstance.start();
                break;
            case MusicState.Fight:
                restInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private void OnDestroy()
    {
        restInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        restInstance.release();
    }

    public void SetRestVal(float val)
    {
        if (State != MusicState.Rest) return;
        val = Mathf.Clamp01(val);
        restInstance.setParameterByName("OnPath", val);
        restInstance.setParameterByName("OffPath", 1f - val);
    }

    public void HealthCallback(float newHealth)
    {
        
    }
}
