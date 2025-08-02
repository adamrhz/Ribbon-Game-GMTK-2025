using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ribbon
{
    public class StageButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private Vector2 pivotStartPoint, startPoint;
        [SerializeField] private RectTransform rectTransform;

        public bool IsHovered;

        public LevelObject AttachedLevel;
        
        private void Start()
        {
            pivotStartPoint = rectTransform.anchoredPosition;
            startPoint = rectTransform.position;
        }
        private void Update()
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, IsHovered ? pivotStartPoint + Vector2.up * 40 : pivotStartPoint, 10 * Time.deltaTime);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHovered = true;
            float time = 0;
            if (AttachedLevel)
            {
                time = AttachedLevel.BestTime > 0 ? AttachedLevel.BestTime : 0;
            }
            string bestTimeText = System.TimeSpan.FromSeconds(time)
            .ToString(@"mm\:ss\:fff");
            MenuManager.Instance.isBestTimeVisible = true;
            MenuManager.Instance.BestTime.SetText(bestTimeText);
            MenuManager.Instance.BestTime.rectTransform.position = startPoint + Vector2.down * 70;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsHovered = false;
            MenuManager.Instance.isBestTimeVisible = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MenuManager.Instance.PrepareLoadLevel(AttachedLevel);
        }
    }
}