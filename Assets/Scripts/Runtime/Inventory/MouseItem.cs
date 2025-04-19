using DC2025.Utils;
using NUnit.Framework.Interfaces;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DC2025
{
    public class MouseItem : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] Image _icon;

        public SlotData Data { get; private set; }
        public PickupableItem Item { get { return Data.Item; } set { Data.Item = value; } }

        [SerializeField, ReadOnly] private bool _wasUpdatedThisFrame = false;

        public bool HasItem() => Item != null;
        
        public bool IsOverUIObject()
        {
            PointerEventData e = new PointerEventData(EventSystem.current);
            e.position = Mouse.current.position.ReadValue();

            List<RaycastResult> res = new List<RaycastResult>();
            EventSystem.current.RaycastAll(e, res);
            if (res.Count > 0) Debug.Log($"{res[0].gameObject.name} : {res[0].gameObject.transform.position} : {e.position}");
            return res.Count > 0;
        }

        public void UpdateSlot(PickupableItem obj)
        {
            Item = obj;
            Item.Hide();
            _icon.sprite = Item.GetIcon();
            _icon.color = Item.GetColor();
            
            _wasUpdatedThisFrame = true;
        }

        public void Clear()
        {
            Item = null;
            _icon.sprite = null;
            _icon.color = Color.clear;

            _wasUpdatedThisFrame = true;
        }

        private void Drop()
        {
            if (GameManager.GetMonoSystem<IFightMonoSystem>().InFight()) return;
            if (Item.Drop())
            {
                Clear();
            }
        }

        private void FollowCurosr()
        {
            if (Item != null)
            {
                transform.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).SetZ(transform.position.z);
                if (!_wasUpdatedThisFrame && Mouse.current.leftButton.wasPressedThisFrame && !IsOverUIObject()) Drop();
            }
        }

        private void Awake()
        {
            Data = new SlotData();
            Clear();
        }

        private void Update()
        {
            FollowCurosr();
            _wasUpdatedThisFrame = false;

            if (HasItem())
            {
                Debug.Log("HasItem");
                DCGameManager.isHovering = true;
            }
            
        }
    }
}
