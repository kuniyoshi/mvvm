using System;
using System.Collections.Generic;
using System.Linq;

namespace Mvvm.Model
{
    public sealed class Party
    {
        public const int SlotCount = 4;

        private readonly Hero?[] _members = new Hero?[SlotCount];

        public event Action<IReadOnlyList<Hero?>> MembersChanged = delegate { };

        public Party()
        {
        }

        public Party(IEnumerable<Hero?> initialMembers)
        {
            if (initialMembers == null)
            {
                throw new ArgumentNullException(nameof(initialMembers));
            }

            var list = initialMembers.ToList();
            if (list.Count != SlotCount)
            {
                throw new ArgumentException($"Party には {SlotCount} 枠が必要です。", nameof(initialMembers));
            }

            list.CopyTo(_members, 0);
            ValidateUniqueHeroes(_members);
        }

        public Hero? GetHero(int slotIndex)
        {
            ValidateSlotIndex(slotIndex);
            return _members[slotIndex];
        }

        public PartyTransaction BeginTransaction()
        {
            return new PartyTransaction(this, _members);
        }

        internal void ApplySnapshot(Hero?[] snapshot)
        {
            if (snapshot.Length != SlotCount)
            {
                throw new ArgumentException("不正なスナップショットです。", nameof(snapshot));
            }

            ValidateUniqueHeroes(snapshot);
            Array.Copy(snapshot, _members, SlotCount);
            MembersChanged?.Invoke(_members);
        }

        public static void ValidateSlotIndex(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SlotCount)
            {
                throw new ArgumentOutOfRangeException(nameof(slotIndex), $"slotIndex は 0 から {SlotCount - 1} で指定してください。");
            }
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
