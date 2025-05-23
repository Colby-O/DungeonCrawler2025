using UnityEngine;

namespace DC2025
{
    public class BladeController : MonoBehaviour
    {
        [SerializeField] private GameObject _normal;
        [SerializeField] private GameObject _damaged;

        public bool IsDamged() => _damaged.activeSelf;

        public void SetNormal()
        {
            _normal.SetActive(true);
            _damaged.SetActive(false);
        }

        public void SetDamaged()
        {
            _damaged.SetActive(true);
            _damaged.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = _normal.transform.GetChild(0).GetComponent<MeshRenderer>().material.color;
            _normal.SetActive(false);
        } 
    }
}
