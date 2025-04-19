using PlazmaGames.Attribute;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.Core.Utils;
using PlazmaGames.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DC2025
{
    public class Chest : Station
    {
        [Header("Chest Settings")]
        [SerializeField] private List<PickupableItem> _itemsPrefab;
        [SerializeField] private int _numSlots = 8;
        [SerializeField] private bool _resetOnRestart = true;

        [SerializeField, ReadOnly] private List<SlotData> _slots;
        private ChestView _view;

        public List<SlotData> GetSlots()
        {
            return _slots;
        }

        public override void Interact()
        {
            IsEnabled = !IsEnabled;
            if (IsEnabled)
            {
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.openChestSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
                GameManager.GetMonoSystem<IUIMonoSystem>().Show<ChestView>();
                _view.SetSlots(_slots);
                _view.SetCurremtChest(this);
            }
            else
            {
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.closeChestSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
                if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<ChestView>()) GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.FetchSlots(ref _slots);
                _view.SetCurremtChest(null);
                OnClose();
            }
        }

        public override void ForceClose()
        {
            if (IsEnabled)
            {
                IsEnabled = false;
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.closeChestSound, PlazmaGames.Audio.AudioType.Sfx, false, true);
                if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<ChestView>()) GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
                _view.FetchSlots(ref _slots);
                OnClose();
            }
        }

        private void Release()
        {
            foreach (SlotData slot in _slots)
            {
                if (slot != null && slot.Item != null) slot.Item.Release();
            }
        }

        private void Init()
        {
            _view = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<ChestView>();

            _slots = new List<SlotData>();
            for (int i = 0; i < _numSlots; i++)
            {
                _slots.Add(new SlotData());
            }

            List<SlotData> slotsRandomOrder = _slots.OrderBy(x => Random.value).ToList();

            for (int i = 0; i < Mathf.Min(_itemsPrefab.Count, _numSlots); i++)
            {
                PickupableItem item = Instantiate(_itemsPrefab[i]);
                item.ForceInit();
                item.Hide();
                slotsRandomOrder[i].Item = item;
            }
        }

        private void OnRestart()
        {
            if (!_resetOnRestart) return;
            Release();
            Init();
        }

        private void Start()
        {
            Init();
            DCGameManager.OnRestart.AddListener(OnRestart);
        }
    }
}
