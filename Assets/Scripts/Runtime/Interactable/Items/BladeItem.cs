using UnityEngine;

namespace DC2025
{
    public class BladeItem : MaterialItem
    {
        public BladeType bladeType;
        
        public override string GetName()
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[GetMaterial()])}>{GetMaterial()} {bladeType} Blade</color>";
        }

        public override string GetDescription()
        {
            return $"A {GetMaterial()} {bladeType} blade. Needs a handle from the crafting bench before use.";
        }

        public override Sprite GetIcon()
        {
            return Resources.Load<Sprite>($"Icons/Blades/{bladeType}"); ;
        }

        public override void SetMaterial(MaterialType material)
        {
            _type = material;
            foreach (MeshRenderer mr in transform.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.color = DCGameManager.settings.materialColors[_type];
            }
        }

        protected override float RotationOffset()
        {
            return 90.0f;
        }

        protected override float VerticalOffset()
        {
            return 0.25f;
        }

        public override Color GetColor()
        {
            return DCGameManager.settings.materialColors[_type];
        }
    }
}
