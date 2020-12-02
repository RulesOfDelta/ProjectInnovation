using System.Collections;
using FMODUnity;
using UnityEngine;

public class Introduction : MonoBehaviour
{
    private InputHandler inputHandler;
    [FMODUnity.EventRef, SerializeField] private string[] voiceLines;
    private FMOD.Studio.EventInstance[] voiceLinesInstances;
    
    void Start()
    {
        inputHandler = GameObject.FindWithTag("InputHandler").GetComponent<InputHandler>();
        
        voiceLinesInstances = new FMOD.Studio.EventInstance[7];

        for (int i = 0; i < voiceLines.Length; i++)
        {
            voiceLinesInstances[i] = FMODUnity.RuntimeManager.CreateInstance(voiceLines[i]);
        }
        StartCoroutine(IntroductionCoroutine());
    }

    private void Update()
    {
        foreach (var voiceLine in voiceLinesInstances)
        {
            voiceLine.set3DAttributes(transform.position.To3DAttributes());
        }
    }

    IEnumerator IntroductionCoroutine()
    {
        yield return StartCoroutine(StartAndWaitForVoiceline(0));
        StartCoroutine(CheckForMovement());
    }

    IEnumerator CheckForMovement()
    {
        Debug.Log("Exec after voiceline");
        yield return new WaitUntil(() => inputHandler.Moving);
        StartCoroutine(CheckForWallBump());
    }

    IEnumerator CheckForWallBump()
    {
        yield return new WaitUntil(() => GameObject.FindWithTag("Player").GetComponentInChildren<PlayerWallSound>().bumpedIntoWall);
        yield return new WaitForSeconds(4.0f);
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        GameObject.Find("Room").GetComponent<EnemySpawner>().SpawnEnemies(1, new Vector2(2,2));
        yield return null;
    }

    IEnumerator StartAndWaitForVoiceline(int voiceLineIndex)
    {
        voiceLinesInstances[voiceLineIndex].start();
        FMOD.Studio.PLAYBACK_STATE playbackState;
        voiceLinesInstances[voiceLineIndex].getPlaybackState(out playbackState);
        while (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            voiceLinesInstances[voiceLineIndex].getPlaybackState(out playbackState);
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(5.0f);
    }
}
