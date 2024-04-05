using System;
using System.Collections;
using System.Collections.Generic;

public static class Utils 
{
    public static Type GetType(string type)
    {
        return type switch
        {
            // add types to support if need
            "string" => typeof(string),
            "string[]" => typeof(string[]),
            "int" => typeof(int),
            "int[]" => typeof(int[]),
            "float" => typeof(float),
            "float[]" => typeof(float[]),
            "double" => typeof(double),
            "double[]" => typeof(double[]),
            "bool" => typeof(bool),
            "bool[]" => typeof(bool[]),
            _ => Type.GetType(type),
        };
    }

    public static string GetType(Type type)
    {
        // add types to support if need
        if (type == typeof(string)) return "string";
        if (type == typeof(string[])) return "string[]";
        if (type == typeof(int)) return "int";
        if (type == typeof(int[])) return "int[]";
        if (type == typeof(float)) return "float";
        if (type == typeof(float[])) return "float[]";
        if (type == typeof(double)) return "double";
        if (type == typeof(double[])) return "double[]";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(bool[])) return "bool[]";
        return type.Name;
    }
}
