using DC2025.Utils;
using UnityEngine;

namespace DC2025
{
    public class MinimapFollow : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Camera _cam;
        [SerializeField] private Transform _target;
        [SerializeField] private float _height;
        [SerializeField] private bool _canRotate;

        private void Start ()
        {
            if (_cam == null) _cam = GetComponent<Camera>();
            if (_target == null) _target = DCGameManager.Player.transform;

            _cam.orthographic = true;
            _cam.orthographicSize = _height;
        }

        private void Update()
        {
            Vector3 targetPos = _target.position;
            transform.position = targetPos.SetY(targetPos.y + _height);
            if (_canRotate)
            {
                Quaternion targetRot = _target.rotation;
                transform.rotation = Quaternion.Euler(90, targetRot.eulerAngles.y, 0);
            }
        }
    }
}
