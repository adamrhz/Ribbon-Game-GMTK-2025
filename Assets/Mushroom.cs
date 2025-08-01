using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    public class Mushroom : RWorldObject2D
    {
        public float LaunchSpeed;

        public void OnTouchIt(Player player)
        {
            float FallSpeed = -player.YSpeed;

            if(FallSpeed < 0)
            {
                return;
            }
            player.Rb.velocity = new Vector2(player.XSpeed, Mathf.Max(FallSpeed, LaunchSpeed));
            player.OnObject(this, true);
        }
    }
}
