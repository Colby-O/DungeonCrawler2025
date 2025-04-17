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
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<CraftingTableView>();
                //_view.SetCraftingTable(this);
            }
            else
            {
                GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                //_view.SetCraftingTable(null);
            }
        }

        public override void ForceClose()
        {
            if (IsEnabled)
            {
                IsEnabled = false;
                GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                //_view.SetCraftingTable(null);
            }
        }
    }
}
