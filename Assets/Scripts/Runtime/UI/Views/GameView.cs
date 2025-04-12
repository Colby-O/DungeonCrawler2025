using PlazmaGames.UI;
using UnityEngine;

namespace DC2025
{
    public class GameView : View
    {
        public override void Init()
        {

        }

        public override void Show()
        {
            base.Show();
            DCGameManager.IsPaused = false;
        }
    }
}
