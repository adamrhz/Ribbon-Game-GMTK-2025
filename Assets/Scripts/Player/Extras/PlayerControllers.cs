using System;
using System.Collections;
using UnityEngine;

namespace Ribbon
{
    [Serializable]
    public class PlayerControllers
    {
        public SwingController SwingController = new SwingController();

        public void Init(Player player)
        {
            SwingController.Init(player);
        }

        public void Update()
        {
            SwingController.OnUpdate();
        }
    }
}