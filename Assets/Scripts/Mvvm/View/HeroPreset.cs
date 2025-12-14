using System;
using System.Collections.Generic;
using Mvvm.Model;

namespace Mvvm.View
{
    [Serializable]
    public struct HeroPreset
    {
        public string Name;
        public int Hp;
        public int Attack;
        public Position Position;

        public Hero ToHero()
        {
            var safeName = string.IsNullOrWhiteSpace(Name) ? "Hero" : Name;
            var safeHp = Math.Max(1, Hp);
            var safeAttack = Math.Max(1, Attack);
            return new Hero(safeName, safeHp, safeAttack, Position);
        }

        public static List<HeroPreset> CreateDefaultPresets()
        {
            return new List<HeroPreset>
            {
                new HeroPreset { Name = "Satoka", Hp = 120, Attack = 15, Position = Position.Near },
                new HeroPreset { Name = "Io", Hp = 100, Attack = 18, Position = Position.Near },
                new HeroPreset { Name = "Tsubame", Hp = 90, Attack = 20, Position = Position.Middle },
                new HeroPreset { Name = "Yumi", Hp = 70, Attack = 25, Position = Position.Far },
                new HeroPreset { Name = "Mana", Hp = 80, Attack = 12, Position = Position.Far }
            };
        }
    }
}
