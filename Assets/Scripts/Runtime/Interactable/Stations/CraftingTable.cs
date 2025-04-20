using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using IUIMonoSystem = PlazmaGames.UI.IUIMonoSystem;

namespace DC2025
{
    public class CraftingTable : Station
    {
        [SerializeField, ReadOnly] private CraftingTableView _view;
        
        private void Start()
        {
            _view = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<CraftingTableView>();
        }
        
        public override void Interact()
        {
            IsEnabled = !IsEnabled;
            StartTransition();

            if (IsEnabled)
            {
                if (!GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<GameView>()) GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<CraftingTableView>();
                _view.SetTable(this);
            }
            else
            {
                if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<CraftingTableView>()) GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.SetTable(null);
                OnClose();
            }
        }

        public override void ForceClose()
        {
            if (IsEnabled)
            {
                IsEnabled = false;
                if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<CraftingTableView>()) GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.SetTable(null);
                OnClose();
            }
        }
    }
}
