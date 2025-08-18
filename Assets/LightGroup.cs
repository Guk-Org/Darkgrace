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

    [SyncVar(hook = nameof(OnChanged))]
    private bool isOn;

    private void Awake()
    {
        foreach (var l in GetComponentsInChildren<Light>(includeInactive: true))
        {
            myLights[l] = l.intensity;
        }
    }


    public override void OnStartServer()
    {
        if (OffOnAwake)
        {
            foreach (var kv in myLights)
            {
                kv.Key.intensity = 0f;
            }
            isOn = false;
        }
    }

    public override void OnStartClient()
    {
        ChangeLights(isOn, false);
    }

    private void OnChanged(bool oldVal, bool newVal)
    {
        ChangeLights(newVal, true);
    }

    private void ChangeLights(bool turnOn, bool shouldTween)
    {
        foreach (var kv in myLights)
        {
            var l = kv.Key;
            var target = turnOn ? kv.Value : 0f;
            l.DOKill(complete: false);

            if (shouldTween)
            {
                var t = turnOn ? FadeInTime : FadeOutTime;
                l.DOIntensity(target, t);
            }
            else
            {
                l.intensity = target;
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdToggle()
    {
        isOn = !isOn;
    }

}
