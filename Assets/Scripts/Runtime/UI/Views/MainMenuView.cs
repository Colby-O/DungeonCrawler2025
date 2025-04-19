using PlazmaGames.Core;
using PlazmaGames.DataPersistence;
using PlazmaGames.UI;
using UnityEngine;

namespace DC2025
{
    public class MainMenuView : View
    {
        [Header("References")]
        [SerializeField] private GameObject _menuScene;
        [SerializeField] private EventButton _newGame;
        [SerializeField] private EventButton _continue;
        [SerializeField] private EventButton _settings;
        [SerializeField] private EventButton _quit;

        public void NewGame()
        {
            GameManager.GetMonoSystem<IDataPersistenceMonoSystem>().DeleteGame();
            GameManager.GetMonoSystem<IDataPersistenceMonoSystem>().NewGame();
            GameManager.GetMonoSystem<IDataPersistenceMonoSystem>().LoadGame(true);
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<GameView>(false);
        }

        public void Continue()
        {
            GameManager.GetMonoSystem<IDataPersistenceMonoSystem>().LoadGame();
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<GameView>(false);
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
            _newGame.onPointerDown.AddListener(NewGame);
            _continue.onPointerDown.AddListener(Continue);
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

        private void Start()
        {
            _continue.IsDisabled = GameManager.GetMonoSystem<IDataPersistenceMonoSystem>().GetGameData() == null;
        }
    }
}
