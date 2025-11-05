using System.Text.Json;

namespace Hephaestus.Extensions.Buffers.Json
{
    public class JsonChunkWriter : ChunkWriter
    {
        public void Serialize<T>(T value, JsonSerializerOptions serializerOptions = null)
        {
            using (var utf8JsonWriter = new Utf8JsonWriter(this))
            {
                JsonSerializer.Serialize(utf8JsonWriter, value, serializerOptions);
            }
        }

        public T Deserialize<T>(JsonSerializerOptions serializerOptions = null)
        {
            var utf8JsonReader = new Utf8JsonReader(Buffer);

            return JsonSerializer.Deserialize<T>(ref utf8JsonReader, serializerOptions);
        }
    }
}
