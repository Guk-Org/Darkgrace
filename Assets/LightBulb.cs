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

    // Runs on clients when Power updates
    void OnPowerChanged(float oldVal, float newVal)
    {
        ApplyPower(newVal);
    }

    void ApplyPower(float value)
    {
        myLight.intensity = OriginIntensity * (value * 0.01f);
        soundSource.volume = value * 0.01f;
    }

    [Command(requiresAuthority = false)]
    public void Toggle()
    {
        float old = Power;
        Power = (Power <= 0.001f) ? 100f : 0f;

        // Update server-side visuals immediately
        ApplyPower(Power);

        // If turning on and we haven't played the start sound yet, pick once on the server
        if (old <= 0.001f && Power > 0.001f && !AlreadyPlayedStartSound && StartSounds != null && StartSounds.Length > 0)
        {
            int index = Random.Range(0, StartSounds.Length);
            AlreadyPlayedStartSound = true;        // SyncVar so clients know we've done it
            RpcPlayStartSound(index);
            float randomTime = Random.Range(0, soundSource.clip.length);
            RpcChangeBuzzTime(randomTime);
        }
    }

    [ClientRpc]
    void RpcPlayStartSound(int index)
    {
        if (!soundSource || StartSounds == null || StartSounds.Length == 0)
        {
            return;
        }
        index = Mathf.Clamp(index, 0, StartSounds.Length - 1);
        var clip = StartSounds[index];
        if (clip)
        {
            soundSource.PlayOneShot(clip);
        }
    }

    [ClientRpc]
    void RpcChangeBuzzTime(float t)
    {
        soundSource.time = t;
    }
}
