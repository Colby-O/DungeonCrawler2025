using PlazmaGames.Attribute;
using PlazmaGames.DataPersistence;
using UnityEngine;

namespace DC2025
{
    public class KeyPad : MonoBehaviour, IDataPersistence
    {
        private static int _instanceCount;

        [SerializeField] private MaterialType _type;
        [SerializeField] private MeshRenderer _mr;
        [SerializeField] private MeshRenderer _mr2;

        [SerializeField, ReadOnly] private bool _isLocked;

        [SerializeField, ReadOnly] private bool _lockedState;

        public MaterialType GetMaterial() => _type;

        [SerializeField, ReadOnly] private int _id;

        public void Unlock()
        {
            _isLocked = false;
            gameObject.SetActive(false);
        }

        public void Lock()
        {
            _isLocked = true;
            gameObject.SetActive(true);
        }

        public bool IsLocked()
        {
            return _isLocked;
        }

        private void Awake()
        {
            _isLocked = true;
            _lockedState = _isLocked;
            _mr.materials[1].color = DCGameManager.settings.materialColors[_type];
            _mr2.materials[1].color = DCGameManager.settings.materialColors[_type];
            _id = _instanceCount++;
        }

        public bool SaveData<TData>(ref TData rawData) where TData : GameData
        {
            DCGameData data = rawData as DCGameData;
            if (data == null) return false;

            if (data.keypadLockedStates.ContainsKey(UID.GetID(transform))) data.keypadLockedStates[UID.GetID(transform)] = _isLocked;
            else data.keypadLockedStates.Add(UID.GetID(transform), _isLocked);
            return true;
        }

        public bool LoadData<TData>(TData rawData) where TData : GameData
        {
            DCGameData data = rawData as DCGameData;
            if (data == null) return false;

            _isLocked = (data.keypadLockedStates.ContainsKey(UID.GetID(transform))) ? data.keypadLockedStates[UID.GetID(transform)] : _lockedState;

            if (!_isLocked) Unlock();
            else Lock();

            return false;
        }
    }
}
