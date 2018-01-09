namespace SrpTask.Game
{
    public class Item
    {
        public const int SuperHealthPotionThreshold = 500;

        /// <summary>
        /// Item's unique Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Item's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// How much damage the item deals
        /// </summary>
        public int Damage { get; set; }

        /// <summary>
        /// Whether the item deals damage
        /// </summary>
        public bool DealsDamage => Damage > 0;

        /// <summary>
        /// How much the item heals by
        /// </summary>
        public int Heal { get; set; }

        /// <summary>
        /// Whether the item causes healing
        /// </summary>
        public bool Heals => Heal > 0;

        /// <summary>
        /// How much armour the player gets when it is equipped
        /// </summary>
        public int Armour { get; set; }

        /// <summary>
        /// How much this item weighs in kilograms
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// A unique item can only be picked up once
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Rare items are posh and shiny
        /// </summary>
        public readonly bool IsRare;

        /// <summary>
        /// Consumable items disappear from inventory when used
        /// </summary>
        public readonly bool IsConsumable;

        public Item(int id, string name, int damage, int heal, int armour, 
            int weight, bool isUnique, bool isRare, bool isConsumable)
        {
            Id = id;
            Name = name;
            Damage = damage;
            Heal = heal;
            Armour = armour;
            Weight = weight;
            IsUnique = isUnique;
            IsRare = isRare;
            IsConsumable = isConsumable;
        }

        /// <summary>
        /// Accessor for any special effects played when the item is picked up
        /// </summary>
        /// <returns>
        /// A string containing the name of the special effect
        /// </returns>
        public string GetSpecialEffectOnPickUp()
        {
            // Healing first, as these get used straight away
            if (Heal > SuperHealthPotionThreshold)
            {
                return "green_swirly";
            }

            if (IsUnique && IsRare)
            {
                return "blue_swirly";
            }

            if (IsRare)
            {
                return "cool_swirly_particles";
            }

            return null;
        }
    }
}