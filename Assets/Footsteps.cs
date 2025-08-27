using Mirror;
using UnityEngine;

public class Footsteps : NetworkBehaviour
{
    private PlayerScript player;

    [SyncVar(hook = nameof(OnSetFootstep))]
    public Vector3? LastFootstepPos;

    public float MaxFootstepDist = 1;

    void Start()
    {
        player = GetComponent<PlayerScript>();
    }

    

    void Update()
    {
        if (isServer)
        {
            CheckFootstepDistance();
        }
    }

    [Server]
    private void CheckFootstepDistance()
    {
        if (player.Gd.IsGrounded)
        {
            if (LastFootstepPos != null)
            {
                if (Vector3.Distance(transform.position, (Vector3)LastFootstepPos) > MaxFootstepDist)
                {
                    LastFootstepPos = transform.position;
                }
            }
            else
            {
                LastFootstepPos = transform.position;
            }
        }
    }

    public void OnSetFootstep(Vector3? oldVal, Vector3? newVal)
    {
        
        
    }
}
