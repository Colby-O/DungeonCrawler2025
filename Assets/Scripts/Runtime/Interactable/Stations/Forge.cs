using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;

namespace DC2025
{
    public class Forge : Station
    {
        [Header("Debugging")]
        private ForgeView _view;

        public override void Interact()
        {
            if (_view.IsStarted()) return;

            IsEnabled = !IsEnabled;
            StartTransition();

            if (IsEnabled)
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<ForgeView>();
                _view.SetForge(this);
            }
            else
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.SetForge(null);
            }
        }

        private void Start()
        {
            _view = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<ForgeView>();
        }
    }
}
