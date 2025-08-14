using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    private Dictionary<float, Light> myLights;
    public float FadeInTime = 0.1f;
    public float FadeOutTime = 0.1f;

    private void Start()
    {
        foreach (Light l in gameObject.GetComponentsInChildren<Light>())
        {
            myLights.Add(l.intensity, l);
        }
    }

    public void OnEnable()
    {
        for (int i = 0; i < myLights.Count; i++)
        {
            Light l = myLights[i];
            l.DOIntensity(myLights[i].intensity, FadeInTime);
        }
    }

    public void OnDisable()
    {
        for (int i = 0; i < myLights.Count; i++)
        {
            Light l = myLights[i];
            l.DOIntensity(0, FadeOutTime);
        }
    }
}
