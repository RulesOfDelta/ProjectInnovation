using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadAfterClip : MonoBehaviour
{
    [EventRef, SerializeField] private string clip;
    private EventInstance clipInstance;
    [SerializeField] private string scene;
    
    // Start is called before the first frame update
    void Start()
    {
        clipInstance = clip.CreateSound();
        clipInstance.start();
    }

    private void OnDestroy()
    {
        clipInstance.release();
    }

    // Update is called once per frame
    void Update()
    {
        clipInstance.getPlaybackState(out var state);
        if (state != PLAYBACK_STATE.STOPPED)
            return;
        SceneManager.LoadScene(scene);
    }
}
