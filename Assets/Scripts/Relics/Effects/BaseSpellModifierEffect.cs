using System;
using CMPM.Core;


namespace CMPM.Relics.Effects {
    public class BaseSpellModifierEffect : IRelicEffect {
        protected readonly PlayerController Player;
        protected readonly int[] Hashes;
        readonly bool _applyOnce;
        bool _hasAdded;
        
        public BaseSpellModifierEffect(in PlayerController player, in int[] hashes, bool applyOnce = true) {
            Player = player;
            Hashes = hashes ?? throw new NullReferenceException("BaseSpellModifierEffect requires a non-null hash array");
            _applyOnce = applyOnce;
        }
        
        public void ApplyEffect() {
            if (_applyOnce && _hasAdded) return;
            Player.AddBaseSpellModifiers(Hashes);
            _hasAdded = true;
        }
        
        public void RevertEffect() {
            if (!_hasAdded) return;
            Player.RemoveBaseSpellModifiers(Hashes);
            _hasAdded = false;
        }

        public bool CanCancel() => _hasAdded;
    }
}