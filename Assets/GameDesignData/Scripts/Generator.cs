#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class Generator
{
    const string RootPath = "Assets/GameDesignData/Scripts/Data";

    public static void Generate(ParsedHeader header)
    {
        StringBuilder body = new StringBuilder();

        for (int i = 0; i < header.Members.Count; i++)
        {
            GenerateMember(body, header.Members[i].Type, header.Members[i].Name, header.Members[i].Desc);
        }

        GenerateConstructor(body, header.Name, header.Members.Select((m) => m.Type).ToArray(), header.Members.Select((m) => m.Name).ToArray());

        var usings = new string[] { "MemoryPack", "System", "UnityEngine" };
        var attributes = new string[] { "MemoryPackable" };

        string content = GenerateClass(usings, header.Name, header.Desc, attributes, body.ToString());

        var path = $"{RootPath}/{header.Name}.cs";

        File.WriteAllText(path, content);
        AssetDatabase.ImportAsset(path);

        Debug.Log($"Generate {header.Name}");
    }

    public static void GenerateEnums(List<ParsedHeader> headers)
    {
        StringBuilder content = new StringBuilder();

        foreach (var header in headers)
        {
            StringBuilder body = new StringBuilder();

            for (int i = 0; i < header.Members.Count; i++)
            {
                var name = header.Members[i].Name;
                var desc = header.Members[i].Desc;

                if (!string.IsNullOrEmpty(desc))
                {
                    body.Append($"\t/// <summary>\n\t/// {desc}\n\t/// </summary>\n");
                }
                body.Append($"\t{name},\n\n");
            }

            content.Append("\n");
            content.Append($"/// <summary>\n/// {header.Desc}\n/// </summary>\n");
            content.Append($"public enum {header.Name}\n{{\n{body}}}\n");
        }

        var path = $"{RootPath}/Enums.cs";

        File.WriteAllText(path, content.ToString());
        AssetDatabase.ImportAsset(path);

        Debug.Log("Generate Enums");
    }

    static string GenerateClass(string[] usings, string name, string desc, string[] attributes, string body)
    {
        StringBuilder sb = new StringBuilder();

        if (usings != null)
        {
            for (int i = 0; i < usings.Length; i++)
            {
                sb.Append($"using {usings[i]};\n");
            }
        }
        sb.Append("\n");
        sb.Append($"/// <summary>\n/// {desc}\n/// </summary>\n");
        if (attributes != null)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                sb.Append($"[{attributes[i]}]\n");
            }
        }

        // create partial class to use with MemoryPack
        sb.Append($"public partial class {name}\n{{\n{body}}}\n");

        return sb.ToString();
    }

    static void GenerateMember(StringBuilder sb, string type, string name, string desc)
    {
        sb.Append($"\t/// <summary>\n\t/// {desc}\n\t/// </summary>\n");
        sb.Append($"\tpublic {type} {name} {{ get; private set; }}\n\n");
    }

    static void GenerateConstructor(StringBuilder sb, string name, string[] memberTypes, string[] memberNames)
    {
        sb.Append($"\tpublic {name}(");
        for (int i = 0; i < memberTypes.Length; i++)
        {
            sb.Append($"{memberTypes[i]} {char.ToLower(memberNames[i][0])}{memberNames[i].Substring(1)}, ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(")\n\t{\n");
        for (int i = 0; i < memberNames.Length; i++)
        {
            sb.Append($"\t\t{memberNames[i]} = {char.ToLower(memberNames[i][0])}{memberNames[i].Substring(1)};\n");
        }
        sb.Append("\t}\n");
    }
}
#endif
