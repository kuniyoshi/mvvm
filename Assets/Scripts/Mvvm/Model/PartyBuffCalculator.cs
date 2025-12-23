using System;
using System.Collections.Generic;
using System.Linq;

namespace Mvvm.Model
{
    public static class PartyBuffCalculator
    {
        private const int BuffAmount = 10;

        public static IReadOnlyList<HeroStats?> Calculate(IReadOnlyList<Hero?> members)
        {
            if (members.Count != Party.SlotCount)
            {
                throw new InvalidOperationException(
                    $"Party の枠数は {Party.SlotCount} 固定です。（実際の値: {members.Count}）");
            }

            var nearCount = members.Count(hero => hero?.Position == Position.Near);
            var middleCount = members.Count(hero => hero?.Position == Position.Middle);
            var farCount = members.Count(hero => hero?.Position == Position.Far);

            var result = new HeroStats?[members.Count];
            for (var i = 0; i < members.Count; i++)
            {
                var hero = members[i];
                if (hero == null)
                {
                    result[i] = null;
                    continue;
                }

                var nearBonus = hero.Position == Position.Near ? middleCount * BuffAmount : 0;
                var hp = hero.Hp + farCount * BuffAmount + nearBonus;
                var attack = hero.Attack + nearCount * BuffAmount + nearBonus;
                result[i] = new HeroStats(hp, attack);
            }

            return result;
        }
    }
}
