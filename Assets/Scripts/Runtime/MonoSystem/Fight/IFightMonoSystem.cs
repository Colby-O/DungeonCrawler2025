using UnityEngine;
using System.Collections.Generic;
using PlazmaGames.Core.MonoSystem;

namespace DC2025
{
    public interface IFightMonoSystem : IMonoSystem
    {
        public void StartFight(Enemy enemy);
        public bool InFight();
        public void PlayerAttack();
        public bool PlayerBlock();
        public void PlayerAttackDone();
        public void EnemyAttackDone();
        public void AddDamageBoost(float mul, float duration);
        public void AddForesightBoost(float amount, float duration);
    }
}
