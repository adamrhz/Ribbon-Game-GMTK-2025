using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Ribbon
{
    public class GameLoopEvent : MonoBehaviour, IComparable<GameLoopEvent>
    {
        public int[] LoopsIn = new int[0];
        public int Loop = 0;

        public bool PlayInIntro = true;

        private void Start()
        {
            LevelManager.RegisterLoopObject(this);
            Loop = -1;
            OnGameStart(Loop);
        }


        public void OnDestroy()
        {
            LevelManager.UnregisterLoopObject(this);
        }

        public bool WillChangeNextLoop(int Loop)
        {
            return LoopsIn.Contains(Loop) != gameObject.activeSelf;
        }

        public bool OnGameStart(int Loop)
        {
            this.Loop = Loop;



            foreach (int inLoop in LoopsIn)
            {
                if (Loop == inLoop)
                {
                    gameObject.SetActive(true);
                    return true;
                }
            }

            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                return true;
            }
            return false;
        }

        public int CompareTo(GameLoopEvent other)
        {
            if (!PlayInIntro)
            {
                return 1;
            }
            return (int)Mathf.Sign(other.transform.position.x - transform.position.x);
        }
    }
}