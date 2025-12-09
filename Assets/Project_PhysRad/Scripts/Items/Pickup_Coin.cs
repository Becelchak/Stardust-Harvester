using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Gameplay
{
    public class Pickup_Coin : Pickup
    {
        public override void HandlePickup()
        {
            base.HandlePickup();

            //Player.m_Current.AddAmmo();
        }
    }
}