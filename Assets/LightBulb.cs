using Mirror;
using UnityEngine;

public class LightBulb : NetworkBehaviour
{
    private Light myLight;
    private AudioSource soundSource;

    public bool OffOnAwake = true;
    public AudioClip[] StartSounds;

    [SyncVar] public float OriginIntensity;
    [SyncVar] public bool AlreadyPlayedStartSound;

    [SyncVar(hook = nameof(OnPowerChanged))]
    public float Power;

    void Awake()
    {
        myLight = GetComponent<Light>();
        soundSource = GetComponent<AudioSource>();
    }

    public override void OnStartServer()
    {
        OriginIntensity = myLight.intensity;
        if (OffOnAwake)
        {
            Power = 0f;
        }
    }

    public override void OnStartClient()
    {
        ApplyPower(Power);
    }

    void OnPowerChanged(float oldVal, float newVal)
    {
        ApplyPower(newVal);
    }

    void ApplyPower(float value)
    {
        myLight.intensity = OriginIntensity * (value * 0.01f);
    }

    [Command(requiresAuthority = false)]
    public void Toggle()
    {
        float old = Power;
        Power = (Power <= 0.001f) ? 100f : 0f;

        OnPowerChanged(old, Power);
    }
}
