using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.Runtime.DataStructures;
using PlazmaGames.UI;
using System.Collections.Generic;
using UnityEngine;

namespace DC2025
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("Player Stats")]
        [SerializeField] private float _maxHealth;
        [SerializeField] private float _maxStamina;
        [SerializeField] private float _StaminaRefillRate;
        [SerializeField] private float _StaminaRefillDelay = 1.5f;

        [Header("Vitals")]
        [SerializeField, ReadOnly] private float _curHealth;
        [SerializeField, ReadOnly] private float _curStamina;

        [SerializeField, ReadOnly] private Vector3 _start;

        private float _staminaRefilCountdown = 0;

        public float GetHealth() => _curHealth;

        public float GetMaxHealth() => _maxHealth;

        public float GetStamina() => _curStamina;

        public float GetMaxStamina() => _maxStamina;

        public void UseStamina (int amount)
        {
            _staminaRefilCountdown = _StaminaRefillDelay;
            _curStamina = Mathf.Max(_curStamina - amount, 0);

            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>().UpdateStamina();
        }

        public void Damage(int amount)
        {
            _curHealth -= amount;

            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>().UpdateHealth();

            if (_curHealth <= 0)
            {
                OnDeath();
            }
        }

        public void Heal(int amount)
        {
            _curHealth = Mathf.Clamp(_curHealth + amount, 0, _maxHealth);
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>().UpdateHealth();
        }

        public void AddStamina(int amount)
        {
            _curStamina = Mathf.Min(_curStamina + amount, _maxStamina);
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>().UpdateStamina();
        }

        public void OnDeath()
        {
            transform.position = _start;
            DCGameManager.PlayerController.Sync();
            DCGameManager.OnRestart.Invoke();
        }

        private void RefillStamina()
        {
            if (DCGameManager.IsPaused) return;

            _staminaRefilCountdown -= Time.deltaTime;

            if (_staminaRefilCountdown <= 0)
            {
                _curStamina = Mathf.Min(_curStamina + _StaminaRefillRate * Time.deltaTime, _maxStamina);
                GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>().UpdateStamina();
            }
        }

        private void OnRestart()
        {
            _curHealth = _maxHealth;
            _curStamina = _maxStamina;
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>().UpdateHealth();
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GenericView>().UpdateStamina();
        }

        private void Awake()
        {
            _curHealth = _maxHealth;
            _curStamina = _maxStamina;
        }

        private void Start()
        {
            _start = transform.position;
            DCGameManager.OnRestart.AddListener(OnRestart);
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space)) { UseStamina(10); }
            RefillStamina();
        }
    }
}
