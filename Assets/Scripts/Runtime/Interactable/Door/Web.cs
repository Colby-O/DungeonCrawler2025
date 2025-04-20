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
                if (blade.GetMaterial() == _type)
                {
                    Unlock();
                    GameManager.GetMonoSystem<IChatWindowMonoSystem>().Send($"You cut down the cob web and are now able to proceed.");
                }
                else
                {
                    GameManager.GetMonoSystem<IChatWindowMonoSystem>().Send($"You try to enter but are unable to. You need a blade of type <color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[_type])}>{_type}</color> to proceed.");
                }
            }
            else
            {
                GameManager.GetMonoSystem<IChatWindowMonoSystem>().Send($"You try to enter but are unable to. You need a blade of type <color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[_type])}>{_type}</color> to proceed.");
            }
        }

        public override void Close() { }

        public override void Lock() 
        {
            IsOpen = false;
            IsLocked = true;
            gameObject.SetActive(true);
        }

        public override void Unlock() 
        {
            IsOpen = true;
            IsLocked = false;
            gameObject.SetActive(false);
            if (_player != null && _player.Sword().HasSword() && DCGameManager.hasStarted) _player.Sword().Swing();
        }

        protected override void Awake()
        {
            IsLocked = true;
            base.Awake();
        }

        private void Start()
        {
            if (!IsLocked) Unlock();
            _player = DCGameManager.Player.GetComponent<Player>();
            _mr.material.color = DCGameManager.settings.materialColors[_type];
        }

        private void Update()
        {
            if (IsOpen) gameObject.SetActive(false);
        }
    }
}
