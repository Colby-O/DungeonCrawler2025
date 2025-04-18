using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;

namespace DC2025
{
    public class Anvil : Station
    {
        [SerializeField, ReadOnly] private AnvilView _view;
        
        private void Start()
        {
            _view = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<AnvilView>();
        }
        
        public override void Interact()
        {
            if (_view.IsStarted()) return;

            IsEnabled = !IsEnabled;
            StartTransition();

            if (IsEnabled)
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<AnvilView>();
                _view.SetAnvil(this);
            }
            else
            {
                if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<AnvilView>()) GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.SetAnvil(null);
                OnClose();
            }
        }

        public override void ForceClose()
        {
            if (IsEnabled)
            {
                IsEnabled = false;
                if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<AnvilView>()) GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.SetAnvil(null);
                OnClose();
            }
        }
    }
}
