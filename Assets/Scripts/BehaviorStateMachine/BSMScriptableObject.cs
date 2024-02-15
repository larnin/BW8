using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BSMSaveNode
{
    public string ID;
    public string name;
    public BSMNodeType nodeType;
    public Rect position;

    public object data;

    public List<string> outNodes = new List<string>();
}

public class BSMSaveData
{
    public List<BSMSaveNode> nodes = new List<BSMSaveNode>();
    public List<BSMAttribute> attributes = new List<BSMAttribute>();
}

public class BSMScriptableObject : SerializedScriptableObject
{
    [SerializeField] public BSMSaveData data;
}

