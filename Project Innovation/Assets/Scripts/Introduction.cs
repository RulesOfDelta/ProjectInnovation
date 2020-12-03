using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Introduction : MonoBehaviour
{
    private InputHandler inputHandler;
    [FMODUnity.EventRef, SerializeField] private string[] voiceLines;
    private FMOD.Studio.EventInstance[] voiceLinesInstances;

    [SerializeField] private string nextScene;

    [SerializeField] private Transform targetPosition;
    [SerializeField] private float speed;

    private Transform spider;
    [SerializeField] private float spiderDist = 8f;

    void Start()
    {
        inputHandler = GameObject.FindWithTag("InputHandler").GetComponent<InputHandler>();
        
        voiceLinesInstances = new FMOD.Studio.EventInstance[voiceLines.Length];

        for (int i = 0; i < voiceLines.Length; i++)
        {
            voiceLinesInstances[i] = FMODUnity.RuntimeManager.CreateInstance(voiceLines[i]);
        }
        
        spider = GameObject.Find("Spider").transform;
        
        StartCoroutine(IntroductionCoroutine());
    }

    private void Update()
    {
        for (int i = 0; i < voiceLinesInstances.Length; i++)
        {
            // :D
            if (i == 2) voiceLinesInstances[i].set3DAttributes(spider.position.To3DAttributes());
            else voiceLinesInstances[i].set3DAttributes(transform.position.To3DAttributes());
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition.position, speed * Time.deltaTime);
    }

    IEnumerator IntroductionCoroutine()
    {
        yield return null;
        yield return StartCoroutine(StartAndWaitForVoiceline(0));
        StartCoroutine(CheckForMovement());
    }

    IEnumerator CheckForMovement()
    {
        yield return new WaitUntil(() => inputHandler.Moving);
        StartCoroutine(CheckForWallBump());
    }

    IEnumerator CheckForWallBump()
    {
        bool doStuff = true;
        var wallSound = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerWallSound>();
        while (doStuff)
        {
            if (wallSound.bumpedIntoWall)
            {
                yield return StartCoroutine(StartAndWaitForVoiceline(1));
                doStuff = false;
            }
            else if ((spider.position - targetPosition.position).magnitude < spiderDist)
            {
                yield return StartCoroutine(StartAndWaitForVoiceline(2));
                StartCoroutine(SpawnEnemy());
                yield break;
            }
            yield return null;
        }
        yield return new WaitUntil(() => (spider.position - targetPosition.position).magnitude < spiderDist);
        yield return StartCoroutine(StartAndWaitForVoiceline(2));
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        //Enemy 1
        List<GameObject> enemy = GameObject.Find("Room").GetComponent<EnemySpawner>().SpawnEnemies(2, new Vector2(2,2));
        Debug.Log(enemy.Count);
        enemy.ElementAt(1).SetActive(false);
        enemy.ElementAt(0).GetComponent<EnemyBehaviour>().isPassive = true;
        yield return StartCoroutine(StartAndWaitForVoiceline(3));
        enemy.ElementAt(0).GetComponent<EnemyBehaviour>().isPassive = false;
        yield return new WaitUntil(() => GameObject.FindWithTag("Enemy") == null);

        yield return StartCoroutine(StartAndWaitForVoiceline(4));
        //Enemy 2
        // TODO you are able to die here
        enemy.ElementAt(0).SetActive(true);
        enemy.ElementAt(0).GetComponent<EnemyBehaviour>().isPassive = true;
        
        yield return StartCoroutine(StartAndWaitForVoiceline(5));
        yield return StartCoroutine(StartAndWaitForVoiceline(6));
        enemy.ElementAt(0).GetComponent<EnemyBehaviour>().isPassive = false;
        yield return new WaitUntil(() => GameObject.FindWithTag("Enemy") == null);
        StartCoroutine(End());
    }

    IEnumerator End()
    {
        yield return StartCoroutine(StartAndWaitForVoiceline(7));
        SceneManager.LoadScene(nextScene);
        yield return null;
    }

    IEnumerator StartAndWaitForVoiceline(int voiceLineIndex)
    {
        inputHandler.LockInput();
        voiceLinesInstances[voiceLineIndex].start();
        FMOD.Studio.PLAYBACK_STATE playbackState;
        voiceLinesInstances[voiceLineIndex].getPlaybackState(out playbackState);
        while (playbackState != FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            voiceLinesInstances[voiceLineIndex].getPlaybackState(out playbackState);
            yield return new WaitForSeconds(0.2f);
        }
        inputHandler.ReleaseInput();
    }
}
