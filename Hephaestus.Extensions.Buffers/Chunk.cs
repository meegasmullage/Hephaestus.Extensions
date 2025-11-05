using System;
using System.Buffers;

namespace Hephaestus.Extensions.Buffers
{
    internal class Chunk : ReadOnlySequenceSegment<byte>, IDisposable
    {
        private readonly static ArrayPool<byte> s_pool = ArrayPool<byte>.Shared;

        private byte[] _buffer;
        private Memory<byte> _memory;
        private int _length;
        private Chunk _next;
        private bool _disposed;

        public Chunk(int sizeHint)
        {
            _buffer = s_pool.Rent(sizeHint);
            _memory = new Memory<byte>(_buffer);
        }

        public Memory<byte> Buffer => _memory;

        public int Capacity => _buffer.Length;

        public int Length => _length;

        public int Remaining => _buffer.Length - _length;

        public Chunk NextChunk
        {
            get => _next;
            set
            {
                _next = value;
                Next = _next;
            }
        }

        public void Advance(int count)
        {
            _length += count;

            Memory = _memory[.._length];
        }

        public void Reset()
        {
            _length = 0;

            Memory = default;

            RunningIndex = 0;
        }

        public void Reallocate(int sizeHint)
        {
            s_pool.Return(_buffer);

            _buffer = s_pool.Rent(sizeHint);
            _memory = new Memory<byte>(_buffer);
        }

        public void SetRunningIndex(Chunk prevChunk)
        {
            RunningIndex = prevChunk.RunningIndex + prevChunk.Length;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                s_pool.Return(_buffer);

                _memory = null;
                _buffer = null;
                _next = null;

                Memory = null;
                Next = null;
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
