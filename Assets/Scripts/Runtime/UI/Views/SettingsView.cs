using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using UnityEngine;
using UnityEngine.UI;

namespace DC2025
{
    public class SettingsView : View
    {
        [Header("References")]
        [SerializeField] private GameObject _menuScene;
        [SerializeField] private Slider _overall;
        [SerializeField] private Slider _sfx;
        [SerializeField] private Slider _music;
        [SerializeField] private EventButton _back;

        private void Back()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
        }

        private void SetOverallVolume(float val)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetOverallVolume(val);
        }

        private void SetSfXVolume(float val)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetSfXVolume(val);
        }

        private void SetMusicVolume(float val)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().SetMusicVolume(val);
        }

        public override void Init()
        {
            _back.onPointerDown.AddListener(Back);

            _overall.onValueChanged.AddListener(SetOverallVolume);
            _music.onValueChanged.AddListener(SetMusicVolume);
            _sfx.onValueChanged.AddListener(SetSfXVolume);

            _overall.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetOverallVolume();
            _sfx.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetSfXVolume();
            _music.value = GameManager.GetMonoSystem<IAudioMonoSystem>().GetMusicVolume();
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
