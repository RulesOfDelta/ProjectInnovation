using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Introduction : MonoBehaviour
{
    private InputHandler inputHandler;
    private int test;
    void Start()
    {
        inputHandler = GameObject.FindWithTag("InputHandler").GetComponent<InputHandler>();
        StartCoroutine(IntroductionCoroutine());
    }
    
    IEnumerator IntroductionCoroutine()
    {
        //Audio
        StartCoroutine(CheckForMovement());
        yield return null;
    }

    IEnumerator CheckForMovement()
    {
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
}
