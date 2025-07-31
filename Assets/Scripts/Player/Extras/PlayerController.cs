using System;
using System.Collections;
using UnityEngine;

namespace Ribbon
{
    [Serializable]
    public class PlayerController : Controller<Player>
    {
        public Player Player => Context;


        public override void Init(Player player)
        {
            Context = player;
        }


        public virtual void OnUpdate()
        {

        }
    }
}