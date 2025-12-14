using System;
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
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException("HeroPreset の Name は必須です。");
            }

            if (Hp <= 0)
            {
                throw new InvalidOperationException("HeroPreset の Hp は 1 以上で設定してください。");
            }

            if (Attack <= 0)
            {
                throw new InvalidOperationException("HeroPreset の Attack は 1 以上で設定してください。");
            }

            return new Hero(Name, Hp, Attack, Position);
        }
    }
}
