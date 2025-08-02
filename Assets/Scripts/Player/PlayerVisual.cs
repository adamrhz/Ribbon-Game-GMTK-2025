using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace Ribbon
{
    public class PlayerVisual : MonoBehaviour
    {
        public Player Player;
        public Animator Animator;
        public SpriteRenderer Sprite;

        public Animator SquashAnimator;

        public LineRenderer IndicatorLine, AttachRibbonLine;

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

            SpriteDirection = Player.Direction;

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
            Animator?.SetInteger("State", Player?.Machine?.CurrentState?.StateNumber ?? 0);
        }
        public void SetTrigger(string name)
        {
            if (Animator)
            {
                Animator.SetTrigger(name);
            }
        }

        public void ToggleIndicatorLine(Vector2? target)
        {
            if (target.HasValue)
            {
                IndicatorLine.positionCount = 2;
                IndicatorLine.SetPositions(new Vector3[]{Player.transform.position, (Vector3)target});
            }
            else
            {
                IndicatorLine.positionCount = 0;
            }
        }
        public void ToggleRibbonAttachLine(Vector2? target)
        {
            if (target.HasValue)
            {
                AttachRibbonLine.positionCount = 2;
                AttachRibbonLine.SetPositions(new Vector3[] { Player.transform.position, (Vector3)target });
            }
            else
            {
                AttachRibbonLine.positionCount = 0;
            }
        }
        public void Play(string name)
        {
            Animator?.Play(name);
        }

        public bool IsPlaying(string idleAnim)
        {
            return Animator?.GetCurrentAnimatorStateInfo(0).IsName(idleAnim) ?? false;
        }
    }

}