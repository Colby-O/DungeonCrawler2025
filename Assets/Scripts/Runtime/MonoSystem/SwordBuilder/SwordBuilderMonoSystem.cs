using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace DC2025
{
    public class SwordBuilderMonoSystem : MonoBehaviour, ISwordBuilderMonoSystem
    {
        private Dictionary<BladeType, GameObject> _blades = new();
        private Dictionary<HandleType, GameObject> _handles = new();

        private void Start()
        {
            _blades.Add(BladeType.Sword, Resources.Load<GameObject>("Prefabs/SwordComponents/SwordBlade"));
            _blades.Add(BladeType.Axe, Resources.Load<GameObject>("Prefabs/SwordComponents/AxeBlade"));
            
            _handles.Add(HandleType.Balanced, Resources.Load<GameObject>("Prefabs/SwordComponents/BalancedHandle"));
            _handles.Add(HandleType.Quick, Resources.Load<GameObject>("Prefabs/SwordComponents/QuickHandle"));
            _handles.Add(HandleType.Lightweight, Resources.Load<GameObject>("Prefabs/SwordComponents/LightweightHandle"));

            BuildSword(transform, BladeType.Sword, HandleType.Quick, MaterialType.Cobolt);
        }

        public void BuildSword(Transform parent, BladeType bladeType, HandleType handleType, MaterialType materialType)
        {
            while (parent.childCount > 0) DestroyImmediate(parent.GetChild(0).gameObject);

            Instantiate(_blades[bladeType], parent);
            Instantiate(_handles[handleType], parent);
        }

        public BladeItem CreateBlade(BladeType bladeType, MaterialType materialType, int rating)
        {
            GameObject go = Instantiate(_blades[bladeType]);
            BladeItem blade = go.AddComponent<BladeItem>();
            blade.bladeType = bladeType;
            blade.SetMaterial(materialType);
            blade.SetRating(rating);
            blade.ForceInit();
            return blade;
        }
    }
}
