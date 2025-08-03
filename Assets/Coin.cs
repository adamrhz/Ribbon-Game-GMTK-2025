using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ribbon
{
    public class Coin : RWorldObject2D
    {


        private void Start()
        {
            OnEnter.AddListener(OnTouchIt);
        }


        private void Update()
        {
        }

        public void OnTouchIt(Player player)
        {
            if (player == null) return;
            player.AddButton();
            Destroy(gameObject);
        }

    }
}