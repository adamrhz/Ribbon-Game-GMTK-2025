using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ribbon
{
    [Serializable]
    public class PlayerControllers
    {
        public List<PlayerController> Controllers = new List<PlayerController>();
        public SwingController SwingController = new SwingController();

        public void Init(Player player)
        {
            SwingController.Init(player);
            Controllers.Add(SwingController);
        }

        public void Update()
        {
            foreach (PlayerController controller in Controllers)
            {
                controller.OnUpdate();
            }
        }
    }
}