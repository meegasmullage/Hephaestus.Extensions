# Hephaestus.Extensions

A collection of useful extension libraries for .NET.

This repository is split into several packages:


### Hephaestus.Extensions.Buffers

Implementation of `IBufferWriter<byte>` backed by `ArrayPool<byte>`. The buffer is exposed via a `ReadOnlySequence<byte>`.

### Hephaestus.Extensions.Buffers.Json

Implementation of `IBufferWriter<byte>` to support `Utf8JsonWriter` and `Utf8JsonReader`, backed by `Hephaestus.Extensions.Buffers`.