#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Parser
{
    // cell position in .csv
    static readonly Vector2Int TableNamePos = new Vector2Int(0, 1);
    static readonly Vector2Int TableDescPos = new Vector2Int(1, 1);
    const int MemberTypeRow = 4;
    const int MemberDescRow = 5;
    const int MemberNameRow = 6;
    const int MemberDataStartRow = 7;
    const int MemberDataStartColumn = 1;

    TextAsset _file;
    string _path;

    public Parser(TextAsset file)
    {
        _file = file;
    }

    public Parser(string path)
    {
        _path = path;
    }

    public List<ParsedHeader> ParseEnums()
    {
        var parsed = Parse();

        var descs = parsed[MemberDescRow];
        var names = parsed[MemberNameRow];

        var headers = new List<ParsedHeader>();

        for (int c = MemberDataStartColumn; c < names.Length - 1; c += 2)
        {
            var members = new List<ParsedMember>();

            for (int r = MemberDataStartRow; r < parsed.Length; r++)
            {
                var memberName = parsed[r][c];
                var memberDesc = parsed[r][c + 1];

                if (!string.IsNullOrEmpty(memberName))
                {
                    members.Add(new ParsedMember(memberName, null, memberDesc));
                }
            }

            headers.Add(new ParsedHeader(names[c], descs[c], members));
        }

        return headers;
    }

    public ParsedHeader ParseHeader()
    {
        var parsed = Parse();

        var types = parsed[MemberTypeRow];
        var descs = parsed[MemberDescRow];
        var names = parsed[MemberNameRow];

        var members = new List<ParsedMember>();

        for (int c = MemberDataStartColumn; c < types.Length; c++)
        {
            if (!string.IsNullOrEmpty(types[c]))
            {
                members.Add(new ParsedMember(names[c], types[c], descs[c]));
            }
        }

        return new ParsedHeader(parsed[TableNamePos.x][TableNamePos.y], parsed[TableDescPos.x][TableDescPos.y], members);
    }

    public IList ParseBody()
    {
        var parsed = this.Parse();

        var name = parsed[TableNamePos.x][TableNamePos.y];
        var type = Type.GetType(name);

        IList list = (IList)typeof(List<>).MakeGenericType(type).GetConstructor(new Type[0]).Invoke(new object[0]);

        for (int r = MemberDataStartRow; r < parsed.Length; r++)
        {
            var memberTypes = new List<Type>();
            var memberValues = new List<object>();

            for (int c = MemberDataStartColumn; c < parsed[MemberTypeRow].Length; c++)
            {
                var memberType = parsed[MemberTypeRow][c];

                if (!string.IsNullOrEmpty(memberType))
                {
                    memberTypes.Add(Utils.GetType(memberType));

                    if (!memberType.Contains("[]"))
                    {
                        memberValues.Add(Parse(memberType, parsed[r][c]));
                    }
                    else
                    {
                        int index = c;

                        var values = new List<string>() { parsed[r][c] };
                        while (string.IsNullOrEmpty(parsed[MemberTypeRow][++c]))
                        {
                            values.Add(parsed[r][c]);
                        }
                        c--;

                        var array = Array.CreateInstance(Utils.GetType(memberType.Replace("[]", "")), values.Count);
                        for (int i = index; i < index + values.Count; i++)
                        {
                            array.SetValue(Parse(memberType.Replace("[]", ""), parsed[r][i]), i - index);
                        }

                        memberValues.Add(array);
                    }
                }
            }
            list.Add(type.GetConstructor(memberTypes.ToArray()).Invoke(memberValues.ToArray()));
        }

        return list;

        object Parse(string type, string value)
        {
            return type switch
            {
                // add types to support if need
                "string" => value,
                "int" => int.Parse(value),
                "float" => float.Parse(value),
                "double" => double.Parse(value),
                "bool" => bool.Parse(value),
                _ => ParseEnum(type, value),
            };

            object ParseEnum(string type, string value)
            {
                // parse int to enum
                if (int.TryParse(value, out var result))
                {
                    return result;
                }

                // parse string to enum
                else
                {
                    return Enum.Parse(Type.GetType(type), value);
                }
            }
        }
    }

    string[][] Parse()
    {
        var text = _file != null ? _file.text : File.ReadAllText(_path);

        var rows = text.Split('\n');
        var parsed = new string[rows.Length][];

        // skip last row because it's empty
        for (int i = 0; i < rows.Length - 1; i++)
        {
            if (rows[i].Length > 0)
            {
                parsed[i] = rows[i][0..^1].Split(',');
            }
        }

        return parsed;
    }
}

[Serializable]
public class ParsedHeader
{
    public string Name { get; private set; }

    /// <summary>
    /// description value used for 'summary' tag in visual studio (may not work for other ide or code editor)
    /// </summary>
    public string Desc { get; private set; }

    public List<ParsedMember> Members { get; private set; }

    public ParsedHeader(string name, string desc, List<ParsedMember> members)
    {
        Name = name;
        Desc = desc;
        Members = members;
    }
}

[Serializable]
public class ParsedMember
{
    public string Name { get; private set; }

    public string Type { get; private set; }

    /// <summary>
    /// description value used for 'summary' tag in visual studio (may not work for other ide or code editor)
    /// </summary>
    public string Desc { get; private set; }

    public ParsedMember(string name, string type, string desc)
    {
        Name = name;
        Type = type;
        Desc = desc;
    }
}
#endif
