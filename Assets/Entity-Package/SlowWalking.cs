using Mirror;
using UnityEngine;

public class SlowWalking : NetworkBehaviour
{
    private BaseEntity entity;
    public float SlowWalkSpeed = 3f;
    public float SlowWalkAcceleration = 22;
    [SyncVar]
    public bool SlowWalk = false;

    public void Start()
    {
        entity = GetComponent<BaseEntity>();
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
}
