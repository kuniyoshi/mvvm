using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mvvm.Model
{
    public sealed class Party
    {
        public const int SlotCount = 5;

        private readonly Hero?[] _members = new Hero?[SlotCount];

        public event Action<IReadOnlyList<Hero?>> MembersChanged = static delegate { };

        public Party() : this(new Hero?[SlotCount])
        {
        }

        public Party(IEnumerable<Hero?> initialMembers)
        {
            var list = initialMembers.ToList();
            Debug.Assert(list.Count == SlotCount, $"Party の枠数は {SlotCount} 固定です。（実際の値: {list.Count}）");
            list.CopyTo(_members, 0);
            ValidateUniqueHeroes(_members);
        }

        public Hero? GetHero(int slotIndex)
        {
            Debug.Assert(slotIndex is >= 0 and < SlotCount, $"不正なスロット番号です。");
            return _members[slotIndex];
        }

        public PartyTransaction BeginTransaction()
        {
            return new PartyTransaction(this, _members);
        }

        internal void ApplySnapshot(Hero?[] snapshot)
        {
            Debug.Assert(snapshot.Length == SlotCount, $"Party の枠数は {SlotCount} 固定です。（実際の値: {snapshot.Length}）");
            ValidateUniqueHeroes(snapshot);
            Array.Copy(snapshot, _members, SlotCount);
            MembersChanged.Invoke(_members);

        }

        private static void ValidateUniqueHeroes(IEnumerable<Hero?> heroes)
        {
            var set = new HashSet<Hero>();
            foreach (var hero in heroes)
            {
                if (hero == null)
                {
                    continue;
                }

                if (!set.Add(hero))
                {
                    throw new InvalidOperationException("Party 内で Hero が重複しています。");
                }
            }
        }
    }
}
