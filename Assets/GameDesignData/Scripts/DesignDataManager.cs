using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DesignDataManager
{
    static Dictionary<string, ITable> _tables;

    /// <summary>
    /// get loaded table
    /// </summary>
    public static T Get<T>() where T : ITable
    {
        return (T)_tables[typeof(T).ToString()];
    }

    /// <summary>
    /// load table
    /// </summary>
    public static async Task<T> LoadAsync<T>() where T : ITable
    {
        return await Serializer.DeserializeAsync<T>();
    }
    
    /// <summary>
    /// unload table
    /// </summary>
    public static void Unload<T>() where T : ITable
    {
        _tables.Remove(typeof(T).ToString());
    }
}
