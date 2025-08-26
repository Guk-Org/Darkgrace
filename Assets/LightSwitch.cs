using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

public class LightSwitch : Interactable
{
    public LightBulb[] LightGroup;

    private AudioSource soundSource;
    public AudioClip OnSound;
    public AudioClip OffSound;

    [SyncVar]
    public bool Switched;

    public override void Awake()
    {
        soundSource = GetComponent<AudioSource>();
    }

    [Server]
    public void DetermineBeginningSwitchState()
    {
        int i;
        int totalZeros = 0;
        for (i = 0; i < LightGroup.Length; i++)
        {
            LightBulb l = LightGroup[i];
            if (l.Power <= 0)
            {
                totalZeros++;
            }
        }
        if ((totalZeros / i) >= 0.5f)
        {
            Switched = false;
        }
        else
        {
            Switched = true;
        }
    }

    [Command(requiresAuthority = false)]
    public override void Interact()
    {
        base.Interact();
        foreach (LightBulb l in LightGroup)
        {
            l.Toggle();
        }
        Switched = !Switched;
        RpcSwitchSound();
    }

    [ClientRpc]
    public void RpcSwitchSound()
    {
        if (Switched)
        {
            soundSource.PlayOneShot(OffSound);
        }
        else
        {
            soundSource.PlayOneShot(OnSound);
        }
    }

    public void OnDrawGizmosSelected()
    {
        foreach (LightBulb l in LightGroup)
        {
            if (l.gameObject)
            {
                Gizmos.DrawLine(transform.position, l.gameObject.transform.position);
            }
        }
    }
}
