using System;
using System.Buffers;


namespace CMPM146.MapGenerator {
    internal ref struct WeightedBag<T> {
        readonly T[] _items;
        readonly ReadOnlySpan<int> _weights;
        readonly int[] _indices;
        int _count;
        int _weightSum;

        public WeightedBag(in T[] items, in ReadOnlySpan<int> weights) {
            _items   = items;
            _weights = weights;
            _indices = ArrayPool<int>.Shared.Rent(_items.Length);
            for (int i = 0; i < _items.Length; i++) _indices[i] = i;
            
            _count     = _items.Length;
            _weightSum = 0;
            foreach (int w in weights) _weightSum += w;
        }

        public bool TryNext(Random rng, out T result) {
            if (_count == 0) {
                result = default;
                return false;
            }

            int pick         = rng.Next(_weightSum);
            float cumulative = 0f;

            for (int i = 0; i < _count; ++i) {
                int index = _indices[i];
                cumulative += _weights[index];
                if (!(pick <= cumulative)) continue;
                result     =  _items[index];
                _weightSum -= _weights[index];

                // Swap and shrink
                _count--;
                _indices[i] = _indices[_count];
                return true;
            }

            result = default;
            return false;
        }
        
        public void Dispose() => ArrayPool<int>.Shared.Return(_indices);
    }
}