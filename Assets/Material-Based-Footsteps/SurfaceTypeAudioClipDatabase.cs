using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SurfaceTypeAudioClipBinding
{
    public Material SurfaceType;
    public List<AudioClip> RunstepClips;
    public List<AudioClip> WalkstepClips;
    public float Volume = 1f;
    public float Pitch = 1f;
    public float PitchVariation = 0.1f;
}

[CreateAssetMenu(fileName = "SurfaceTypeAudioClip", menuName = "Footsteps/Binding Database")]
public class SurfaceTypeAudioClipDatabase : ScriptableObject
{
    public List<SurfaceTypeAudioClipBinding> Bindings = new();
}
