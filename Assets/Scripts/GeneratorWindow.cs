using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class GeneratorWindow : EditorWindow
{
    [SerializeField]
    TextAsset _fileToParse;

    [SerializeReference]
    ParsedHeader _parsedHeader;

    [SerializeReference]
    ITable _table;

    SerializedObject _serializedObject;

    [MenuItem("Window/GeneratorWindow")]
    static void Init()
    {
        GetWindow<GeneratorWindow>().Show();
    }

    void OnEnable()
    {
        _serializedObject = new SerializedObject(this);
    }

    void OnDisable()
    {

    }

    void OnGUI()
    {
        var fileToParseProp = _serializedObject.FindProperty("_fileToParse");
        var parsedHeaderProp = _serializedObject.FindProperty("_parsedHeader");
        var tableProp = _serializedObject.FindProperty("_table");

        EditorGUILayout.ObjectField(fileToParseProp);

        EditorGUILayout.BeginHorizontal();

        GUI.enabled = fileToParseProp.objectReferenceValue != null;
        if (GUILayout.Button("Parse"))
        {
            var file = fileToParseProp.objectReferenceValue as TextAsset;
            var extension = Path.GetExtension(AssetDatabase.GetAssetPath(file));

            Parser parser = extension switch
            {
                ".csv" => new CSVParser(file)
            };

            parsedHeaderProp.managedReferenceValue = parser.ParseHeader();
        }

        if (GUILayout.Button("Generate"))
        {
            Generate(parsedHeaderProp.managedReferenceValue as ParsedHeader);
        }

        var parsedHeader = parsedHeaderProp.managedReferenceValue as ParsedHeader;
        var tableType = parsedHeader != null ? Type.GetType($"Table{parsedHeader.Name}") : null;

        GUI.enabled = GUI.enabled && parsedHeader != null && tableType != null;
        if (GUILayout.Button("Convert"))
        {
            var file = fileToParseProp.objectReferenceValue as TextAsset;
            var extension = Path.GetExtension(AssetDatabase.GetAssetPath(file));

            Parser parser = extension switch
            {
                ".csv" => new CSVParser(file)
            };

            tableProp.managedReferenceValue = tableType.GetConstructor(new Type[] { typeof(List<>).MakeGenericType(Type.GetType(parsedHeader.Name)) }).Invoke(new object[] { parser.ParseBody() }) as ITable;

            var obj = tableProp.managedReferenceValue;
            var list = obj.GetType().GetField("_data", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj) as List<AccountExp>;

            foreach (var element in list)
            {
                Debug.Log(element.Key);
                Debug.Log(element.Level);
                Debug.Log(element.NeedExp);
            }
        }
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;

        if (parsedHeaderProp.managedReferenceValue != null)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Parsed Header", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(parsedHeaderProp.FindPropertyRelative("_name"));
            EditorGUILayout.PropertyField(parsedHeaderProp.FindPropertyRelative("_desc"));

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Parsed Members", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", GUILayout.Width(150));
            EditorGUILayout.LabelField("Type", GUILayout.Width(100));
            EditorGUILayout.LabelField("Desc");
            EditorGUILayout.EndHorizontal();
            var membersProp = parsedHeaderProp.FindPropertyRelative("_members");
            for (int i = 0; i < membersProp.arraySize; i++)
            {
                var memberProp = membersProp.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(memberProp.FindPropertyRelative("_name"), GUIContent.none, GUILayout.Width(150));
                EditorGUILayout.PropertyField(memberProp.FindPropertyRelative("_type"), GUIContent.none, GUILayout.Width(100));
                EditorGUILayout.PropertyField(memberProp.FindPropertyRelative("_desc"), GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }
        }

        if (tableProp.managedReferenceValue != null)
        {
            
        }

        _serializedObject.ApplyModifiedProperties();
        _serializedObject.Update();
    }

    static void Generate(ParsedHeader header)
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

        var path = "Assets/Scripts/DesignData.cs";

        File.WriteAllText(path, content);
        AssetDatabase.ImportAsset(path);
    }

    static string GenerateClass(string[] usings, string name, string desc, string[] attributes, string body)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < usings.Length; i++)
        {
            sb.Append($"using {usings[i]};\n");
        }
        sb.Append("\n");
        sb.Append($"/// <summary>\n/// {desc}\n/// </summary>\n");
        for (int i = 0; i < attributes.Length; i++)
        {
            sb.Append($"[{attributes[i]}]\n");
        }
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
