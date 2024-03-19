using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVParser : Parser
{
    static readonly Vector2Int TableNamePos = new Vector2Int(0, 1);
    static readonly Vector2Int TableDescPos = new Vector2Int(1, 1);
    const int MemberTypeRow = 4;
    const int MemberDescRow = 5;
    const int MemberNameRow = 6;
    const int MemberDataStartRow = 7;
    const int MemberDataStartColumn = 1;

    TextAsset _file;
    string _path;

    public CSVParser(string path)
    {
        _path = path;
    }

    public CSVParser(TextAsset file)
    {
        _file = file;
    }

    public override ParsedHeader ParseHeader()
    {
        return ParseHeader(Parse());
    }

    public override IList ParseBody()
    {
        var parsed = Parse();

        ParsedHeader header = ParseHeader(parsed);

        var type = Type.GetType(header.Name);
        var memberTypes = new Type[header.Members.Count];
        for (int i = 0; i < header.Members.Count; i++)
        {
            memberTypes[i] = GetType(header.Members[i].Type);
        }
        var memberValues = new object[header.Members.Count];

        IList list = (IList)typeof(List<>).MakeGenericType(type).GetConstructor(new Type[0]).Invoke(new object[0]);

        // TODO :
        // remove last line
        //
        for (int r = MemberDataStartRow; r < parsed.Length - 1; r++)
        {
            for (int c = 0; c < header.Members.Count; c++)
            {
                var value = parsed[r][c + MemberDataStartColumn];

                memberValues[c] = header.Members[c].Type switch
                {
                    "string" => value,
                    "int" => int.Parse(value),
                    "float" => float.Parse(value),
                };
            }

            list.Add(type.GetConstructor(memberTypes).Invoke(memberValues));
        }

        return list;
    }

    ParsedHeader ParseHeader(string[][] file)
    {
        var types = file[MemberTypeRow];
        var descs = file[MemberDescRow];
        var names = file[MemberNameRow];

        var members = new List<ParsedMember>();

        for (int i = MemberDataStartColumn; i < types.Length; i++)
        {
            if (!string.IsNullOrEmpty(types[i]))
            {
                int index = names[i].IndexOf('[');
                if (index > 0)
                {
                    names[i] = names[i].Substring(0, index);
                }

                members.Add(new ParsedMember(names[i], types[i], descs[i]));
            }
        }

        return new ParsedHeader(file[TableNamePos.x][TableNamePos.y], file[TableDescPos.x][TableDescPos.y], members);
    }

    string[][] Parse()
    {
        var text = _file != null ? _file.text : File.ReadAllText(_path);

        var rows = text.Split('\n');
        var parsed = new string[rows.Length][];

        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].Length > 0)
            {
                rows[i] = rows[i].Substring(0, rows[i].Length - 1);
                parsed[i] = rows[i].Split(',');
            }
        }

        return parsed;
    }
}
