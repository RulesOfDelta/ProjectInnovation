using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    [FMODUnity.EventRef, SerializeField] private string restMusicPath;
    private FMOD.Studio.EventInstance restInstance;

    [FMODUnity.EventRef, SerializeField] private string fightMusicPath;
    private FMOD.Studio.EventInstance fightInstance;

    [SerializeField, Range(0f, 1f)] private float battleVolume;

    public enum MusicState
    {
        Rest,
        Fight,
        Stop
    }

    private void Awake()
    {
        restInstance = restMusicPath.CreateSound();
        restInstance.setParameterByName("RestVolume", 1f);
        
        fightInstance = fightMusicPath.CreateSound();
        fightInstance.setParameterByName("BattleMusic", 1);
        fightInstance.setParameterByName("BattleVolume", battleVolume);
        fightInstance.setParameterByName("BattleIntensity", 0);

        State = MusicState.Fight;
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
        public float healthLevel;
        public int intensity;
    }

    [SerializeField] private List<IntensityLevel> intensityLevels;

    private void SwitchMusic(MusicState newState)
    {
        switch (newState)
        {
            case MusicState.Rest:
                restInstance.start();
                fightInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;
            case MusicState.Fight:
                restInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                fightInstance.start();
                break;
            case MusicState.Stop:
                restInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                fightInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
        foreach (var level in intensityLevels)
        {
            if (!(newHealth < level.healthLevel)) continue;
            fightInstance.setParameterByName("BattleIntensity", level.intensity);
            break;
        }
    }
}
