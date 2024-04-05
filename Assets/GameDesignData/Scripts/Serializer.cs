using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public static class Serializer
{
    const string RootPath = "Assets/GameDesignData/Data";

#if UNITY_EDITOR
    /// <summary>
    /// serialize loaded table data to binary foramt using MemoryPack
    /// </summary>
    public static async Task SerializeAsync<T>(T table) where T : ITable
    {
        if (!Directory.Exists(RootPath))
        {
            Directory.CreateDirectory(RootPath);
        }

        using (FileStream fs = File.Open($"{RootPath}/{typeof(T).Name}.bin", FileMode.OpenOrCreate))
        {
            await MemoryPackSerializer.SerializeAsync<T>(fs, table);
        }
    }
#endif

    /// <summary>
    /// deserialize binary formatted table data using MemoryPack
    /// </summary>
    public static async Task<T> DeserializeAsync<T>() where T : ITable
    {
        using (FileStream fs = File.Open($"{RootPath}/{typeof(T).Name}.bin", FileMode.Open))
        {
            return await MemoryPackSerializer.DeserializeAsync<T>(fs);
        }
    }
}
