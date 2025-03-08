using Events.Channels;
using NUnit.Framework;
using UnityEngine;

namespace Enemy
{
    public class BasicEnemy: AbstractEnemy
    {
        
        public override void AttackStart()
        {
            // does nothing lol
        }
        
        protected override void OnHit(float healthRemaining)
        {
        }

        protected override void OnHeal(float healthRemaining)
        {
            
        }

        protected override void OnDeath(GameObject deadEnemy)
        {
            gameObject.SetActive(false);
        }
    }
}