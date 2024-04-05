using MemoryPack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// should add MemoryPackUnion attribute for all table class inherits ITable
[MemoryPackable]
[MemoryPackUnion(1, typeof(TableAccountExp))]
[MemoryPackUnion(2, typeof(TablePcStory))]
public partial interface ITable
{

}

// have to make table classes for every design data classes like this
[MemoryPackable]
public partial class TableAccountExp : ITable
{
    // should have Dictionary or List type of private field with name of '_data'
    [MemoryPackInclude]
    Dictionary<string, AccountExp> _data;

    [MemoryPackConstructor]
    public TableAccountExp()
    {

    }

    // constructor for binarizing
    public TableAccountExp(List<AccountExp> data)
    {
        _data = data.ToDictionary((datum) => datum.Key1);
    }

    // add any functions to get data with conditions from table
    public List<AccountExp> GetData()
    {
        return _data.Values.ToList();
    }
}

// have to make table classes for every design data classes like this
[MemoryPackable]
public partial class TablePcStory : ITable
{
    // should have Dictionary or List type of private field with name of '_data'
    [MemoryPackInclude]
    Dictionary<string, PcStory> _data;

    [MemoryPackConstructor]
    public TablePcStory()
    {

    }

    // constructor for binarizing
    public TablePcStory(List<PcStory> data)
    {
        _data = data.ToDictionary((datum) => datum.Key);
    }

    // add any functions to get data with conditions from table
    public List<PcStory> GetData()
    {
        return _data.Values.ToList();
    }
}
