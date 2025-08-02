using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ribbon
{
    public class GameLoopEvent : MonoBehaviour, IComparable<GameLoopEvent>
    {
        public int[] LoopsIn = new int[0];
        public int Loop = 0;
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
            return (int)Mathf.Sign(other.transform.position.x - transform.position.x);
        }
    }
}