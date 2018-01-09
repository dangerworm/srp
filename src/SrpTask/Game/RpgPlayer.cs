using System;
using System.Collections.Generic;
using System.Linq;

namespace SrpTask.Game
{
    public class RpgPlayer
    {
        private readonly IGameEngine _gameEngine;

        public const int MaximumCarryingCapacity = 1000;
        private const int SuperHealthPotionThreshold = 500;
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
            if (item.Name.Equals("Stink Bomb"))
            {
                var enemies = _gameEngine.GetEnemiesNear(this);

                foreach (var enemy in enemies)
                {
                    enemy.TakeDamage(100);
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
            if (item.Heal > 0)
            {
                CurrentHealth += item.Heal;

                if (CurrentHealth > MaxHealth)
                    CurrentHealth = MaxHealth;

                if (item.Heal > SuperHealthPotionThreshold)
                {
                    _gameEngine.PlaySpecialEffect("green_swirly");
                }

                return true;
            }

            if (item.IsUnique && item.IsRare)
            {
                _gameEngine.PlaySpecialEffect("blue_swirly");
            }
            else if (item.IsRare)
            {
                _gameEngine.PlaySpecialEffect("cool_swirly_particles");
            }

            Inventory.Add(item);

            CalculateStats();

            return true;
        }

        public void TakeDamage(int damage)
        {
            if (damage <= Armour)
            {
                _gameEngine.PlaySpecialEffect("parry");
                return;
            }

            var damageToDeal = damage - Armour;

            var weight = CalculateInventoryWeight();
            if (weight < MaximumCarryingCapacity / 2)
            {
                // Un-encumbered players can parry more successfully.
                // Reduce damage dealt by 25%.
                damageToDeal = (int)Math.Round(damageToDeal * 0.75);
            }

            CurrentHealth -= damageToDeal;
            
            _gameEngine.PlaySpecialEffect("lots_of_gore");
        }

        private bool CanCarry(Item item)
        {
            var weight = CalculateInventoryWeight();
            return weight + item.Weight <= CarryingCapacity;
        }

        private bool CanPickUp(Item item)
        {
            return !(item.IsUnique && ItemExistsInInventory(item));
        }

        private bool ItemExistsInInventory(Item item)
        {
            return Inventory.Any(x => x.Id == item.Id);
        }

        private int CalculateInventoryWeight()
        {
            return Inventory.Sum(x => x.Weight);
        }

        private void CalculateStats()
        {
            Armour = Inventory.Sum(x => x.Armour);
        }
    }
}