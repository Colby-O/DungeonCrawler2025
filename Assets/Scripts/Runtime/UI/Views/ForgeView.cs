using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using System.Collections.Generic;
using PlazmaGames.Attribute;

namespace DC2025
{
    public class ForgeView : View
    {
        [Header("Forge Interface")]
        [SerializeField] private List<InventorySlot> _input;
        [SerializeField] private InventorySlot _output;
        [SerializeField] private EventButton _start;

        private GenericView _generic;
        
        [Header("Debugging")]
        [SerializeField, ReadOnly]private bool _isStarted = false;
        [SerializeField, ReadOnly] private Forge _currentForge;


        private void UpdateForgeState()
        {
            _start.IsDisabled = !CanStartForge();
        }

        public bool IsStarted() => _isStarted;
        public void SetForge(Forge forge) => _currentForge = forge;

        public bool CanStartForge()
        {
            return (_input[0].Item is RawCraftingItem) && 
                (_input[1].Item is RawCraftingItem) && 
                (_input[0].Item as MaterialItem).GetMaterial() == (_input[1].Item as MaterialItem).GetMaterial();
        }

        public void StartForge()
        {
            _isStarted = true;
            _start.IsDisabled = true;
            _input[0].ToogleDisableState(true);
            _input[1].ToogleDisableState(true);
            _generic.ToggleInventory(false, false);

            _input[0].ToogleDisableState(true);
            _input[1].ToogleDisableState(true);
            _output.ToogleDisableState(true);
            _output.SetCoverAmount(1f);
        }

        public override void Init()
        {
            _generic = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>();
            UpdateForgeState();
            _input[0].OnChange.AddListener(UpdateForgeState);
            _input[1].OnChange.AddListener(UpdateForgeState);
            _start.onPointerDown.AddListener(StartForge);

            _input[0].ToogleDisableState(false);
            _input[1].ToogleDisableState(false);
            _output.ToogleDisableState(true);
        }

        public override void Show()
        {
            base.Show();
            DCGameManager.IsPaused = true;
            _generic?.ToggleInventory(!_isStarted, false);
        }

        public override void Hide()
        {
            base.Hide();
            _generic.ToggleInventory(false, true);
        }
    }
}
