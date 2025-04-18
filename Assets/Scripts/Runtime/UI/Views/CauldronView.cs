using PlazmaGames.UI;
using System.Collections.Generic;
using PlazmaGames.Core;
using UnityEngine;

namespace DC2025
{
    public class CauldronView : View
    {
        [Header("Cauldron Interface")]
        [SerializeField] private InventorySlot _input;
        [SerializeField] private InventorySlot _output;
        [SerializeField] private EventButton _brew;

        private Cauldron _currentCauldron;
        private GenericView _generic;

        public void SetCauldron(Cauldron cauldron) => _currentCauldron = cauldron;

        private bool CanBrew()
        {
            return _input.Item is RawCraftingItem;
        }

        private void UpdateCauldranState()
        {
            _brew.IsDisabled = !CanBrew();
            _input.ToogleDisableState(_output.Item != null);
            _output.ToogleDisableState(_output.Item == null);
        }

        private void CreatePotion()
        {
            if (!CanBrew()) return;

            PotionItem potion = Instantiate(Resources.Load<PotionItem>("Prefabs/Items/PotionItem"));
            potion.SetMaterial((_input.Item as RawCraftingItem).GetMaterial());
            potion.SetRating(0);
            potion.ForceInit();
            _output.UpdateSlot(potion);
        }

        private void ClearInput()
        {
            _input.Item.Release();
            _input.Clear();
        }

        private void Brew()
        {
            CreatePotion();
            ClearInput();
        }

        public override void Init()
        {
            _generic = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>();
            _input.OnChange.AddListener(UpdateCauldranState);
            _output.OnChange.AddListener(UpdateCauldranState);
            UpdateCauldranState();
            _output.ToogleDisableState(true);

            _brew.onPointerDown.AddListener(Brew);
        }

        public override void Show()
        {
            base.Show();
            DCGameManager.IsPaused = true;
            _generic?.ToggleInventory(true, false);
        }

        public override void Hide()
        {
            base.Hide();
            DCGameManager.IsPaused = false;
            _generic?.ToggleInventory(false, false);
            if (_currentCauldron != null && _currentCauldron.IsEnabled) _currentCauldron.Interact();
        }
    }
}
