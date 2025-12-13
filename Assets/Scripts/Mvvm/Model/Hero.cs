using System;

namespace Mvvm.Model
{
    public sealed class Hero
    {
        public string Name { get; }
        public int Hp { get; }
        public int Attack { get; }
        public Position Position { get; }

        public Hero(string name, int hp, int attack, Position position)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("名前は必須です。", nameof(name));
            }

            if (hp <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(hp), "HP は 1 以上である必要があります。");
            }

            if (attack <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attack), "攻撃力は 1 以上である必要があります。");
            }

            Name = name;
            Hp = hp;
            Attack = attack;
            Position = position;
        }

        public override string ToString()
        {
            return $"{Name} (HP:{Hp} ATK:{Attack} {Position})";
        }
    }
}
