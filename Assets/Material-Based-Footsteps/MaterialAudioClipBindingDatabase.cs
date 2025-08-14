using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MaterialAudioClipBinding
{
    public Material Mat;
    public List<AudioClip> FootstepClips;
    public List<AudioClip> SoftstepClips;
    public List<AudioClip> RunstepClips;
    public float Volume = 1f;
    public float Pitch = 1f;
    public float PitchVariation = 0.1f;
}

[CreateAssetMenu(fileName = "FootstepBindings", menuName = "Footsteps/Binding Database")]
public class MaterialAudioClipBindingDatabase : ScriptableObject
{
    public List<MaterialAudioClipBinding> Bindings = new();
}
