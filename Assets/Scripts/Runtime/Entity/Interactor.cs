using PlazmaGames.Attribute;
using PlazmaGames.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DC2025
{
    public class Interactor : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _head;

        [Header("Input System")]
        [SerializeField] private PlayerInput _input;

        [Header("Debugging")]
        [SerializeField, ReadOnly] private string _nearbyStationName;

        private Vector3 _cameraPos;
        private Quaternion _cameraRot;

        public Station NearbyStation { get; set; }

        public Camera GetCamera()
        {
            return _camera;
        }

        public (Vector3, Quaternion) GetCameraLoc()
        {
            return (_head.transform.TransformPoint(_cameraPos), _head.transform.rotation * _cameraRot);
        }

        private void HandleInteractAction(InputAction.CallbackContext e)
        {
            if (NearbyStation != null) NearbyStation.Interact();
        }

        private void Awake()
        {
            if (_input == null) _input = GetComponent<PlayerInput>();
            if (_camera == null) _camera = Camera.main;

            _input.actions["Interact"].performed += HandleInteractAction;

            _cameraPos = _camera.transform.localPosition;
            _cameraRot = _camera.transform.rotation;
        }

        private void Update()
        {
            _nearbyStationName = (NearbyStation != null) ? NearbyStation.gameObject.name : "Null";
        }
    }
}
