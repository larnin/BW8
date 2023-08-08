using UnityEngine;
using UnityEditor;
using System.Collections;

public class FileModificationWarning : AssetModificationProcessor
{
    static string[] OnWillSaveAssets(string[] paths)
    {
        string saveStr = "Save Assets: ";
        foreach (var str in paths)
            saveStr += str + ", ";

        if (paths.Length > 0)
            saveStr.Remove(saveStr.Length - 2);

        Debug.Log(saveStr);

        return paths;
    }
}