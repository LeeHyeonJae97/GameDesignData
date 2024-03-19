using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using UnityEngine;

public static class Serializer
{
    public static async Task SerializeAsync(ITable table)
    {
        using (FileStream fs = File.Open("Assets/Table/Serialized.bin", FileMode.OpenOrCreate))
        {
            PipeWriter writer = PipeWriter.Create(fs);

            MemoryPackSerializer.Serialize<ITable>(writer, table);
            await writer.FlushAsync();
        }
    }

    public static async Task<ITable> DeserializeAsync()
    {
        using (FileStream fs = File.Open("Assets/Table/Serialized.bin", FileMode.Open))
        {
            PipeReader reader = PipeReader.Create(fs);

            await reader.ReadAsync();
            if (reader.TryRead(out var result))
            {
                return MemoryPackSerializer.Deserialize<ITable>(result.Buffer);
            }
            else
            {
                return null;
            }
        }
    }
}
