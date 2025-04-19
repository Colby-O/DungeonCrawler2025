using PlazmaGames.Core;
using PlazmaGames.DataPersistence;
using PlazmaGames.UI;
using UnityEngine;

namespace DC2025
{
    public class PauseMenuView : View
    {
        [Header("References")]
        [SerializeField] private GameObject _menuScene;
        [SerializeField] private EventButton _resume;
        [SerializeField] private EventButton _settings;
        [SerializeField] private EventButton _quit;

        private void Resume()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
        }

        public void Settings()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<SettingsView>();
        }

        public void Quit()
        {
            GameManager.GetMonoSystem<IDataPersistenceMonoSystem>().SaveGame();
            Application.Quit();
        }

        public override void Init()
        {
            _resume.onPointerDown.AddListener(Resume);
            _settings.onPointerDown.AddListener(Settings);
            _quit.onPointerDown.AddListener(Quit);
        }

        public override void Show()
        {
            base.Show();
            _menuScene.SetActive(true);
            if (DCGameManager.Player != null) DCGameManager.Player.GetCamera().gameObject.SetActive(false);
            DCGameManager.IsPaused = true;
        }

        public override void Hide()
        {
            base.Hide();
            _menuScene.SetActive(false);
            if (DCGameManager.Player != null) DCGameManager.Player.GetCamera().gameObject.SetActive(true);
            DCGameManager.IsPaused = false;
        }
    }
}
