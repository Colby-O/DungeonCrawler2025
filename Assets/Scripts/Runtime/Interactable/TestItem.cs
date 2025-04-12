using PlazmaGames.Attribute;
using UnityEngine;

namespace DC2025
{
    public class TestItem : MonoBehaviour, IInteractable
    {
        public bool IsAdjancent { get; set; }
        public bool IsEntered { get; set; }
        public bool HasCollider { get { return false; } }

        [SerializeField, ReadOnly] bool _isEntered;
        [SerializeField, ReadOnly] bool _isAdjancent;

        public void OnPlayerAdjancentEnter()
        {
            Debug.Log("On Adjancent Enter");
        }

        public void OnPlayerAdjancentExit()
        {
            Debug.Log("On Adjancent Exit");
        }

        public void OnPlayerEnter()
        {
            Debug.Log("On Enter");
        }

        public void OnPlayerExit()
        {
            Debug.Log("On Exit");
        }

        private void FixedUpdate()
        {
            _isEntered = IsEntered;
            _isAdjancent = IsAdjancent;
        }
    }
}
