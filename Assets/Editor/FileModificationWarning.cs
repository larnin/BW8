using UnityEngine;
using UnityEditor;
using System.Collections;

public class FileModificationWarning : AssetModificationProcessor
{
    static string[] OnWillSaveAssets(string[] paths)
    {
        Debug.Log("OnWillSaveAssets");
        foreach (string path in paths)
            Debug.Log(path);
        return paths;
    }
}