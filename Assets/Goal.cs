using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Ribbon {
public class Goal : RWorldObject2D
{




        public void OnTouchIt(Player player)
        {
            if(player.Input.BlockInput) return;
            LevelManager.LoopEnd();
        }


}

}