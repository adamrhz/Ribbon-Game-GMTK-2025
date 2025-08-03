using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Ribbon
{
    public sealed class PlayerHUD : MonoBehaviour
    {
        public CanvasGroup HUDAlpha;
        
        public TMP_Text ButtonCount, Timer;
        
        private const string UI_CORE_FORMAT = "<mspace=25>{0}";
        private const string RING_COUNT_ATTACH_001_FORMAT = "<alpha=#55>00<alpha=#FF>{0}";
        private const string RING_COUNT_ATTACH_011_FORMAT = "<alpha=#55>0<alpha=#FF>{0}";
        private const string RING_COUNT_ATTACH_111_FORMAT = "{0}";

        [Header("Pause Menu")] public TextWobble PausedText;
        public RectTransform Tooltip;
        public CanvasGroup PauseBackground;

        private float pauseAnchorY, pauseTooltipAnchorY;

        [Header("Tutorial Stuff")] public CanvasGroup MoveJumpTutorial;
        public CanvasGroup HoldSwingTutorial;
        public CanvasGroup SpinToWinTutorial;
        
        private void Start()
        {
            pauseAnchorY = PausedText.GetComponent<RectTransform>().anchoredPosition.y;
            pauseTooltipAnchorY = Tooltip.anchoredPosition.y;
            PausedText.GetComponent<RectTransform>().anchoredPosition = Tooltip.anchoredPosition = new Vector2(0, 50);

            HUDAlpha.alpha = 0;

            MoveJumpTutorial.alpha = HoldSwingTutorial.alpha = SpinToWinTutorial.alpha = 0;
        }
        
        public void Update()
        {
            HUDAlpha.alpha = Mathf.Lerp(HUDAlpha.alpha, LevelManager.Instance.CurrentLevel ? 1 : 0,
                4 * Time.unscaledDeltaTime);
            
            PausedText.offsetTime = Time.unscaledTime;
            string attachRingText = RING_COUNT_ATTACH_111_FORMAT;

            int buttonCount = Player.Instance.ButtonCount;
            
            if (buttonCount < 100) attachRingText = RING_COUNT_ATTACH_011_FORMAT;
            if (buttonCount < 10) attachRingText = RING_COUNT_ATTACH_001_FORMAT;

            attachRingText = string.Format(attachRingText, buttonCount);
            if (ButtonCount) ButtonCount.text = string.Format(UI_CORE_FORMAT, attachRingText);

            TimeSpan span = TimeSpan.FromSeconds(LevelManager.Instance.LevelTimer);
            if (Timer) Timer.text =
                string.Format(UI_CORE_FORMAT, $"{span.Minutes:00}:{span.Seconds:00}:{Mathf.Floor(span.Milliseconds / 10):00}");

            PauseBackground.alpha = Mathf.Lerp(PauseBackground.alpha, LevelManager.Instance.GamePaused ? 1 : 0,
                8 * Time.unscaledDeltaTime);
            
            PausedText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 
                Mathf.Lerp( PausedText.GetComponent<RectTransform>().anchoredPosition.y, LevelManager.Instance.GamePaused ? pauseAnchorY : 50, 8 * Time.unscaledDeltaTime));
            Tooltip.anchoredPosition = new Vector2(0, 
                Mathf.Lerp(Tooltip.anchoredPosition.y, LevelManager.Instance.GamePaused ? pauseTooltipAnchorY : 50, 8 * Time.unscaledDeltaTime));
        }
    }

}