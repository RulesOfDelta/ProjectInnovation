using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highscore : MonoBehaviour
{
    private static int _currentHighscore;
    private static List<int> _highscores;
    
    private void Start()
    {
        _currentHighscore = 0;
        _highscores = new List<int>();
    }
    
    /*
     Add to score when:
     1. Successful attack
     2. Successful kill
     3. Proceeding to next room
     
     Reduce score when:
     1. Get hit by enemy
     
     Store score when:
     1. Killed
    */

    public static void AddToHighscore(int valueToAdd)
    {
        if(valueToAdd > 0) _currentHighscore += valueToAdd;
    }

    public static void ReduceHighscore(int valueToReduce)
    {
        if (valueToReduce > 0) _currentHighscore -= valueToReduce;
    }

    public static int GetCurrentHighscore()
    {
        return _currentHighscore;
    }

    public static void SaveCurrentHighscore()
    {
        _highscores.Add(_currentHighscore);
        _currentHighscore = 0;
    }

    public static int GetHighestScore()
    {
        int currentHighestScore = 0;    
        foreach (int highscore in _highscores)
        {
            if (highscore > currentHighestScore) currentHighestScore = highscore;
        }

        return currentHighestScore;
    }
}
