using UnityEngine;
using UnityEditor;
using RealtimeCSG.Components;

public static class InstaModelParent
{
    [MenuItem("Assets/RealtimeCSG/Insta Model Parent")]
    public static void AssignModel()
    {
        if (Selection.activeTransform != null)
        {
            GameObject model = CreateModel();
            model.transform.parent = Selection.activeTransform.parent;
            Selection.activeTransform.parent = model.transform;
        }
        else
        {
            CreateModel();
        }
    }

    public static GameObject CreateModel()
    {
        GameObject go = new GameObject();
        go.name = "Model";
        go.AddComponent<CSGModel>();
        return go;
    }
}
