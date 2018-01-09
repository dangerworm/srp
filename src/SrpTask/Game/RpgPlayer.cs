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
            if (item.Name == "Stink Bomb")
            {
                var enemies = _gameEngine.GetEnemiesNear(this);

                foreach (var enemy in enemies)
                {
                    enemy.TakeDamage(100);
                }
            }
        }

        public bool PickUpItem(Item item)
        {
            var weight = CalculateInventoryWeight();
            if (weight + item.Weight > CarryingCapacity)
                return false;

            if (item.IsUnique && CheckIfItemExistsInInventory(item))
                return false;

            // Don't pick up items that give health, just consume them.
            if (item.Heal > 0)
            {
                CurrentHealth += item.Heal;

                if (CurrentHealth > MaxHealth)
                    CurrentHealth = MaxHealth;

                if (item.Heal > 500)
                {
                    _gameEngine.PlaySpecialEffect("green_swirly");
                }

                return true;
            }

            if (item.IsRare)
                _gameEngine.PlaySpecialEffect("cool_swirly_particles");

            Inventory.Add(item);

            CalculateStats();

            return true;
        }

        private void CalculateStats()
        {
            Armour = Inventory.Sum(x => x.Armour);
        }

        private bool CheckIfItemExistsInInventory(Item item)
        {
            return Inventory.Any(x => x.Id == item.Id);
        }

        private int CalculateInventoryWeight()
        {
            return Inventory.Sum(x => x.Weight);
        }

        public void TakeDamage(int damage)
        {
            if (damage < Armour)
            {
                _gameEngine.PlaySpecialEffect("parry");
                return;
            }

            var damageToDeal = damage - Armour;
            CurrentHealth -= damageToDeal;
            
            _gameEngine.PlaySpecialEffect("lots_of_gore");
        }
    }
}