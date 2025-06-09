using System;
using System.Buffers;


namespace CMPM146.MapGenerator {
    internal readonly struct Slice<T> : IDisposable {
        public readonly T[] Buffer;
        public readonly int Count;
        public Slice(T[] buf, int cnt) { Buffer = buf; Count = cnt; }
        public void Dispose() => ArrayPool<T>.Shared.Return(Buffer); 
    }
}