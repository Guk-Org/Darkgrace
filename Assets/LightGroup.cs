using DG.Tweening;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class LightGroup : NetworkBehaviour
{
    private Dictionary<Light, float> myLights = new Dictionary<Light, float>();
    public float FadeInTime = 0.04f;
    public float FadeOutTime = 0.04f;

    public bool OffOnAwake = true;

    private void Start()
    {
        foreach (Light l in gameObject.GetComponentsInChildren<Light>())
        {
            myLights.Add(l, l.intensity);
            if (OffOnAwake)
            {
                l.intensity = 0;
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void Toggle()
    {
        foreach (var keyValuePair in myLights)
        {
            Light l = keyValuePair.Key;
            float intensity = keyValuePair.Value;
            if (l.intensity != intensity)
            {
                l.DOIntensity(intensity, FadeInTime);
            }
            else
            {
                l.DOIntensity(0, FadeOutTime);
            }
        }
    }
}
