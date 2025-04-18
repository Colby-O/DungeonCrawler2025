using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;

namespace DC2025
{
    public class MainMenuView : View
    {
        [Header("References")]
        [SerializeField] private GameObject _menuScene;
        [SerializeField] private EventButton _play;
        [SerializeField] private EventButton _settings;
        [SerializeField] private EventButton _quit;

        public void Play()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<GameView>(false);
        }

        public void Settings()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<SettingsView>();
        }

        public void Quit()
        {
            Application.Quit();
        }

        public override void Init()
        {
            _play.onPointerDown.AddListener(Play);
            _settings.onPointerDown.AddListener(Settings);
            _quit.onPointerDown.AddListener(Quit);
        }

        public override void Show()
        {
            base.Show();
            _menuScene.SetActive(true);
            DCGameManager.IsPaused = true;
        }

        public override void Hide()
        {
            base.Hide();
            _menuScene.SetActive(false);
            DCGameManager.IsPaused = false;
        }
    }
}
