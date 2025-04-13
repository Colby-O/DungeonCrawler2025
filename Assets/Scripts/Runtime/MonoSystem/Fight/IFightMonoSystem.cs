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
        public void PlayerAttackDone();
    }
}
