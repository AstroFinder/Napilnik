using System;

namespace Napilnik
{
    public class Player
    {
        public int Health { get; private set; }

        public bool IsAlive => Health > 0;

        public Player(int health)
        {
            if (health <= 0)
                throw new ArgumentOutOfRangeException(nameof(health));

            Health = health;
        }

        public void Damage(int damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage));

            if (IsAlive)
                Health -= damage;
        }
    }

    public class Bot
    {
        private readonly Weapon _weapon;

        public Bot(Weapon weapon)
        {
            _weapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
        }

        public void OnSeePlayer(Player player)
        {
            player = player ?? throw new ArgumentNullException(nameof(player));

            if (_weapon.CanFire)
                _weapon.Fire(player);
        }
    }

    public class Weapon
    {
        public int Damage { get; private set; }
        public int Bullets { get; private set; }
        public bool CanFire => Bullets > 0;

        public Weapon(int damage, int bullets)
        {
            Damage = damage;
            Bullets = bullets;
        }

        public void Fire(Player player)
        {
            if (CanFire == false)
                throw new InvalidOperationException();

            player.Damage(Damage);

            Bullets -= 1;
        }
    }
}
