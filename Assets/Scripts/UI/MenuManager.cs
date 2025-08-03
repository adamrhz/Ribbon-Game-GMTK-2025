using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Ribbon
{
    public sealed class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;

        [Header("UI")]
        public TMP_Text BestTime;

        public CanvasGroup BestTimeCanvas;

        public bool isBestTimeVisible;

        public Animator MenuAnimator;

        private bool isPlaying;

        public GameObject MainMenuGrid;

        public GameObject[] Menus;

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            MusicPlayer.Stop();
            SetMenu(0);
        }

        public void CloseGame() => Application.Quit();

        // Update is called once per frame
        void Update()
        {
            BestTimeCanvas.alpha = Mathf.Lerp(BestTimeCanvas.alpha, isBestTimeVisible ? 1 : 0, 20 * Time.deltaTime);
        }

        public void PrepareLoadLevel(LevelObject Target)
        {
            if (isPlaying) return;
            MenuAnimator.Play("BeginPlay", 0, 0);
            isPlaying = true;
            StartCoroutine(LevelLoadSequence(Target));
        }

        public void SetMenu(int index)
        {
            if (index < 0 || index >= Menus.Length) return;
            for (int i = 0; i < Menus.Length; i++)
            {
                Menus[i].SetActive(i == index);
            }
        }

        private IEnumerator LevelLoadSequence(LevelObject Target)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            LevelManager.Instance.CurrentLevel = Target;
            MainMenuGrid?.SetActive(false);
            LevelManager.Instance.BeginLevel();
        }
    }

}