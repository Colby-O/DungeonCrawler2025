using PlazmaGames.Animation;
using PlazmaGames.Core;
using UnityEngine;

namespace DC2025
{
    public class DCGameManager : GameManager
    {
        [SerializeField] GameObject _monoSystemHolder;

        [Header("MonoSystems")]
        [SerializeField] private AnimationMonoSystem _animSystem;

        private void AttachMonoSystems()
        {
            AddMonoSystem<AnimationMonoSystem, IAnimationMonoSystem>(_animSystem);
        }

        public override string GetApplicationName()
        {
            return nameof(DCGameManager);
        }

        public override string GetApplicationVersion()
        {
            return "v0.0.1";
        }

        protected override void OnInitalized()
        {
            AttachMonoSystems();

            _monoSystemHolder.SetActive(true);
        }
    }
}