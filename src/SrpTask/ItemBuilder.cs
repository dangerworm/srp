using SrpTask.Game;

namespace SrpTask
{
    public class ItemBuilder
    {
        private int _id = 10;
        private string _name = "Item";
        private int _damage = 0;
        private int _heal = 0;
        private int _armour = 0;
        private int _weight = 0;
        private bool _isUnique = false;
        private bool _isRare = false;
        private bool _isConsumable = false;

        public static ItemBuilder Build => new ItemBuilder();

        public Item AnItem()
        {
            return new Item(_id, _name, _damage, _heal, _armour, _weight, 
                _isUnique, _isRare, _isConsumable);
        }

        private ItemBuilder()
        {
        }

        public ItemBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        public ItemBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public ItemBuilder WithDamage(int damage)
        {
            _damage = damage;
            return this;
        }

        public ItemBuilder WithHeal(int heal)
        {
            _heal = heal;
            return this;
        }

        public ItemBuilder WithArmour(int armour)
        {
            _armour = armour;
            return this;
        }

        public ItemBuilder WithWeight(int weight)
        {
            _weight = weight;
            return this;
        }

        public ItemBuilder IsUnique(bool isUnique)
        {
            _isUnique = isUnique;
            return this;
        }

        public ItemBuilder IsRare(bool isRare)
        {
            _isRare = isRare;
            return this;
        }

        public ItemBuilder IsConsumable(bool isConsumable)
        {
            _isConsumable = isConsumable;
            return this;
        }
    }
}