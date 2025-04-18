using UnityEngine;

namespace DC2025
{
    public class UnfBladeItem : MaterialItem
    {
        public BladeType bladeType;
        public override Sprite GetIcon()
        {
            return Resources.Load<Sprite>($"Icons/UnfBlades/{bladeType}"); ;
        }

        public override string GetDescription()
        {
            return $"An unfinished {GetMaterial()} {bladeType} blade. Needs to be refined on an anvil.";
        }

        public override string GetName()
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(DCGameManager.settings.materialColors[GetMaterial()])}>Unfinished {GetMaterial()} {bladeType} Blade</color>";
        }

        public override void SetMaterial(MaterialType material)
        {
            _type = material;
            foreach (MeshRenderer mr in transform.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.color = DCGameManager.settings.materialColors[_type];
            }
        }
        public override Color GetColor()
        {
            return DCGameManager.settings.materialColors[_type];
        }
    }
}
