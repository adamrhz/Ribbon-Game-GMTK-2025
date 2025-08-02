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

        public void OnStartSwinging(Player player)
        {
            RB_PS_Swing swing = player.Machine.Get<RB_PS_Swing>();
            LoopsAround = 0;
            AngleTotal = 0;
            Direction = (int)Mathf.Sign(swing.RelativeSpeed);
        }
        public void WhileSwinging(Player player)
        {
            if(player.Input.BlockInput) return;
            RB_PS_Swing swing = player.Machine.Get<RB_PS_Swing>();
            if(swing.RelativeSpeed != 0)
            {
                if((int)Mathf.Sign(player.Direction) != (int)Mathf.Sign(Direction))
                {
                    LoopsAround = 0;
                    AngleTotal = 0;
                    Direction = (int)Mathf.Sign(player.Direction);
                }
            }

            AngleTotal += Mathf.Abs(swing.deltaAngle) * Mathf.Rad2Deg;
            if(AngleTotal > 360)
            {
                AngleTotal -= 360;
                LoopsAround++;

                if(LoopsAround >= LoopsToComplete)
                {
                    Debug.Log("Swing Goal Completed!");
                    LevelManager.LoopEnd();
                    LoopsAround = 0;
                    AngleTotal = 0;
                }
            }
        }

        public void OnStopSwinging(Player player)
        {
            if (player.Input.BlockInput)
            {
                player.Rb.velocity = new(0, player.PhysicsInfo.JumpStrength);
                return;

            }
            LoopsAround = 0;
            AngleTotal = 0;
        }

    }
}