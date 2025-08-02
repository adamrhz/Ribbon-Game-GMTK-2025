using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string GameManagerPrefabPath = "GameManager";

    public static GameManager Instance
    {

        get
        {
            return _instance;
        }

        set
        {
            if(_instance == null)
            {
                _instance = value;
            }
            else if(_instance != value)
            {
                Debug.LogError("GameManager instance already exists. Cannot set a new instance.");
            }
        }

    }
    private static GameManager _instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        GameObject GameManagerObject = Resources.Load<GameObject>(GameManagerPrefabPath);
        GameObject gm = Instantiate(GameManagerObject);
        Instance = gm.GetComponent<GameManager>();
        DontDestroyOnLoad(Instance.gameObject);

    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static void LevelFinished()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
