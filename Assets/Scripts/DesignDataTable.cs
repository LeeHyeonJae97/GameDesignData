using MemoryPack;
using System.Collections;
using System.Collections.Generic;

[MemoryPackable]
[MemoryPackUnion(0, typeof(TableAccountExp))]
public partial interface ITable
{

}

[MemoryPackable]
public partial class TableAccountExp : ITable
{
    List<AccountExp> _data;

    [MemoryPackConstructor]
    public TableAccountExp()
    {

    }

    public TableAccountExp(List<AccountExp> data)
    {
        _data = new List<AccountExp>(data);
        _data.Sort((l, r) => l.Level.CompareTo(r.Level));
    }

    /// <summary>
    /// 현재 레벨의 AccountExp를 반환
    /// </summary>
    public bool TryGetValue(int level, out AccountExp value)
    {
        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].Level == level)
            {
                value = _data[i];
                return true;
            }
        }

        value = null;
        return false;
    }

    /// <summary>
    /// 현재 레벨의 AccountExp와 다음 레벨의 AccountExp를 함께 반환
    /// </summary>
    public bool TryGetValue(int level, out AccountExp currentValue, out AccountExp nextValue)
    {
        currentValue = null;
        nextValue = null;

        for (int i = 0; i < _data.Count; i++)
        {
            if (_data[i].Level == level)
            {
                currentValue = _data[i];
            }
            else if (_data[i].Level == level + 1)
            {
                nextValue = _data[i];
            }
        }

        return currentValue != null && nextValue != null;
    }
}
