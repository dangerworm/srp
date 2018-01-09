using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SrpTask.Game;

namespace SrpTask.GameTests
{
    [TestFixture]
    public class RpgPlayerTests
    {
        public RpgPlayer Player { get; set; }

        public Mock<IGameEngine> Engine { get; set; }

        [SetUp]
        public void Setup()
        {
            Engine = new Mock<IGameEngine>();
            Player = new RpgPlayer(Engine.Object);
        }

        [Test]
        public void PickUpItem_ThatCanBePickedUp_ItIsAddedToTheInventory()
        {
            // Arrange
            var item = ItemBuilder.Build.AnItem();
            Player.Inventory.Should().BeEmpty();

            // Act
            Player.PickUpItem(item);

            // Assert
            Player.Inventory.Should().Contain(item);
        }

        [Test]
        public void PickUpItem_ThatGivesHealth_HealthIsIncreased()
        {
            // Arrange
            Player.MaxHealth = 100;
            Player.CurrentHealth = 10;

            var healthPotion = ItemBuilder
                .Build
                .WithHeal(100)
                .AnItem();

            // Act
            Player.PickUpItem(healthPotion);

            // Assert
            Player.CurrentHealth.Should().BeGreaterThan(10);
        }

        [Test]
        public void PickUpItem_ThatGivesHealth_ItemIsNotAddedToInventory()
        {
            // Arrange
            var healthPotion = ItemBuilder
                .Build
                .WithHeal(100)
                .AnItem();

            // Act
            Player.PickUpItem(healthPotion);

            // Assert
            Player.Inventory.Should().BeEmpty();
        }

        [Test]
        public void PickUpItem_ThatGivesHealth_HealthDoesNotExceedMaxHealth()
        {
            // Arrange
            Player.MaxHealth = 50;
            Player.CurrentHealth = 10;

            var healthPotion =
                ItemBuilder
                .Build
                .WithHeal(100)
                .AnItem();

            // Act
            Player.PickUpItem(healthPotion);

            // Assert
            Player.Inventory.Should().BeEmpty();
            Player.CurrentHealth.Should().Be(Player.MaxHealth);
        }

        [Test]
        public void PickUpItem_ThatIsRare_ASpecialEffectShouldBePlayed()
        {
            // Arrange
            Engine.Setup(x => x.PlaySpecialEffect("cool_swirly_particles")).Verifiable();

            var rareItem = ItemBuilder.Build.IsRare(true).AnItem();

            // Act
            Player.PickUpItem(rareItem);

            // Assert
            Engine.VerifyAll();
        }

        [Test]
        public void PickUpItem_ThatIsUnique_ItShouldNotBePickedUpIfThePlayerAlreadyHasIt()
        {
            // Arrange
            Player.PickUpItem(ItemBuilder.Build.WithId(100).AnItem());

            var uniqueItem = ItemBuilder
                .Build
                .WithId(100)
                .IsUnique(true)
                .AnItem();

            // Act
            var result = Player.PickUpItem(uniqueItem);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void PickUpItem_ThatDoesMoreThan500Healing_AnExtraGreenSwirlyEffectOccurs()
        {
            // Arrange
            Engine.Setup(x => x.PlaySpecialEffect("green_swirly")).Verifiable();

            var xPotion = ItemBuilder.Build.WithHeal(501).AnItem();

            // Act
            Player.PickUpItem(xPotion);

            // Assert
            Engine.VerifyAll();
        }

        [Test]
        public void PickUpItem_ThatGivesArmour_ThePlayersArmourValueShouldBeIncreased()
        {
            // Arrange
            Player.Armour.Should().Be(0);

            var armour = ItemBuilder.Build.WithArmour(100).AnItem();

            // Act
            Player.PickUpItem(armour);

            // Assert
            Player.Armour.Should().Be(100);
        }

        [Test]
        public void PickUpItem_ThatIsTooHeavy_TheItemIsNotPickedUp()
        {
            // Arrange
            var heavyItem = ItemBuilder.Build.WithWeight(Player.CarryingCapacity + 1).AnItem();

            // Act
            var result = Player.PickUpItem(heavyItem);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void PickUpItem_ThatIsRareAndUnique_AnExtraBlueSwirlyEffectOccurs()
        {
            // Arrange
            Engine.Setup(x => x.PlaySpecialEffect("blue_swirly")).Verifiable();

            var rareUniqueItem = ItemBuilder
                .Build
                .IsRare(true)
                .IsUnique(true)
                .AnItem();

            // Act
            Player.PickUpItem(rareUniqueItem);

            // Assert
            Engine.VerifyAll();
        }

        [Test]
        public void TakeDamage_AnySituation_ParticleEffectIsShown()
        {
            // Arrange
            Engine.Setup(x => x.PlaySpecialEffect("lots_of_gore")).Verifiable();

            Player.CurrentHealth = 200;

            // Act
            Player.TakeDamage(100);

            // Assert
            Engine.VerifyAll();
        }

        [Test]
        public void TakeDamage_WithNoArmour_HealthIsReduced()
        {
            // Arrange
            Player.CurrentHealth = 200;

            // Act
            Player.TakeDamage(100);

            // Assert
            Player.CurrentHealth.Should().BeLessThan(200);
        }

        [Test]
        public void TakeDamage_With50Armour_DamageIsReducedBy38()
        {
            // Arrange
            Player.PickUpItem(ItemBuilder.Build.WithArmour(50).AnItem());
            Player.CurrentHealth = 200;

            // Act
            Player.TakeDamage(100);

            // Assert
            Player.CurrentHealth.Should().Be(162);
        }

        [Test]
        public void TakeDamage_WithMoreArmourThanDamage_NoDamageIsDealtAndParryEffectIsPlayed()
        {
            // Arrange
            Engine.Setup(x => x.PlaySpecialEffect("parry")).Verifiable();

            Player.PickUpItem(ItemBuilder.Build.WithArmour(2000).AnItem());
            Player.CurrentHealth = 200;

            // Act
            Player.TakeDamage(100);

            // Assert
            Player.CurrentHealth.Should().Be(200);
            Engine.VerifyAll();
        }

        [Test]
        public void TakeDamage_CarryingUnderHalfCapacity_DamageTakenReducedByQuarter()
        {
            // Arrange
            Player.CurrentHealth = 200;

            // Act
            Player.TakeDamage(100);

            // Assert
            Player.CurrentHealth.Should().Be(125);
        }

        [Test]
        public void UseItem_ItemCausesDamage_AllEnemiesNearThePlayerAreDamaged()
        {
            // Arrange
            var enemy = new Mock<IEnemy>();
            var item = ItemBuilder
                .Build
                .WithDamage(100)
                .AnItem();

            Engine.Setup(x => x.GetEnemiesNear(Player))
                .Returns(new List<IEnemy>
                {
                    enemy.Object
                });

            // Act
            Player.UseItem(item);

            // Assert
            enemy.Verify(x => x.TakeDamage(100));
        }

        [Test]
        public void UseItem_ItemIsConsumable_ItemIsRemovedFromInventory()
        {
            // Arrange
            var item = ItemBuilder
                .Build
                .IsConsumable(true)
                .AnItem();

            Player.Inventory.Should().BeEmpty();

            // Act
            Player.PickUpItem(item);
            Player.Inventory.Should().NotBeEmpty();

            Player.UseItem(item);

            // Assert
            Player.Inventory.Should().BeEmpty();
        }
    }
}