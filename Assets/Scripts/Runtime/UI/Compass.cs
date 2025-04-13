using UnityEngine;
using UnityEngine.UI;

namespace DC2025
{
    public class Compass : MonoBehaviour
    {
        [SerializeField] private RawImage _compass;

        private void Start()
        {
            if (_compass == null) _compass = GetComponent<RawImage>();
        }

        private void Update()
        {
            _compass.uvRect = new Rect(DCGameManager.Player.transform.rotation.eulerAngles.y / 360f, 0f, 1f, 1f);
        }
    }
}
