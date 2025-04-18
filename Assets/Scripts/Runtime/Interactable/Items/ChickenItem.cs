using UnityEngine;

namespace DC2025
{
    public class ChickenItem : PickupableItem
    {
        private bool _placed = false;
        protected override bool CanPickup() => !_placed;
        protected override void Start()
        {
            base.Start();
            OnPickup.AddListener(OnPickupChick);
            OnDrop.AddListener(OnDropChick);
        }

        private void OnDropChick()
        {
            transform.GetComponent<Chicken>().CauseDistraction();
            _placed = true;
        }

        private void OnPickupChick()
        {
            transform.GetComponent<Chicken>().RemoveSelf();
        }

        public override Sprite GetIcon()
        {
            return Resources.Load<Sprite>("Icons/Chicken");
        }

        public override string GetDescription() => "A noisy chicken, useful for distracting monsters.";

        public override string GetName() => "Noisy Chicken";
    }
}
