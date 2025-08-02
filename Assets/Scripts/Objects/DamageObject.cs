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
    public enum ConditionType
    {
        None,
        NegativeYSpeed,
        PositiveYSpeed
    }
    public class DamageObject : RWorldObject2D
    {
        public DamageType damageType = DamageType.Normal;
        public ConditionType ConditionType = ConditionType.None;

        private void Start()
        {
            OnStay.AddListener(OnTouchIt);
        }

        public void OnTouchIt(Player player)
        {
            if((ConditionType == ConditionType.NegativeYSpeed && player.YSpeed > 0) || (ConditionType == ConditionType.PositiveYSpeed && player.YSpeed < 0))
            {
                return;
            }
            switch (damageType)
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