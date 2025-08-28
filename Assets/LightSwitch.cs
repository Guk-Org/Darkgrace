using Mirror;
using UnityEngine;
using UnityEngine.Rendering;

public class LightSwitch : Interactable
{
    public LightBulb[] LightGroup;

    private AudioSource soundSource;
    public AudioClip[] OnSounds;
    public AudioClip[] OffSounds;

    public AudioClip[] QuietOnSounds;
    public AudioClip[] QuietOffSounds;

    public float RegularMaxDistance = 15;
    public float QuietMaxDistance = 5;

    [SyncVar]
    public bool Switched;

    public override void Awake()
    {
        soundSource = GetComponent<AudioSource>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
    }

    public void CmdReflectHearingDistance()
    {
        
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
    public override void Interact(NetworkConnectionToClient sender = null)
    {
        if (!ValidateInteract(sender))
        {
            return;
        }

        BaseInteractMethod();

        foreach (LightBulb l in LightGroup)
        {
            l.Toggle();
        }
        Switched = !Switched;

        PlayerScript player = sender.identity.gameObject.GetComponent<PlayerScript>();
        

        int offSoundIndex = Random.Range(0, OffSounds.Length);
        int onSoundIndex = Random.Range(0, OnSounds.Length);

        if (player && player.SlowWalking.SlowWalk)
        {
            offSoundIndex = Random.Range(0, QuietOffSounds.Length);
            onSoundIndex = Random.Range(0, QuietOnSounds.Length);
            QuietSwitchSound(offSoundIndex, onSoundIndex);
        }
        else
        {
            SwitchSound(offSoundIndex, onSoundIndex);
        }
    }

    [Server]
    public void SwitchSound(int offSoundIndex, int onSoundIndex)
    {
        if (Switched)
        {
            RpcPlayOffSound(offSoundIndex);
        }
        else
        {
            
            RpcPlayOnSound(onSoundIndex);
        }
    }

    [Server]
    public void QuietSwitchSound(int offSoundIndex, int onSoundIndex)
    {
        if (Switched)
        {
            RpcPlayQuietOffSound(offSoundIndex);
        }
        else
        {

            RpcPlayQuietOnSound(onSoundIndex);
        }
    }

    [ClientRpc]
    void RpcPlayOnSound(int index)
    {
        index = Mathf.Clamp(index, 0, OnSounds.Length - 1);
        var clip = OnSounds[index];
        if (clip)
        {
            soundSource.maxDistance = RegularMaxDistance;
            soundSource.PlayOneShot(clip);
        }
    }

    [ClientRpc]
    void RpcPlayOffSound(int index)
    {
        index = Mathf.Clamp(index, 0, OffSounds.Length - 1);
        var clip = OffSounds[index];
        if (clip)
        {
            soundSource.maxDistance = RegularMaxDistance;
            soundSource.PlayOneShot(clip);
        }
    }


    [ClientRpc]
    void RpcPlayQuietOnSound(int index)
    {
        index = Mathf.Clamp(index, 0, QuietOnSounds.Length - 1);
        var clip = QuietOnSounds[index];
        if (clip)
        {
            soundSource.maxDistance = QuietMaxDistance;
            soundSource.PlayOneShot(clip);
        }
    }

    [ClientRpc]
    void RpcPlayQuietOffSound(int index)
    {
        index = Mathf.Clamp(index, 0, QuietOffSounds.Length - 1);
        var clip = QuietOffSounds[index];
        if (clip)
        {
            soundSource.maxDistance = QuietMaxDistance;
            soundSource.PlayOneShot(clip);
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
