using System;
using System.Collections.Generic;
using System.Linq;

namespace SrpTask.Game
{
    public class RpgPlayer
    {
        private readonly IGameEngine _gameEngine;

        public const int MaximumCarryingCapacity = 1000;
        public List<Item> Inventory;

        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }

        public int Armour { get; private set; }

        /// <summary>
        /// How much the player can carry in kilograms
        /// </summary>
        public int CarryingCapacity { get; private set; }

        public RpgPlayer(IGameEngine gameEngine)
        {
            _gameEngine = gameEngine;

            Inventory = new List<Item>();
            CarryingCapacity = MaximumCarryingCapacity;
        }

        public void UseItem(Item item)
        {
            if (item.DealsDamage)
            {
                var enemies = _gameEngine.GetEnemiesNear(this);

                foreach (var enemy in enemies)
                {
                    enemy.TakeDamage(item.Damage);
                }
            }

            if (item.Heals)
            {
                CurrentHealth += item.Heal;

                if (CurrentHealth > MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                }
            }

            if (item.IsConsumable)
            {
                Inventory.Remove(item);
            }
        }

        public bool PickUpItem(Item item)
        {
            if (!CanCarry(item) || !CanPickUp(item))
            {
                return false;
            }

            // Don't pick up items that give health, just consume them.
            if (item.Heals)
            {
                UseItem(item);
            }
            else
            {
               Inventory.Add(item);
            }

            var specialEffect = item.GetSpecialEffectOnPickUp();
            PlaySpecialEffect(specialEffect);

            CalculateStats();

            return true;
        }

        public void TakeDamage(int damage)
        {
            var damageToDeal = GetDamageToDeal(damage);

            var specialEffect = GetSpecialEffectOnDamage(damageToDeal);
            PlaySpecialEffect(specialEffect);

            CurrentHealth -= damageToDeal;
        }

        private bool CanCarry(Item item)
        {
            return GetInventoryWeight() + item.Weight <= CarryingCapacity;
        }

        private bool CanPickUp(Item item)
        {
            return !(item.IsUnique && ItemExistsInInventory(item));
        }

        private bool ItemExistsInInventory(Item item)
        {
            return Inventory.Any(x => x.Id == item.Id);
        }

        private int GetDamageToDeal(int damage)
        {
            if (damage < Armour)
            {
                return 0;
            }

            var damageToDeal = damage - Armour;

            if (GetInventoryWeight() < MaximumCarryingCapacity / 2)
            {
                // Un-encumbered players can parry more successfully.
                // Reduce damage dealt by 25%.
                damageToDeal = (int)Math.Round(damageToDeal * 0.75);
            }

            return damageToDeal;
        }

        private int GetInventoryWeight()
        {
            return Inventory.Sum(x => x.Weight);
        }

        public string GetSpecialEffectOnDamage(int damage)
        {
            return damage == 0 ? "parry" : "lots_of_gore";
        }

        private void CalculateStats()
        {
            Armour = Inventory.Sum(x => x.Armour);
        }

        private void PlaySpecialEffect(string specialEffect)
        {
            // This check should be moved to whichever class implements
            // the IGameEngine interface
            if (!string.IsNullOrWhiteSpace(specialEffect))
            {
                _gameEngine.PlaySpecialEffect(specialEffect);
            }
        }
    }
}
