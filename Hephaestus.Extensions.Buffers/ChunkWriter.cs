using System;
using System.Buffers;

namespace Hephaestus.Extensions.Buffers
{
    public class ChunkWriter : IBufferWriter<byte>, IDisposable
    {
        private Chunk _head;
        private Chunk _tail;
        private long _length;
        private long _capacity;
        private bool _disposed;

        public long Capacity => _capacity;

        public long Length => _length;

        private Memory<byte> GetMemoryCore(int sizeHint)
        {
            if (_head == null)
            {
                _head = new Chunk(sizeHint);

                _tail = _head;

                _capacity += _tail.Capacity;
            }

            if (_tail.Remaining >= sizeHint)
            {
                return _tail.Buffer.Slice(_tail.Length, sizeHint);
            }

            var nextChunk = _tail.NextChunk;

            if (nextChunk == null)
            {
                nextChunk = new Chunk(sizeHint);

                _tail.NextChunk = nextChunk;

                _capacity += nextChunk.Capacity;
            }
            else
            {
                if (nextChunk.Remaining < sizeHint)
                {
                    nextChunk.Reallocate(sizeHint);
                }
            }

            nextChunk.SetRunningIndex(_tail);

            _tail = nextChunk;

            return _tail.Buffer.Slice(_tail.Length, sizeHint);
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            return GetMemoryCore(sizeHint);
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            return GetMemoryCore(sizeHint).Span;
        }

        public void Advance(int count)
        {
            _tail.Advance(count);
            _length += count;
        }

        public void Reset()
        {
            _length = 0;

            var chunk = _head;

            while (chunk != null)
            {
                chunk.Reset();
                chunk = chunk.NextChunk;
            }

            _tail = _head;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                var chunk = _head;

                while (chunk != null)
                {
                    var nextChunk = chunk.NextChunk;

                    chunk.Dispose();

                    chunk = nextChunk;
                }

                _head = null;
                _tail = null;
                _capacity = 0;
                _length = 0;
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public ReadOnlySequence<byte> Buffer => new(_head, 0, _tail, _tail.Length);
    }
}
