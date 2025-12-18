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
