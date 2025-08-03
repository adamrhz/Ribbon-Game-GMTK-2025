using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        public bool GamePaused = false;

        private float holdMenuTimer;

        public delegate void LoopEndEvent();
        public event LoopEndEvent OnLoopEnd;

        private int tutorialStageID;

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

        private void HandleTutorial()
        {
            if (tutorialStageID == 0 && Player.Instance.transform.position.x > 38.19935f)
                tutorialStageID = 1;
            
            if (!Player.Instance.Input.BlockInput)
            {
                if (CurrentLevel.LevelName == "1-1")
                {
                    Player.Instance.HUD.MoveJumpTutorial.alpha =
                        Mathf.Lerp(Player.Instance.HUD.MoveJumpTutorial.alpha, 
                            tutorialStageID == 0 ? 1 : 0, 10 * Time.deltaTime);
                    
                    Player.Instance.HUD.HoldSwingTutorial.alpha =
                        Mathf.Lerp(Player.Instance.HUD.HoldSwingTutorial.alpha, 
                            tutorialStageID == 1 && Player.Instance.Controllers.SwingController.SwingingTarget  && 
                                Player.Instance.Machine.CurrentState is not RB_PS_Swing ? 1 : 0, 10 * Time.deltaTime);
                    Player.Instance.HUD.SpinToWinTutorial.alpha =
                        Mathf.Lerp(Player.Instance.HUD.SpinToWinTutorial.alpha, 
                            tutorialStageID == 1 && 
                           Player.Instance.Machine.CurrentState is RB_PS_Swing ? 1 : 0, 10 * Time.deltaTime);
                }
            }
            else
            {
                Player.Instance.HUD.SpinToWinTutorial.alpha =
                    Mathf.Lerp(Player.Instance.HUD.SpinToWinTutorial.alpha, 0, 10 * Time.deltaTime);
            }
        }

        public IEnumerator LevelStart()
        {
            LevelTimer = 0;
            TimerActive = false;
            Player.Instance.Input.BlockInput = true;
            Player.Instance.Init();
            PlayerCamera playerCamera = Camera.main?.GetComponent<PlayerCamera>();
            playerCamera.Intro = true;
            yield return SideScrollTransition();
            yield return SceneManager.LoadSceneAsync(CurrentLevel.SceneName, LoadSceneMode.Additive);
            playerCamera.discreteTarget = GameObject.Find("SwingGoal")?.transform.position ?? Vector3.zero;
            playerCamera.ForcePosition(playerCamera.discreteTarget);
            yield return new WaitForEndOfFrame();
            yield return SideScrollFadeTransition();
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
            }
        }
        private void IncrementLoop()
        {
            tutorialStageID = 2;
            if (CurrentLevel)
            {
                if (CurrentLoop == CurrentLevel.LoopCounts - 1)
                {
                    StartCoroutine(LevelFinished());
                    return;
                }
            }

            CurrentLoop++;
            StartCoroutine(NotifyLoopChange(CurrentLoop));
        }

        public bool IsLevelFinished;
        
        public IEnumerator LevelFinished()
        {
            TimerActive = false;
            IsLevelFinished = true;
            Player.Instance.Input.BlockInput = true;

            if (CurrentLevel)
            {
                if(CurrentLevel.BestTime < 0 || LevelTimer < CurrentLevel.BestTime)
                {
                    CurrentLevel.BestTime = LevelTimer;
                    SaveFileManager.SaveCompleteFile();
                }
            }

            OnLoopEnd?.Invoke();

            yield return new WaitForSecondsRealtime(2f); // Simulating whatever cool wait ending sequence you want here
            if (Player.Instance.Machine.IsCurrentState<RB_PS_Ground>())
            {
                Player.Instance.Visual.Play("Ness");
            }
            yield return new WaitForSecondsRealtime(2f); // Simulating whatever cool wait ending sequence you want here
            StartCoroutine(RestartSequence());

        }
        public IEnumerator NotifyLoopChange(int loop)
        {
            bool Skip = false;
            TimerActive = false;
            List<GameLoopEvent> LoopObjects = new List<GameLoopEvent>(LevelManager.LoopObjects);
            Player.Instance.Input.BlockInput = true;
            yield return new WaitForSecondsRealtime(.4f); // Simulating whatever cool wait ending sequence you want here
            Vector3 SpawnPoint = Vector3.zero;

            SpawnPoint = GameObject.Find("SpawnPoint")?.transform.position ?? Vector3.zero;
            Camera camera = Camera.main;
            PlayerCamera playerCamera = camera?.GetComponent<PlayerCamera>();
            playerCamera.Intro = true;
            bool resetPlayerPosition = false;
            yield return new WaitForEndOfFrame(); // Ensure all updates are processed before notifying



            playerCamera.discreteTarget = Vector3.Lerp(playerCamera.transform.position, SpawnPoint, 10 * Time.deltaTime);
            foreach (GameLoopEvent loopObject in LoopObjects)
            {
                if (Player.Instance.Input.GetAnyButton(true))
                {
                    Skip = true;
                }

                if (!loopObject.WillChangeNextLoop(loop))
                {
                    yield return null;
                    continue;
                }

                playerCamera.discreteTarget = loopObject.transform.position;
                while (Mathf.Abs(playerCamera.transform.position.x - loopObject.transform.position.x) > 5f && !Skip && loopObject.PlayInIntro)
                {
                    if (Player.Instance.Input.GetAnyButton(true))
                    {
                        Skip = true;
                    }
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
                    if (ParticleEffectPrefab != null && difference && loopObject.PlayInIntro)
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
        
        private bool isRestarting;
        public RectTransform RestartTransition;
        
        private IEnumerator RestartSequence()
        {
            yield return SideScrollTransition();
            SceneManager.LoadScene("GameScene");
            Time.timeScale = 1;
        }

        public IEnumerator SideScrollTransition()
        {
            Image image = RestartTransition.gameObject.GetComponent<Image>();
            Color color = image.color;
            color.a = 1;
            image.color = color;
            while (RestartTransition.localScale.x < .994f)
            {
                RestartTransition.localScale =
                    Vector3.Lerp(RestartTransition.localScale, Vector3.one, 9 * Time.unscaledDeltaTime);
                yield return null;
            }
            RestartTransition.localScale = Vector3.one;
        }

        public IEnumerator SideScrollFadeTransition()
        {
            Image image = RestartTransition.gameObject.GetComponent<Image>();
            Color target = image.color;
            target.a = 0;
            while (image.color.a > .006f)
            {
                image.color =
                    Color.Lerp(image.color, target, 9 * Time.unscaledDeltaTime);
                yield return null;
            }
            image.color = target;
        }

        // Update is called once per frame
        void Update()
        {
            HandleTutorial();
            
            if (Player.Instance.Input.GetButtonDown("Pause"))
            {
                GamePaused = !GamePaused;
                Time.timeScale = GamePaused ? Mathf.Epsilon : 1;
            }

            if (GamePaused)
            {
                if (Input.anyKey && !Player.Instance.Input.GetButton("Pause"))
                {
                    holdMenuTimer += Time.unscaledDeltaTime;
                    if (holdMenuTimer >= 0.5f)
                    {
                        if (!isRestarting)
                        {
                            isRestarting = true;
                            StartCoroutine(RestartSequence());
                        }
                    }
                }
                else holdMenuTimer = 0;
            }

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
