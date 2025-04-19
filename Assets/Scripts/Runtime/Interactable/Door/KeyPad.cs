using PlazmaGames.Attribute;
using UnityEngine;

namespace DC2025
{
    public class KeyPad : MonoBehaviour
    {
        [SerializeField] private MaterialType _type;
        [SerializeField] private MeshRenderer _mr;

        [SerializeField, ReadOnly] private bool _isLocked;

        public MaterialType GetMaterial() => _type;

        public void Unlock()
        {
            _isLocked = false;
            gameObject.SetActive(false);
        }

        public bool IsLocked()
        {
            return _isLocked;
        }

        private void Awake()
        {
            _isLocked = true;
            _mr.materials[1].color = DCGameManager.settings.materialColors[_type];
        }
    }
}
