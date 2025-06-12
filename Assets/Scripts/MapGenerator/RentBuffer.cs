using System;
using System.Buffers;


namespace CMPM.MapGenerator {
    internal readonly struct RentBuffer<T> : IDisposable {
        public readonly T[] Buffer;
        public readonly int Count;
        public RentBuffer(T[] buf, int cnt) { Buffer = buf; Count = cnt; }
        public void Dispose() => ArrayPool<T>.Shared.Return(Buffer); 
    }
}