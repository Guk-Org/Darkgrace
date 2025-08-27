using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MaterialSurfaceTypeBinding
{
    public Material Mat;
    public SurfaceType SurfaceType;
}

[CreateAssetMenu(fileName = "MaterialSurfaceType", menuName = "Footsteps/Binding Database")]
public class MaterialSurfaceTypeDatabase : ScriptableObject
{
    public List<MaterialSurfaceTypeBinding> Bindings = new();
}
