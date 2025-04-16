using System;
using System.Collections.Generic;
using PlazmaGames.Core.MonoSystem;
using UnityEngine;

namespace DC2025
{
    public interface ISwordBuilderMonoSystem : IMonoSystem
    {
        public void BuildSword(Transform parent, BladeType bladeType, HandleType handleType, MaterialType materialType);
        public BladeItem CreateBlade(BladeType bladeType, MaterialType materialType, int rating);
    }
}
