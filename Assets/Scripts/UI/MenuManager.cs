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
        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            BestTimeCanvas.alpha = Mathf.Lerp(BestTimeCanvas.alpha, isBestTimeVisible ? 1 : 0, 20 * Time.deltaTime);
        }
    }

}