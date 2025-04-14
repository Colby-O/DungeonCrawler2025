using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;

namespace DC2025
{
    public class Forge : Station
    {
        public override void Interact()
        {
            Debug.Log("Interacting With Forge!");
            IsEnabled = !IsEnabled;
            StartTransition();

            if (IsEnabled) GameManager.GetMonoSystem<IUIMonoSystem>().Show<ForgeView>();
            else GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
        }
    }
}
