using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mvvm.Model
{
    public sealed class PartyTransaction
    {
        private readonly Party _party;
        private readonly Hero?[] _originalSnapshot;
        private readonly Hero?[] _workingSnapshot;
        private bool _completed;

        internal PartyTransaction(Party party, Hero?[] members)
        {
            _party = party;
            _originalSnapshot = new Hero?[Party.SlotCount];
            _workingSnapshot = new Hero?[Party.SlotCount];
            Array.Copy(members, _originalSnapshot, Party.SlotCount);
            Array.Copy(members, _workingSnapshot, Party.SlotCount);
        }

        public IReadOnlyList<Hero?> Members => _workingSnapshot;

        public bool HasChanges => !_originalSnapshot.SequenceEqual(_workingSnapshot);

        public Hero? GetHero(int slotIndex)
        {
            Debug.Assert(slotIndex is >= 0 and < Party.SlotCount, $"Invalid slot index: {slotIndex}");
            return _workingSnapshot[slotIndex];
        }

        public bool Contains(Hero hero)
        {
            return _workingSnapshot.Any(h => ReferenceEquals(h, hero));
        }

        public void AssignHero(int slotIndex, Hero? hero)
        {
            Debug.Assert(slotIndex is >= 0 and < Party.SlotCount, $"Invalid slot index: {slotIndex}");
            AssertActive();

            if (hero == null)
            {
                _workingSnapshot[slotIndex] = null;
                return;
            }

            var duplicateIndex = Array.FindIndex(_workingSnapshot, h => ReferenceEquals(h, hero));
            if (duplicateIndex >= 0 && duplicateIndex != slotIndex)
            {
                _workingSnapshot[duplicateIndex] = null;
            }

            _workingSnapshot[slotIndex] = hero;
        }

        public void ClearSlot(int slotIndex)
        {
            AssignHero(slotIndex, null);
        }

        public void Commit()
        {
            AssertActive();
            var snapshot = new Hero?[Party.SlotCount];
            Array.Copy(_workingSnapshot, snapshot, Party.SlotCount);
            _party.ApplySnapshot(snapshot);
            _completed = true;
        }

        public void Rollback()
        {
            AssertActive();
            _completed = true;
        }

        private void AssertActive()
        {
            Debug.Assert(!_completed, "トランザクションは既に完了しています。");
        }
    }
}
