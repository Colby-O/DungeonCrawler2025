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

        [Header("Inventory")]
        [SerializeField] private SerializableDictionary<MaterialTypes, int> _craftingMats;

        [Header("Potions")]
        [SerializeField] private List<Potion> _potions;

        [Header("Vitals")]
        [SerializeField, ReadOnly] private float _curHealth;

        public float GetHealth() => _curHealth;

        public float GetMaxHealth() => _maxHealth;

        public bool GivePotion(Potion potion)
        {
            if (_potions.Count >= 2) return false;
            _potions.Add(potion);
            return true;
        }

        public bool UsePotion(int index)
        {
            if (_potions.Count <= index) return false;
            _potions[index].Use(this);
            _potions.RemoveAt(index);
            return true;
        }

        public int GetCraftingMaterial(MaterialTypes type)
        {
            return _craftingMats.ContainsKey(type) ? _craftingMats[type] : 0;
        }

        public void AddCraftingMaterial(MaterialTypes type, int amount)
        {
            if (CanTakeCraftingMaterial(type, -amount))
            {
                if (_craftingMats.ContainsKey(type)) _craftingMats[type] += amount;
                else _craftingMats.Add(type, amount);
            }
        }

        public bool CanTakeCraftingMaterial(MaterialTypes type, int amount)
        {
            return ((_craftingMats.ContainsKey(type) ? _craftingMats[type] : 0) - amount) >= 0;
        }

        public void Damage(int amount)
        {
            _curHealth -= amount;

            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().UpdateHealth();

            if (_curHealth <= 0)
            {
                OnDeath();
            }
        }

        public void Heal(int amount)
        {
            _curHealth = Mathf.Clamp(_curHealth + amount, 0, _maxHealth);
            GameManager.GetMonoSystem<IUIMonoSystem>().GetView<GameView>().UpdateHealth();
        }

        public void OnDeath()
        {
            Debug.Log("The Player Had Died :<");
        }

        private void Awake()
        {
            _curHealth = _maxHealth;
            _potions = new List<Potion>();
            _craftingMats = new SerializableDictionary<MaterialTypes, int>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) { Damage(10); }
            if (Input.GetKeyDown(KeyCode.Q)) { Heal(5); }
        }
    }
}
