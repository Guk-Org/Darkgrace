using Mirror;
using UnityEngine;

public class SlowWalking : NetworkBehaviour
{
    private BaseEntity entity;
    public float SlowWalkSpeed = 3f;
    public float SlowWalkAcceleration = 22;
    [SyncVar]
    public bool SlowWalk = false;

    private AudioSource soundSource;

    public AudioClip[] UnSlowWalkSounds;

    public void Start()
    {
        entity = GetComponent<BaseEntity>();
        soundSource = GetComponent<AudioSource>();
    }

    public void FixedUpdate()
    {
        if (isServer)
        {
            HandleSlowWalk();
        }
    }

    [Command]
    public void CmdSetSlowWalk(bool val)
    {
        SlowWalk = val;
        if (val == false)
        {
            if (UnSlowWalkSounds.Length > 0)
            {
                int index = Random.Range(0, UnSlowWalkSounds.Length);
                RpcPlayUnSlowWalkSound(index);
            }
        }
    }

    [Server]
    public void HandleSlowWalk()
    {
        if (SlowWalk)
        {
            entity.TargetSpeed = SlowWalkSpeed;
            entity.TargetAcceleration = SlowWalkAcceleration;
        }
        else
        {
            if (!entity.Running)
            {
                entity.TargetSpeed = entity.WalkSpeed;
                entity.TargetAcceleration = entity.WalkAcceleration;
            }

        }
    }

    [ClientRpc]
    public void RpcPlayUnSlowWalkSound(int index)
    {
        soundSource.PlayOneShot(UnSlowWalkSounds[index]);
    }
}
