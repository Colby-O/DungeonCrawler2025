using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;

namespace DC2025
{
    public class Cauldron : Station
    {
        private CauldronView _view;

        public override void Interact()
        {
            IsEnabled = !IsEnabled;
            StartTransition();

            if (IsEnabled)
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<CauldronView>();
                _view.SetCauldron(this);
            }
            else
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.SetCauldron(null);
                OnClose();
            }
        }

        public override void ForceClose()
        {
            if (IsEnabled)
            {
                IsEnabled = false;
                GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.SetCauldron(null);
                OnClose();
            }
        }

        private void Start()
        {
            _view = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<CauldronView>();
        }
    }
}
