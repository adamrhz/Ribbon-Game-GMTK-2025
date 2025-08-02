using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ribbon
{

    public enum DamageType
    {
        Normal,
        Death
    }
    public class DamageObject : RWorldObject2D
    {
        public DamageType damageType = DamageType.Normal;


        private void Start()
        {
            OnStay.AddListener(OnTouchIt);
        }

        public void OnTouchIt(Player player)
        {
            switch(damageType)
            {
                case DamageType.Normal:
                    player.TriggerDamage();
                    break;

                case DamageType.Death:
                    player.TriggerDeath();
                    break;
            }
        }

    }
}