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

        [Header("Attach Ribbon Visual")] public int AttachRibbonLinePointCount = 20;

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
                AttachRibbonLine.positionCount = AttachRibbonLinePointCount;
                for (int i = 0; i < AttachRibbonLinePointCount; i++)
                {
                    if (Player.Machine.CurrentState is not RB_PS_Swing Swing) return;
                    
                    Vector3 pointOffset = Vector2.zero;

                    float speedMultiplier = Player.Rb.velocity.sqrMagnitude / 100;

                    float angle = i * 1.0f / AttachRibbonLinePointCount * Mathf.PI;

                    pointOffset = Swing.SwingTangent * SpriteDirection * (Mathf.Sin(angle) * 0.12f * speedMultiplier);
                    
                    AttachRibbonLine.SetPosition(i, Vector3.Lerp(Player.transform.position, (Vector3)target, i * 1.0f / AttachRibbonLinePointCount) + pointOffset);
                }
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