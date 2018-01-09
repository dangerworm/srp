namespace SrpTask.Game
{
    public class Item
    {
        /// <summary>
        /// Item's unique Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Item's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// How much the item heals by
        /// </summary>
        public int Heal { get; set; }

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

        public Item(int id, string name, int heal, int armour, int weight, bool isUnique, bool isRare)
        {
            IsRare = isRare;
            Name = name;
            Heal = heal;
            Armour = armour;
            Weight = weight;
            IsUnique = isUnique;
            Id = id;
        }
    }
}