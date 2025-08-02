using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Ribbon
{

    public class LevelManager : MonoBehaviour
    {
        public LevelObject CurrentLevel;
        
        public static LevelManager Instance
        {

            get
            {
                return _instance;
            }

            set
            {
                if (_instance == null)
                {
                    _instance = value;
                }
                else if (_instance != value && value != null)
                {
                    Debug.LogError("LevelManager instance already exists. Cannot set a new instance.");
                }
            }

        }
        private static LevelManager _instance;
        
        public static List<GameLoopEvent> LoopObjects = new List<GameLoopEvent>();

        public int CurrentLoop = 0;

        public AssetReferenceGameObject ParticleEffectPrefab;
        public float LevelTimer = 0;
        public bool TimerActive = false;

        private void Awake()
        {
            Instance = this;
            CurrentLoop = 0;
        }
        
        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            LevelTimer = 0;   
        }

        public IEnumerator LevelStart()
        {
            LevelTimer = 0;
            TimerActive = false;
            Player.Instance.Input.BlockInput = true;
            Player.Instance.Init();
            PlayerCamera playerCamera = Camera.main?.GetComponent<PlayerCamera>();
            playerCamera.Intro = true;
            yield return SceneManager.LoadSceneAsync(CurrentLevel.SceneName, LoadSceneMode.Additive);
            playerCamera.discreteTarget = GameObject.Find("SwingGoal")?.transform.position ?? Vector3.zero;
            playerCamera.ForcePosition(playerCamera.discreteTarget);
            yield return new WaitForEndOfFrame();
            yield return NotifyLoopChange(CurrentLoop);
            TimerActive = false;
            Player.Instance.Input.BlockInput = true;
            Player.Instance.Visual.Play("IdleMad");
            yield return new WaitForSecondsRealtime(.5f); // Simulating whatever cool wait ending sequence you want here
            Player.Instance.AudioBankHolder.Play("ReadySetGo");
            yield return new WaitForSecondsRealtime(2f); // Simulating whatever cool wait ending sequence you want here
            Player.Instance.Input.BlockInput = false;
            TimerActive = true;

        }
        public static void RegisterLoopObject(GameLoopEvent loopObject)
        {
            if (!LoopObjects.Contains(loopObject))
            {
                LoopObjects.Add(loopObject);
                LoopObjects.Sort();
                Debug.Log("Registered GameLoopEvent: " + loopObject.name);
            }
        }
        private void IncrementLoop()
        {
            if (CurrentLevel)
            {
                if (CurrentLoop == CurrentLevel.LoopCounts - 1)
                {
                    StartCoroutine(LevelFinished());
                    Debug.Log("Reached the end. YOU WON");
                    return;
                }
            }

            CurrentLoop++;
            StartCoroutine(NotifyLoopChange(CurrentLoop));
        }

        public IEnumerator LevelFinished()
        {
            TimerActive = false;
            Player.Instance.Input.BlockInput = true;

            if (CurrentLevel)
            {
                if(CurrentLevel.BestTime < 0 || LevelTimer < CurrentLevel.BestTime)
                {
                    CurrentLevel.BestTime = LevelTimer;
                    MenuManager.Instance.BestTime.text = "Best Time: " + LevelTimer.ToString("F2") + "s";
                    MenuManager.Instance.isBestTimeVisible = true;
                }
                else
                {
                    MenuManager.Instance.isBestTimeVisible = false;
                }
            }


            yield return new WaitForSecondsRealtime(3f); // Simulating whatever cool wait ending sequence you want here
            if (Player.Instance.Machine.IsCurrentState<RB_PS_Ground>())
            {
                Player.Instance.Visual.Play("Ness");
            }
            yield return new WaitForSecondsRealtime(3f); // Simulating whatever cool wait ending sequence you want here
            GameManager.LevelFinished();

        }
        public IEnumerator NotifyLoopChange(int loop)
        {
            TimerActive = false;
            List<GameLoopEvent> LoopObjects = new List<GameLoopEvent>(LevelManager.LoopObjects);
            Player.Instance.Input.BlockInput = true;
            yield return new WaitForSecondsRealtime(1f); // Simulating whatever cool wait ending sequence you want here
            Vector3 SpawnPoint = Vector3.zero;

            SpawnPoint = GameObject.Find("SpawnPoint")?.transform.position ?? Vector3.zero;
            Camera camera = Camera.main;
            PlayerCamera playerCamera = camera?.GetComponent<PlayerCamera>();
            playerCamera.Intro = true;
            Debug.Log("Notifying loop change to " + loop);
            bool resetPlayerPosition = false;
            yield return new WaitForEndOfFrame(); // Ensure all updates are processed before notifying



            playerCamera.discreteTarget = Vector3.Lerp(playerCamera.transform.position, SpawnPoint, 10 * Time.deltaTime);
            foreach (GameLoopEvent loopObject in LoopObjects)
            {
                playerCamera.discreteTarget = loopObject.transform.position;
                while (Mathf.Abs(playerCamera.transform.position.x - loopObject.transform.position.x) > 5f)
                {
                    yield return null;
                }

                if (!Player.Instance.Visual.Sprite.isVisible && !resetPlayerPosition)
                {
                    resetPlayerPosition = true;
                    Player.Instance.Direction = 1;
                    Player.Instance.transform.position = SpawnPoint;
                }
                if (loopObject.Loop != loop)
                {

                    bool difference = loopObject.OnGameStart(loop);
                    if (ParticleEffectPrefab != null && difference)
                    {
                        ParticleEffectPrefab.InstantiateAsync(loopObject.transform.position, Quaternion.identity);
                    }
                }
                yield return null;
            }

            if (!resetPlayerPosition)
            {
                resetPlayerPosition = true;
                Player.Instance.Direction = 1;
                Player.Instance.transform.position = SpawnPoint;
            }
            playerCamera.Intro = false;
            yield return new WaitForSecondsRealtime(1f); // Ensure all updates are processed before notifying
            Player.Instance.Input.BlockInput = false;

            if (CurrentLevel)
            {
                if (CurrentLoop == CurrentLevel.LoopCounts - 1)
                {
                    MusicPlayer.MPlayer.PlaySong(CurrentLevel.FinalLapMusicTrack, true);
                }
            }
            TimerActive = true;

        }



        // Update is called once per frame
        void Update()
        {
            if (TimerActive)
            {
                LevelTimer += Time.deltaTime;
            }
        }

        public static void LoopEnd()
        {
            Instance?.IncrementLoop();
        }

        public static void UnregisterLoopObject(GameLoopEvent gameLoopEvent)
        {
            if (LoopObjects.Contains(gameLoopEvent))
            {
                LoopObjects.Remove(gameLoopEvent);
            }
            LoopObjects.TrimExcess();
        }

        public void BeginLevel()
        {
            if (CurrentLevel)
            {
                MusicPlayer.MPlayer.PlaySong(CurrentLevel.MusicTrack, true);
            }

            StartCoroutine(LevelStart());
        }
    }
}
