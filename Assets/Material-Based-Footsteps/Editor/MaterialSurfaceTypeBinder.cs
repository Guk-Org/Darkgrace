using UnityEditor;
using UnityEngine;
using System.IO;

public class MaterialSurfaceTypeBinder : EditorWindow
{
    private MaterialSurfaceTypeDatabase bindingAsset;
    private Vector2 scroll;
    private const string assetPath = "Assets/Editor/MaterialSurfaceType.asset"; // adjust path as needed
    private SerializedObject serializedAsset;


    [MenuItem("Tools/Material Surface Type Binder")]
    public static void ShowExample()
    {
        MaterialSurfaceTypeBinder window = GetWindow<MaterialSurfaceTypeBinder>();
        window.titleContent = new GUIContent("Material Surface Type Binder");
    }

    private void OnEnable()
    {
        LoadOrCreateAsset();
        serializedAsset = new SerializedObject(bindingAsset);
    }


    private void OnGUI()
    {
        if (bindingAsset == null)
        {
            EditorGUILayout.HelpBox("Asset not loaded.", MessageType.Error);
            return;
        }

        serializedAsset.Update();

        SerializedProperty bindingsProp = serializedAsset.FindProperty("Bindings");

        scroll = EditorGUILayout.BeginScrollView(scroll);

        for (int i = 0; i < bindingsProp.arraySize; i++)
        {
            SerializedProperty bindingProp = bindingsProp.GetArrayElementAtIndex(i);
            SerializedProperty matProp = bindingProp.FindPropertyRelative("Mat");
            SerializedProperty surfaceType = bindingProp.FindPropertyRelative("SurfaceType");

            EditorGUILayout.BeginVertical("box");

            // Draw material texture preview
            Material mat = matProp.objectReferenceValue as Material;
            if (mat != null && mat.mainTexture != null)
            {
                Texture texture = mat.mainTexture ?? mat.GetTexture("_BaseMap") ?? mat.GetTexture("_MainTex");

                float previewSize = 80f;
                float padding = 10f;
                float x = EditorGUIUtility.currentViewWidth - previewSize - padding;
                Rect rect = new Rect(x, GUILayoutUtility.GetRect(0, previewSize).y, previewSize, previewSize);

                if (Event.current.type == EventType.Repaint)
                {
                    GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
                }

                
            }

            EditorGUILayout.PropertyField(matProp, new GUIContent("Material"));
            EditorGUILayout.PropertyField(surfaceType, new GUIContent("Surface Type"));


            if (GUILayout.Button("Remove Binding"))
            {
                bindingsProp.DeleteArrayElementAtIndex(i);
                break; // Prevent GUI error
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Binding"))
        {
            bindingsProp.InsertArrayElementAtIndex(bindingsProp.arraySize);
        }

        EditorGUILayout.EndScrollView();

        if (serializedAsset.ApplyModifiedProperties())
        {
            SaveAsset(); // only saves when changes were made
        }

    }



    private void LoadOrCreateAsset()
    {
        bindingAsset = AssetDatabase.LoadAssetAtPath<MaterialSurfaceTypeDatabase>(assetPath);
        if (bindingAsset == null)
        {
            bindingAsset = ScriptableObject.CreateInstance<MaterialSurfaceTypeDatabase>();
            Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
            AssetDatabase.CreateAsset(bindingAsset, assetPath);
            AssetDatabase.SaveAssets();
        }
    }

    private void SaveAsset()
    {
        EditorUtility.SetDirty(bindingAsset);
        AssetDatabase.SaveAssets();
    }


}
