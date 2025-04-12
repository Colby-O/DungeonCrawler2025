using UnityEngine;

namespace DC2025
{
    public interface IInteractable
    {
        public bool IsAdjancent { get; set; }
        public bool IsEntered { get; set; }
        public void OnPlayerEnter();
        public void OnPlayerExit();
        public void OnPlayerAdjancentEnter();
        public void OnPlayerAdjancentExit();
    }
}
