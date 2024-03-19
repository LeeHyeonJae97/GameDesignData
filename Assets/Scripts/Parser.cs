using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Parser
{
    public abstract ParsedHeader ParseHeader();

    public abstract IList ParseBody();

    protected Type GetType(string type)
    {
        return type switch
        {
            // TODO :
            // convert types
            //
            "string" => typeof(string),
            "int" => typeof(int),
            "float" => typeof(float),
        };
    }
}

[Serializable]
public class ParsedHeader
{
    public string Name { get { return _name; } }

    public string Desc { get { return _desc; } }

    public List<ParsedMember> Members { get { return _members; } }

    [SerializeField]
    string _name;

    [SerializeField]
    string _desc;

    [SerializeField]
    List<ParsedMember> _members;

    public ParsedHeader(string name, string desc, List<ParsedMember> members)
    {
        _name = name;
        _desc = desc;
        _members = members;
    }
}

[Serializable]
public class ParsedMember
{
    public string Name { get { return _name; } }

    public string Type { get { return _type; } }

    public string Desc { get { return _desc; } }

    [SerializeField]
    string _name;

    [SerializeField]
    string _type;

    [SerializeField]
    string _desc;

    public ParsedMember(string name, string type, string desc)
    {
        _name = name;
        _type = type;
        _desc = desc;
    }
}
