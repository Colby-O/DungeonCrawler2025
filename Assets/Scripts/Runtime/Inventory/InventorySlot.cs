using DC2025.Utils;
using PlazmaGames.Attribute;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.DataPersistence;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

namespace DC2025
{
    public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDataPersistence
    {
        private static int instanceCount;

        [Header("UI")]
        [SerializeField] private Image _icon;
        [SerializeField] private List<GameObject> _stars;
        [SerializeField] private GameObject _durablityContainer;
        [SerializeField] private RectTransform _durablityProgress;
        [SerializeField] private GameObject _cover;
        [SerializeField] private GameObject _hint;
        [SerializeField] private bool _disableLoad = false;

        [Header("Infomaton")]
        [SerializeField] SlotType _type;
        [SerializeField] private bool _disabled = false;
        [SerializeField] private bool _disablePopup = false;

        [SerializeField, ReadOnly] private int _id;
        [SerializeField, ReadOnly] private bool _hasLoaded;

        private IInventoryMonoSystem _inventory;

        public SlotData Data { get; private set; }
        public PickupableItem Item { get { return (Data != null) ? Data.Item : null; } set { if (Data != null) Data.Item = value; } }

        public UnityEvent OnChange = new UnityEvent();

        public void SetCoverAmount(float amount)
        {
            _cover.transform.localScale = _cover.transform.localScale.SetY(Mathf.Clamp01(amount));
        }

        public void ToogleDisableState(bool state)
        {
            _disabled = state;
            _cover.SetActive(_disabled);
        }

        public void ClearData()
        {
            Data = null;
        }

        public bool HasItem() => Item != null;

        public void ResetRating()
        {
            for (int i = 0; i < _stars.Count; i++)
            {
                _stars[i].SetActive(false);
            }
        }

        public void SetStarSlots(int rating)
        {
            ResetRating();
            for (int i = 0; i < Mathf.Min(rating, _stars.Count); i++)
            {
                _stars[i].SetActive(true);
            }
        }

        public void ToggleDurability(bool state)
        {
            _durablityContainer.SetActive(state);
        }

        public void SetDurability(float val)
        {
            _durablityProgress.localScale = _durablityProgress.localScale.SetX(Mathf.Clamp01(val));
        }

        public void Refresh()
        {
            if (Data == null || Item == null)
            {
                if (Data == null) Data = new SlotData();
                Clear();
                return;
            }

            _icon.sprite = Item.GetIcon();
            _icon.color = Item.GetColor();

            if (_hint != null) _hint.SetActive(false);

            if (Item is MaterialItem)
            {
                SetStarSlots((Item as MaterialItem).GetRating());
            }
            else
            {
                ResetRating();
            }

            if (Item is WeaponItem)
            {
                ToggleDurability(true);
                SetDurability((Item as WeaponItem).GetDurability());
            }
            else
            {
                SetDurability(0);
                ToggleDurability(false);
            }
        }

        public void UpdateSlot(SlotData data) => UpdateSlot((data != null) ? data.Item : null);

        public void UpdateSlot(PickupableItem obj)
        {
            Debug.Log(transform.name);
            Item = obj;
            Item?.Hide();
            Refresh();
            OnChange.Invoke();
        }

        public void Clear()
        {
            Item = null;
            _icon.sprite = null;
            _icon.color = Color.clear;
            if (_hint != null) _hint.SetActive(true);
            ResetRating();
            SetDurability(0);
            ToggleDurability(false);
            OnChange.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_disablePopup && !_inventory.GetMouseSlot().HasItem() && Item != null)
            {
                _inventory.GetPopup().Enable();
                _inventory.GetPopup().SetText($"<align=center>{Item.GetName()}</align>\n\n<align=left>{Item.GetDescription()}</align>");
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _inventory.GetPopup().Disable();
        }

        private void Awake()
        {
            if (!_hasLoaded)
            {
                Data = new SlotData();
                Clear();
            }
            _inventory = GameManager.GetMonoSystem<IInventoryMonoSystem>();
            ToogleDisableState(_disabled);
            _id = InventorySlot.instanceCount++;
        }

        private void Start()
        {
            //LoadData(GameManager.GetMonoSystem<IDataPersistenceMonoSystem>().GetGameData());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_disabled) return;

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (Item != null)
                {
                    if (Item is PotionItem)
                    {
                        (Item as PotionItem).Use();
                        Item.Release();
                        Clear();
                    }
                }
            } 
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(DCGameManager.settings.uiClickSound, PlazmaGames.Audio.AudioType.Sfx, false, true);

                if (_inventory.GetMouseSlot().HasItem())
                {
                    if (Item == null)
                    {
                        UpdateSlot(_inventory.GetMouseSlot().Item);
                        _inventory.GetMouseSlot().Clear();
                    }
                    else
                    {
                        PickupableItem temp = Item;
                        Clear();
                        UpdateSlot(_inventory.GetMouseSlot().Item);
                        _inventory.GetMouseSlot().Clear();
                        _inventory.GetMouseSlot().UpdateSlot(temp);
                    }
                    if (!_disablePopup)
                    {
                        _inventory.GetPopup().Enable();
                        _inventory.GetPopup().SetText($"<align=center>{Item.GetName()}</align>\n\n<align=left>{Item.GetDescription()}</align>");
                    }
                }
                else if (Item != null)
                {
                    _inventory.GetPopup().Disable();
                    _inventory.GetMouseSlot().UpdateSlot(Item);
                    Clear();
                }
            }
        }

        public bool SaveData<TData>(ref TData rawData) where TData : GameData
        {
            if (_disableLoad) return false;

            DCGameData data = rawData as DCGameData;
            if (data == null) return false;

            if (data.slots.ContainsKey(_id)) data.slots[_id] = Data;
            else data.slots.Add(_id, Data);
            return true;
        }

        public bool LoadData<TData>(TData rawData) where TData : GameData
        {
            if (_disableLoad) return false;

            DCGameData data = rawData as DCGameData;
            if (data == null) return false;

            if (data.slots.ContainsKey(_id))
            {
                Debug.Log("Loaded");
                Data = data.slots[_id];
                Data.LoadSlot();
                Refresh();
                _hasLoaded = true;
                OnChange?.Invoke();
            }
            else if (_hasLoaded)
            {
                Data = new SlotData();
                Clear();
                OnChange?.Invoke();
            }
            return true;
        }
    }
}
