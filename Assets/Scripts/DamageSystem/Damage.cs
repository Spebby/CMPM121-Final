namespace CMPM.DamageSystem {
    public class Damage {
        public readonly int Amount;

        public enum Type {
            PHYSICAL,
            ARCANE,
            NATURE,
            FIRE,
            ICE,
            DARK,
            LIGHT
        }

        // ReSharper disable once InconsistentNaming
        public Type type;

        public Damage(int amount, Type type) {
            Amount    = amount;
            this.type = type;
        }

        public static Type TypeFromString(string type) {
            string t = type.ToLower();
            return t switch {
                "arcane" => Type.ARCANE,
                "nature" => Type.NATURE,
                "fire"   => Type.FIRE,
                "ice"    => Type.ICE,
                "dark"   => Type.DARK,
                "light"  => Type.LIGHT,
                _        => Type.PHYSICAL
            };
        }
    }
}