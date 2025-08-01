using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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


    public static UnityEvent<int> OnLoopChange = new UnityEvent<int>();

    public int CurrentLoop = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        GameObject GameManagerObject = Resources.Load<GameObject>(GameManagerPrefabPath);
        Instantiate(GameManagerObject);
        Instance = GameManagerObject.GetComponent<GameManager>();

    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            IncrementLoop();
        }
    }

    private void IncrementLoop()
    {
        CurrentLoop++;
        OnLoopChange.Invoke(CurrentLoop);
        Debug.Log("Current Loop: " + CurrentLoop);
    }
}
