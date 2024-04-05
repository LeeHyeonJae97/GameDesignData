using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    async void Awake()
    {
        var table = await DesignDataManager.LoadAsync<TableAccountExp>();

        foreach (var datum in table.GetData())
        {
            Debug.Log(datum.Level1.Length);
        }
    }
}
