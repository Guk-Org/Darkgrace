using UnityEditor;
using UnityEngine;
using System.IO;

public class SurfaceTypeAudioClipBinder : EditorWindow
{
    private SurfaceTypeAudioClipDatabase bindingAsset;
    private Vector2 scroll;
    private const string assetPath = "Assets/Editor/SurfaceTypeAudioClip.asset"; // adjust path as needed
    private SerializedObject serializedAsset;


    [MenuItem("Tools/Surface Type Audio Clip Binder")]
    public static void ShowExample()
    {
        SurfaceTypeAudioClipBinder window = GetWindow<SurfaceTypeAudioClipBinder>();
        window.titleContent = new GUIContent("Surface Type Audio Clip Binder");
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
            SerializedProperty surfaceType = bindingProp.FindPropertyRelative("SurfaceType");
            SerializedProperty footstepClips = bindingProp.FindPropertyRelative("FootstepClips");
            SerializedProperty softstepClips = bindingProp.FindPropertyRelative("SoftstepClips");
            SerializedProperty runstepClips = bindingProp.FindPropertyRelative("RunstepClips");
            SerializedProperty volProp = bindingProp.FindPropertyRelative("Volume");
            SerializedProperty pitchProp = bindingProp.FindPropertyRelative("Pitch");
            SerializedProperty pitchVariationProp = bindingProp.FindPropertyRelative("PitchVariation");

            EditorGUILayout.BeginVertical("box");

            if (surfaceType == null)
            {
                EditorGUILayout.HelpBox("Binding is missing 'SurfaceType' field (name/case must match).", MessageType.Error);
            }
            else
            {
                EditorGUILayout.PropertyField(surfaceType, new GUIContent("Surface Type"));
            }

            // Footsteps
            EditorGUILayout.PropertyField(footstepClips, new GUIContent("Footstep Clips"), true);

            if (GUILayout.Button("▶ Play"))
            {
                var obj = bindingProp.serializedObject.targetObject as SurfaceTypeAudioClipDatabase;
                var actualBinding = obj.Bindings[i];
                var valid = actualBinding.FootstepClips.FindAll(c => c != null);
                if (valid.Count > 0)
                    PlayClip(valid[Random.Range(0, valid.Count)], actualBinding.Volume, actualBinding.Pitch, actualBinding.PitchVariation);
                else
                    Debug.LogWarning("No valid AudioClips assigned.");
            }

            // Softsteps
            EditorGUILayout.PropertyField(softstepClips, new GUIContent("Softstep Clips"), true);

            if (GUILayout.Button("▶ Play"))
            {
                var obj = bindingProp.serializedObject.targetObject as SurfaceTypeAudioClipDatabase;
                var actualBinding = obj.Bindings[i];
                var valid = actualBinding.SoftstepClips.FindAll(c => c != null);
                if (valid.Count > 0)
                    PlayClip(valid[Random.Range(0, valid.Count)], actualBinding.Volume, actualBinding.Pitch, actualBinding.PitchVariation);
                else
                    Debug.LogWarning("No valid AudioClips assigned.");
            }

            // Runsteps
            EditorGUILayout.PropertyField(runstepClips, new GUIContent("Runstep Clips"), true);

            if (GUILayout.Button("▶ Play"))
            {
                var obj = bindingProp.serializedObject.targetObject as SurfaceTypeAudioClipDatabase;
                var actualBinding = obj.Bindings[i];
                var valid = actualBinding.RunstepClips.FindAll(c => c != null);
                if (valid.Count > 0)
                    PlayClip(valid[Random.Range(0, valid.Count)], actualBinding.Volume, actualBinding.Pitch, actualBinding.PitchVariation);
                else
                    Debug.LogWarning("No valid AudioClips assigned.");
            }

            EditorGUILayout.Slider(volProp, 0f, 1f);
            EditorGUILayout.Slider(pitchProp, 0.1f, 3f);
            EditorGUILayout.Slider(pitchVariationProp, 0, 1);


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
        bindingAsset = AssetDatabase.LoadAssetAtPath<SurfaceTypeAudioClipDatabase>(assetPath);
        if (bindingAsset == null)
        {
            bindingAsset = ScriptableObject.CreateInstance<SurfaceTypeAudioClipDatabase>();
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

    private void PlayClip(AudioClip clip, float volume, float pitch, float pitchVariation)
    {
        if (clip == null)
        {
            Debug.LogWarning("No AudioClip assigned.");
            return;
        }

        // Create hidden GameObject
        GameObject tempGO = new GameObject("EditorAudioPlayer");
        tempGO.hideFlags = HideFlags.HideAndDontSave;

        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch + Random.Range(-pitchVariation, pitchVariation);
        source.playOnAwake = false;
        source.loop = false;

        source.Play();

        // Destroy after clip finishes
        EditorApplication.update += Cleanup;

        void Cleanup()
        {
            if (!source.isPlaying)
            {
                EditorApplication.update -= Cleanup;
                Object.DestroyImmediate(tempGO);
            }
        }
    }


}
