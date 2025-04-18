using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DC2025
{
    public class Interactor : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _head;

        [Header("Pickingup Settings")]
        [SerializeField] private int _clickDist = 1;

        [Header("Input System")]
        [SerializeField] private PlayerInput _input;

        [Header("Debugging")]
        [SerializeField, ReadOnly] private string _nearbyStationName;

        private Vector3 _cameraPos;
        private Quaternion _cameraRot;

        public Station NearbyStation { get; set; }
        public Blockage NearbyBlockage { get; set; }

        public int GetClickDistance() => _clickDist;

        public Camera GetCamera()
        {
            return _camera;
        }

        public (Vector3, Quaternion) GetCameraLoc()
        {
            return (_head.transform.TransformPoint(_cameraPos), _head.transform.rotation * _cameraRot);
        }

        public void Interact()
        {
            if (NearbyStation != null) NearbyStation.Interact();
            if (NearbyBlockage != null) NearbyBlockage.Interact();
        }

        private void HandleInteractAction(InputAction.CallbackContext e)
        {
            Interact();
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceAction1Highlighted(true);
        }

        private void HandleInteractCancel(InputAction.CallbackContext e)
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().ForceAction1Highlighted(false);
        }

        private void Awake()
        {
            if (_input == null) _input = GetComponent<PlayerInput>();
            if (_camera == null) _camera = Camera.main;

            _input.actions["Interact"].performed += HandleInteractAction;
            _input.actions["Interact"].canceled += HandleInteractCancel;

            _cameraPos = _camera.transform.localPosition;
            _cameraRot = _camera.transform.rotation;
        }

        private void Update()
        {
            _nearbyStationName = (NearbyStation != null) ? NearbyStation.gameObject.name : "Null";
        }
    }
}
