using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace DC2025
{
    public class SwordBuilderMonoSystem : MonoBehaviour, ISwordBuilderMonoSystem
    {
        private Dictionary<BladeType, GameObject> _blades = new();
        private Dictionary<BladeType, GameObject> _unfBlades = new();
        private Dictionary<HandleType, GameObject> _handles = new();

        private void Start()
        {
            _blades.Add(BladeType.ShortSword, Resources.Load<GameObject>("Prefabs/SwordComponents/ShortSwordBlade"));
            _blades.Add(BladeType.LongSword, Resources.Load<GameObject>("Prefabs/SwordComponents/LongSwordBlade"));
            _blades.Add(BladeType.Axe, Resources.Load<GameObject>("Prefabs/SwordComponents/AxeBlade"));
            _blades.Add(BladeType.BattleAxe, Resources.Load<GameObject>("Prefabs/SwordComponents/BattleAxeBlade"));
            _blades.Add(BladeType.Dagger, Resources.Load<GameObject>("Prefabs/SwordComponents/DaggerBlade"));
            
            _unfBlades.Add(BladeType.ShortSword, Resources.Load<GameObject>("Prefabs/SwordComponents/UnfShortSwordBlade"));
            _unfBlades.Add(BladeType.LongSword, Resources.Load<GameObject>("Prefabs/SwordComponents/UnfLongSwordBlade"));
            _unfBlades.Add(BladeType.Axe, Resources.Load<GameObject>("Prefabs/SwordComponents/UnfAxeBlade"));
            _unfBlades.Add(BladeType.BattleAxe, Resources.Load<GameObject>("Prefabs/SwordComponents/UnfBattleAxeBlade"));
            _unfBlades.Add(BladeType.Dagger, Resources.Load<GameObject>("Prefabs/SwordComponents/UnfDaggerBlade"));
            
            _handles.Add(HandleType.Balanced, Resources.Load<GameObject>("Prefabs/SwordComponents/BalancedHandle"));
            _handles.Add(HandleType.Dominant, Resources.Load<GameObject>("Prefabs/SwordComponents/DominantHandle"));
            _handles.Add(HandleType.Lightweight, Resources.Load<GameObject>("Prefabs/SwordComponents/LightweightHandle"));
            _handles.Add(HandleType.Rugged, Resources.Load<GameObject>("Prefabs/SwordComponents/RuggedHandle"));
            _handles.Add(HandleType.Wise, Resources.Load<GameObject>("Prefabs/SwordComponents/WiseHandle"));
        }

        public BladeController BuildSword(Transform parent, BladeType bladeType, HandleType handleType, MaterialType materialType)
        {
            while (parent.childCount > 0) DestroyImmediate(parent.GetChild(0).gameObject);

            Transform blade = Instantiate(_blades[bladeType], parent).transform;
            Transform handle = Instantiate(_handles[handleType], parent).transform;
            foreach (MeshRenderer mr in blade.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.color = DCGameManager.settings.materialColors[materialType];
            }

            return blade.GetComponent<BladeController>();
        }

        public WeaponItem CreateSword(BladeType bladeType, HandleType handleType, MaterialType materialType, int rating)
        {
            GameObject go = new GameObject();
            Transform model = new GameObject("Model").transform;
            model.parent = go.transform;
            BladeController bc = BuildSword(model, bladeType, handleType, materialType);
            
            WeaponItem weapon = go.AddComponent<WeaponItem>();
            weapon.bladeType = bladeType;
            weapon.SetMaterial(materialType);
            weapon.SetRating(rating);
            weapon.handleType = handleType;
            weapon.SetDurability(1);
            bc.SetNormal();
            weapon.SetBladeController(bc);
            weapon.ForceInit();
            return weapon;
        }

        public BladeItem CreateBlade(BladeType bladeType, MaterialType materialType, int rating)
        {
            GameObject go = Instantiate(_blades[bladeType]);
            foreach (MeshRenderer mr in go.transform.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.color = DCGameManager.settings.materialColors[materialType];
            }
            BladeItem blade = go.AddComponent<BladeItem>();
            blade.bladeType = bladeType;
            blade.SetMaterial(materialType);
            blade.SetRating(rating);
            blade.ForceInit();
            return blade;
        }
        
        public UnfBladeItem CreateUnfBlade(BladeType bladeType, MaterialType materialType, int rating)
        {
            GameObject go = Instantiate(_unfBlades[bladeType]);
            foreach (MeshRenderer mr in go.transform.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.color = DCGameManager.settings.materialColors[materialType];
            }
            UnfBladeItem blade = go.AddComponent<UnfBladeItem>();
            blade.bladeType = bladeType;
            blade.SetMaterial(materialType);
            blade.SetRating(rating);
            blade.ForceInit();
            return blade;
        }
    }
}
