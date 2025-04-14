using DC2025.Utils;
using PlazmaGames.Attribute;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DC2025
{
    public class MouseItem : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] Image _icon;

        public Item ItemData {get; set;}
        public PickupableItem CurrentObject { get; set;}

        [SerializeField, ReadOnly] private bool _wasUpdatedThisFrame = false;

        public bool HasItem() => CurrentObject != null && ItemData != null;
        
        public void UpdateSlot(Item data, PickupableItem obj)
        {
            ItemData = data;
            CurrentObject = obj;
            _icon.sprite = ItemData.Icon;
            _icon.color = Color.white;

            _wasUpdatedThisFrame = true;
        }

        public void Clear()
        {
            ItemData = null;
            CurrentObject = null;
            _icon.sprite = null;
            _icon.color = Color.clear;

            _wasUpdatedThisFrame = true;
        }

        private void Drop()
        {
            CurrentObject.Drop();
            Clear();
        }

        private void FollowCurosr()
        {
            if (ItemData != null)
            {
                transform.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).SetZ(transform.position.z);
                if (!_wasUpdatedThisFrame && Mouse.current.leftButton.wasPressedThisFrame) Drop();
            }
        }

        private void Awake()
        {
            Clear();
        }

        private void Update()
        {
            FollowCurosr();
            _wasUpdatedThisFrame = false;
        }
    }
}
