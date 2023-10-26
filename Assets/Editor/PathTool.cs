using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;

[EditorTool("Path Tool", typeof(Path))]
public class PathTool : EditorTool, IDrawSelectedHandles
{
    [Shortcut("Activate Path Tool", typeof(SceneView), KeyCode.P)]
    static void PlatformToolShortcut()
    {
        if (Selection.GetFiltered<Path>(SelectionMode.TopLevel).Length > 0)
            ToolManager.SetActiveTool<PathTool>();
        else
            Debug.Log("No Path selected!");
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView sceneView))
            return;

        foreach (var obj in targets)
        {
            if (obj is Path path)
                DrawOnePathHandles(path);
        }
    }

    public void OnDrawHandles()
    {
        foreach (var obj in targets)
        {
            if (obj is Path path)
                DrawOnePathDebug(path);
        }
    }

    void DrawOnePathHandles(Path path)
    {
        for (int i = 0; i < path.GetPointNb(); i++)
        {
            Handles.color = Color.yellow;
            Vector3 pos = path.GetPointPos(i);

            EditorGUI.BeginChangeCheck();
            //pos = Handles.PositionHandle(pos, Quaternion.identity);
            pos = Handles.Slider2D(pos, Vector3.forward, Vector3.up, Vector3.left, 0.015f, Handles.DotHandleCap, 0.0f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(path, "Move Path");
                path.SetPointPos(i, pos);
            }

            Vector3 orthoDir = path.GetPointOrthoDir(i);
            float width = path.GetPointWidth(i);

            Vector3 offPos = pos + orthoDir * width;
            Vector3 offPos2 = pos - orthoDir * width;

            Handles.color = Color.green;

            EditorGUI.BeginChangeCheck();
            offPos = Handles.Slider2D(offPos, Vector3.forward, Vector3.up, Vector3.left, 0.015f, Handles.DotHandleCap, 0.0f);
            if (EditorGUI.EndChangeCheck())
            {
                float dist = (offPos - pos).magnitude;
                path.SetPointWidth(i, dist);
            }

            EditorGUI.BeginChangeCheck();
            offPos2 = Handles.Slider2D(offPos2, Vector3.forward, Vector3.up, Vector3.left, 0.015f, Handles.DotHandleCap, 0.0f);
            if (EditorGUI.EndChangeCheck())
            {
                float dist = (offPos2 - pos).magnitude;
                path.SetPointWidth(i, dist);
            }
        }
    }

    void DrawOnePathDebug(Path path)
    {
        int nbPoint = path.IsLoop() ? path.GetPointNb() : path.GetPointNb() - 1;

        for (int i = 0; i < nbPoint; i++)
        {
            int nextIndex = i < nbPoint - 1 ? i + 1 : 0;

            Vector3 pos = path.GetPointPos(i);
            Vector3 nextPos = path.GetPointPos(nextIndex);

            Vector3 orthoDir = path.GetPointOrthoDir(i);
            Vector3 nextOrthoDir = path.GetPointOrthoDir(nextIndex);

            float width = path.GetPointWidth(i);
            float nextWidth = path.GetPointWidth(nextIndex);

            Handles.color = Color.yellow;
            Handles.DrawLine(pos, nextPos, 0.5f);

            Handles.color = Color.green;

            Vector3 offPos = pos + orthoDir * width;
            Vector3 nextOffPos = nextPos + nextOrthoDir * nextWidth;
            Handles.DrawLine(offPos, nextOffPos, 1.5f);

            Vector3 offPos2 = pos - orthoDir * width;
            Vector3 nextOffPos2 = nextPos - nextOrthoDir * nextWidth;
            Handles.DrawLine(offPos2, nextOffPos2, 1.5f);

            Handles.DrawLine(offPos, offPos2, 1.5f);
            Handles.DrawLine(nextOffPos, nextOffPos2, 1.5f);
        }
    }
}
