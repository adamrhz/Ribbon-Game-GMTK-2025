using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class PlayerVisual : MonoBehaviour
    {

        public Player Player;
        public Animator Animator;
        public SpriteRenderer Sprite;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float XSpeed = Player.GroundSpeed;
            
            if (Mathf.Abs(XSpeed) > .01f)
            {
                if(XSpeed > 0 && Sprite.flipX)
                {
                    Sprite.flipX = false;
                }
                else if (XSpeed < 0 && Sprite.flipX == false)
                {
                    Sprite.flipX = true;
                }
            }

            Animator?.SetFloat("XSpeed", Mathf.Abs(XSpeed));
            Animator?.SetFloat("YSpeed", Player.YSpeed);
            Animator?.SetInteger("State", Player.Machine.CurrentState.StateNumber);
        }
        public void SetTrigger(string name)
        {
            if (Animator)
            {
                Animator.SetTrigger(name);
            }
        }

        public void Play(string name)
        {
            Animator?.Play(name);
        }
    }

}