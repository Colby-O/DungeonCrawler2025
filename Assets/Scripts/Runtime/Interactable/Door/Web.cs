using PlazmaGames.Core;
using UnityEngine;

namespace DC2025
{
    public class Web : Blockage
    {
        [Header("References")]
        [SerializeField] private MeshRenderer _mr;

        [Header("settings")]
        [SerializeField] private MaterialType _type;

        private Player _player;

        public override void Open()
        {
            InventorySlot slot = GameManager.GetMonoSystem<IInventoryMonoSystem>().GetHandSlot(SlotType.Left);

            if (slot.Data.Item is WeaponItem)
            {
                WeaponItem blade = slot.Data.Item as WeaponItem;
                if (blade.GetMaterial() == _type) Unlock();
            }
        }

        public override void Close() { }

        public override void Lock() 
        {
            IsOpen = false;
            gameObject.SetActive(true);
        }

        public override void Unlock() 
        {
            IsOpen = true;
            gameObject.SetActive(false);
            _player.Sword().Swing();
        }

        private void Start()
        {
            _player = DCGameManager.Player.GetComponent<Player>();
            _mr.material.color = DCGameManager.settings.materialColors[_type];
        }
    }
}
