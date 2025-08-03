using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ribbon
{
    public class SwingGoal : MonoBehaviour
    {
        public int LoopsAround = 0;
        public int Direction = 1;
        public float AngleTotal = 0;
        public int LoopsToComplete = 5;
        private bool InCompleteState = false;
        public GameObject SpriteObject;
        private Animator SprAnim;
        
        public void Start()
        {
            SprAnim = SpriteObject.GetComponent<Animator>();
        }

        public void OnStartSwinging(Player player)
        {
            RB_PS_Swing swing = player.Machine.Get<RB_PS_Swing>();
            InCompleteState = false;
            LoopsAround = 0;
            AngleTotal = 0;
            Direction = (int)Mathf.Sign(swing.RelativeSpeed);
            AnimationChecks();
        }
        public void WhileSwinging(Player player)
        {
            if (player.Input.BlockInput) return;
            RB_PS_Swing swing = player.Machine.Get<RB_PS_Swing>();
            if ((int)Mathf.Sign(swing.Direction) != (int)Mathf.Sign(Direction))
            {
                LoopsAround = 0;
                AngleTotal = 0;
                Direction = (int)Mathf.Sign(player.Direction);
            }
            AngleTotal += Mathf.Abs(swing.deltaAngle) * Mathf.Rad2Deg;
            if (AngleTotal > 360)
            {
                AngleTotal -= 360;
                LoopsAround++;

                if (LoopsAround >= LoopsToComplete)
                {
                    player.AudioBankHolder.Play("HighDing");
                    Debug.Log("Swing Goal Completed!");
                    LevelManager.LoopEnd();
                    StartCoroutine(ResetAnimation());
                    LoopsAround = 0;
                    AngleTotal = 0;
                    InCompleteState = true;
                }
                else
                {
                    player.AudioBankHolder.Play("LowDing");
                }
            }

            AnimationChecks();
        }

        private IEnumerator ResetAnimation()
        {
            yield return new WaitForSeconds(2);
            if (!LevelManager.Instance.IsLevelFinished) InCompleteState = false;
            AnimationChecks();
        }

        public void OnStopSwinging(Player player)
        {
            if (player.Input.BlockInput)
            {
                player.AudioBankHolder.Play("Yippee");
                player.Rb.velocity = new(0, player.PhysicsInfo.JumpStrength);
                LoopsAround = 0;
                AngleTotal = 0;
                Invoke("AnimationChecks", 10f);
                return;

            }
            LoopsAround = 0;
            AngleTotal = 0;
            AnimationChecks();
        }

        public void AnimationChecks()
        {
            if (LoopsAround > 0)
            {
                //Update animator with the corresponding value
                string AnimationName = LoopsAround.ToString();
                SprAnim?.Play(AnimationName);
            }
            else
            {
                //Update animator to empty frame
                if (InCompleteState) { SprAnim?.Play("5"); }
                else { SprAnim?.Play("Empty"); }
            }
        }

    }
}