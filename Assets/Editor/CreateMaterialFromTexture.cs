
using Microsoft.SqlServer.Server;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CreateMaterialFromTexture
{

    [MenuItem("Assets/Create Material")]
    public static void CreateDiffuseMaterial()
    {
        var selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        var cnt = selectedAssets.Length * 1.0f;
        var idx = 0f;

        Texture2D color = null;
        Texture2D displacement = null;
        Texture2D normalMap = null;
        Texture2D metalness = null;
        Texture2D ambientOcclusion = null;
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        foreach (Object obj in selectedAssets)
        {
            idx++;
            EditorUtility.DisplayProgressBar("Create material", "Create material for: " + obj.name, idx / cnt);

            if (obj is Texture2D)
            {
                if (obj.name.ToLower().Contains("displacement"))
                {
                    displacement = (Texture2D)obj;
                }
                else if (obj.name.ToLower().Contains("normal"))
                {
                    normalMap = (Texture2D)obj;
                }
                else if (obj.name.ToLower().Contains("metallic"))
                {
                    metalness = (Texture2D)obj;
                }
                else if (obj.name.ToLower().Contains("ambient"))
                {
                    ambientOcclusion = (Texture2D)obj;
                }
                else
                {
                    color = (Texture2D)obj;
                }
            }
        }
        EditorUtility.ClearProgressBar();

        if (shader == null)
        {
            return;
        }
        var path = AssetDatabase.GetAssetPath(color);
        if (File.Exists(path))
        {
            path = Path.GetDirectoryName(path);
        }
        var mat = new Material(shader) { mainTexture = color };
        mat.mainTextureScale = Vector2.one * 0.5f;
        if (displacement != null)
        {
            mat.SetTexture("_ParallaxMap", displacement);
        }
        if (normalMap != null)
        {
            mat.SetTexture("_BumpMap", normalMap);
        }
        if (metalness != null)
        {
            mat.SetTexture("_MetallicGlossMap", metalness);
        }
        if (ambientOcclusion != null)
        {
            mat.SetTexture("_OcclusionMap", ambientOcclusion);
        }

        mat.SetFloat("_Smoothness", 0);

        AssetDatabase.CreateAsset(mat, Path.Combine(path, string.Format("{0}.mat", color.name)));

        // Register the asset for undo
        Undo.RegisterCreatedObjectUndo(mat, "Create Material Asset");

        // Refresh the AssetDatabase to reflect the changes
        AssetDatabase.Refresh();

    }
}