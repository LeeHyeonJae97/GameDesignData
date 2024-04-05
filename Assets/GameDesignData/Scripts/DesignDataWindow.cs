#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class DesignDataWindow : EditorWindow
{
    const string RootPath = "DesignData";
    
    SerializedObject _serializedObject;
    ITable _table;
    Vector2 _scrollList;
    Vector2 _scrollParser;
    string _searchData;

    [MenuItem("Window/Design Data")]
    static void Init()
    {
        GetWindow<DesignDataWindow>().Show();
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
        EditorGUILayout.BeginHorizontal();
        DrawDesignDataList();
        DrawDesignData();
        EditorGUILayout.EndHorizontal();

        Refresh();
    }

    void DrawDesignDataList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(225));
        if (GUILayout.Button("Open Root Path"))
        {
            EditorUtility.RevealInFinder(RootPath);
        }
        if (GUILayout.Button("Generate Enums"))
        {
            Generator.GenerateEnums(new Parser($"{RootPath}/Enums.csv").ParseEnums());
        }
        if (GUILayout.Button("Generate All"))
        {
            foreach (var filePath in Directory.GetFiles(RootPath))
            {
                if (filePath.Contains("Enums.csv"))
                {
                    Generator.GenerateEnums(new Parser(filePath).ParseEnums());
                }
                else
                {
                    Generator.Generate(new Parser(filePath).ParseHeader());
                }
            }
        }
        if (GUILayout.Button("Binarize All"))
        {
            foreach (var filePath in Directory.GetFiles(RootPath))
            {
                // enum.csv doesn't need to be binarized
                if (filePath.Contains("Enums.csv")) continue;

                Binarize(filePath);
            }
        }

        EditorGUILayout.Space(5);
        _searchData = EditorGUILayout.TextField(_searchData);
        _scrollList = EditorGUILayout.BeginScrollView(_scrollList);
        foreach (var filePath in Directory.GetFiles(RootPath).OrderBy((filePath) => filePath))
        {
            if (filePath.Contains("Enums.csv")) continue;

            var fileName = Path.GetFileNameWithoutExtension(filePath);

            if (string.IsNullOrEmpty(_searchData) || fileName.Contains(_searchData, StringComparison.CurrentCultureIgnoreCase))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(fileName))
                {
                    LoadAsync(fileName);
                }

                // 'G' stands for 'Generate'
                if (GUILayout.Button("G", GUILayout.Width(20)))
                {
                    Generator.Generate(new Parser(filePath).ParseHeader());
                }

                // 'B' stands for 'Binarize'
                if (GUILayout.Button("B", GUILayout.Width(20)))
                {
                    Binarize(filePath);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    void DrawDesignData()
    {
        var tableType = _table != null ? _table.GetType() : null;

        var data = _table != null ? tableType.GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_table) : null;
        var props = _table != null ? data.GetType().GetGenericArguments().Last().GetProperties(BindingFlags.Instance | BindingFlags.Public) : null;

        _scrollParser = EditorGUILayout.BeginScrollView(_scrollParser);
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", EditorStyles.miniButton, GUILayout.Width(100));
        EditorGUILayout.LabelField(tableType?.Name, EditorStyles.textField, GUILayout.Width(500));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Desc", EditorStyles.miniButton, GUILayout.Width(100));
        EditorGUILayout.LabelField("", new GUIStyle(EditorStyles.textField) { wordWrap = true }, GUILayout.Width(500));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        if (data != null)
        {
            var content = new GUIContent($"Data ({(data as ICollection).Count})");

            EditorGUILayout.LabelField(content, EditorStyles.boldLabel, GUILayout.Width(EditorStyles.boldLabel.CalcSize(content).x));
        }
        else
        {
            var content = new GUIContent("Data");

            EditorGUILayout.LabelField(content, EditorStyles.boldLabel, GUILayout.Width(EditorStyles.boldLabel.CalcSize(content).x));
        }

        if (props != null)
        {
            EditorGUILayout.BeginHorizontal();
            foreach (var prop in props)
            {
                EditorGUILayout.LabelField(Utils.GetType(prop.PropertyType), EditorStyles.miniButton, GUILayout.Width(200));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            foreach (var prop in props)
            {
                EditorGUILayout.LabelField(new GUIContent(prop.Name, "Description"), EditorStyles.miniButton, GUILayout.Width(200));
            }
            EditorGUILayout.EndHorizontal();

            if (data != null)
            {
                var values = data switch
                {
                    IDictionary => (data as IDictionary).Values as ICollection,
                    IList => data as ICollection,
                };
                foreach (var datum in values)
                {
                    EditorGUILayout.BeginHorizontal();
                    foreach (var prop in props)
                    {
                        var value = prop.GetValue(datum);

                        if (value is Array array)
                        {
                            var valueStr = $"{array.GetValue(0)}";
                            for (int i = 1; i < array.Length; i++)
                            {
                                valueStr += $", {array.GetValue(i)}";
                            }

                            EditorGUILayout.LabelField(valueStr, EditorStyles.textField, GUILayout.Width(200));
                        }
                        else
                        {
                            EditorGUILayout.LabelField($"{value}", EditorStyles.textField, GUILayout.Width(200));
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    void Refresh()
    {
        _serializedObject.ApplyModifiedProperties();
        _serializedObject.Update();
    }

    async void LoadAsync(string file)
    {
        var method = typeof(Serializer).GetMethod("DeserializeAsync", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(Type.GetType($"Table{file}"));
        var task = method.Invoke(null, new object[0]) as Task;

        await task;

        _table = task.GetType().GetProperty("Result").GetValue(task) as ITable;

        Repaint();
    }

    void Binarize(string filePath)
    {
        var parser = new Parser(filePath);
        var header = parser.ParseHeader();
        var tableType = Type.GetType($"Table{header.Name}");
        var table = tableType.GetConstructor(new Type[] { typeof(List<>).MakeGenericType(Type.GetType(header.Name)) }).Invoke(new object[] { parser.ParseBody() }) as ITable;

        typeof(Serializer).GetMethod("SerializeAsync", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(tableType).Invoke(null, new object[] { table });
    }
}
#endif
