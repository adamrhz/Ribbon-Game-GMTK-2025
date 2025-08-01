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

        public Animator SquashAnimator;

        public int SpriteDirection;
        private int previousSpriteDirection;

        // Start is called before the first frame update
        private void Start()
        {
            SpriteDirection = 1;
        }

        // Update is called once per frame
        private void Update()
        {
            float XSpeed = Player.GroundSpeed;
            
            if (Mathf.Abs(XSpeed) > .01f)
            {
                if(XSpeed > 0)
                {
                    SpriteDirection = 1;
                }
                else if (XSpeed < 0)
                {
                    SpriteDirection = -1;
                }
            }

            if (Player.Machine.CurrentState is RB_PS_Swing swing)
            {
                SpriteDirection = Math.Sign(Vector2.Dot(Player.Rb.velocity.normalized, swing.SwingTangent));
                // Debug.Log(Vector2.Dot(Player.Rb.velocity.normalized, swing.SwingTangent));
            }
            
            Sprite.flipX = SpriteDirection != 1;

            if (previousSpriteDirection != SpriteDirection)
            {
                SquashAnimator.Play("SwitchDir", 0, 0);
                previousSpriteDirection = SpriteDirection;
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
    }

}