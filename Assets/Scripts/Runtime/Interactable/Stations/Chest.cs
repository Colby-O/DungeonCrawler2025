using PlazmaGames.Attribute;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.Core.Utils;
using PlazmaGames.DataPersistence;
using PlazmaGames.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DC2025
{
    public class Chest : Station, IDataPersistence
    {
        public static int instanaceCount = 0;

        [Header("Chest Settings")]
        [SerializeField] private List<PickupableItem> _itemsPrefab;
        [SerializeField] private int _numSlots = 8;
        [SerializeField] private bool _resetOnRestart = true;
        [SerializeField, ReadOnly] private bool _resetOnAwake = true;

        [SerializeField, ReadOnly] private int id = -1;
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
                if (!GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<GameView>()) GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
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
            if (_slots == null) return;

            foreach (SlotData slot in _slots)
            {
                if (slot != null && slot.Item != null) slot.Item.Release();
            }
        }

        private void InitSlots()
        {
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

        private void Init()
        {
            _view = GameManager.GetMonoSystem<IUIMonoSystem>().GetView<ChestView>();
            if (_resetOnAwake) InitSlots();
        }

        private void OnRestart()
        {
            if (!_resetOnRestart) return;
            Release();
            Init();
        }

        private void Awake()
        {
            id = Chest.instanaceCount++;
        }

        private void Start()
        {
            Init();
            DCGameManager.OnRestart.AddListener(OnRestart);
        }

        public bool SaveData<TData>(ref TData rawData) where TData : GameData
        {
            DCGameData data = rawData as DCGameData;
            if (data == null) return false;

            if (!_resetOnRestart)
            {
                if (IsEnabled) GameManager.GetMonoSystem<IUIMonoSystem>().GetView<ChestView>().FetchSlots(ref _slots);
                if (data.chestInvs == null) data.chestInvs = new PlazmaGames.Runtime.DataStructures.SerializableDictionary<int, List<SlotData>>();

                if (data.chestInvs.ContainsKey(UID.GetID(transform))) data.chestInvs[UID.GetID(transform)] = new List<SlotData>(_slots);
                else data.chestInvs.Add(UID.GetID(transform), new List<SlotData>(_slots));
                return true;
            }
            return false;
        }

        public bool LoadData<TData>(TData rawData) where TData : GameData
        {
            DCGameData data = rawData as DCGameData;
            if (data == null) return false; 

            if (!_resetOnRestart)
            {
                Release();

                _resetOnAwake = false;

                if (data.chestInvs.ContainsKey(UID.GetID(transform)))
                {
                    List<SlotData> slots = data.chestInvs[UID.GetID(transform)];

                    foreach (SlotData slot in slots)
                    {
                        slot.LoadSlot();
                    }

                    _slots = new List<SlotData>(slots);
                }
                else
                {
                    InitSlots();
                }

                return true;
            }
            else _resetOnAwake = true;
            return false;
        }
    }
}
