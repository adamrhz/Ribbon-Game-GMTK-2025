using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoopEvent : MonoBehaviour
{
    public int[] LoopsIn = new int[0];

    private void Start()
    {
        if(GameManager.Instance != null)
        {
            GameManager.OnLoopChange.AddListener(OnGameStart);
        }
    }

    public void OnGameStart(int Loop)
    {

        gameObject.SetActive(false);

        Debug.Log("GameLoopEvent OnGameStart called for " + gameObject.name);

        foreach (int inLoop in LoopsIn)
        {
            if(Loop == inLoop)
            {
                Debug.Log("GameLoopEvent OnGameStart: Loop " + inLoop + " matched for " + gameObject.name);
                gameObject.SetActive(true);
                return;
            }
        }
    }
}
