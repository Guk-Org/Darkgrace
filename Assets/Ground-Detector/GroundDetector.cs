using Mirror;
using System.Linq;
using UnityEngine;

public class GroundDetector : NetworkBehaviour
{
    public float RayDistance = 1;

    [SyncVar]
    public bool IsGrounded;
    public LayerMask GroundMask;

    [SyncVar]
    public SurfaceType GroundMaterial;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (isServer)
        {
            CheckGround();
        }
    }

    [Server]
    private void CheckGround()
    {
        bool willGround = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform pose = transform.GetChild(i);
            RaycastHit[] hits = Physics.RaycastAll(pose.position, -pose.up, RayDistance, GroundMask);
            hits = hits.OrderBy(hit => hit.distance).ToArray();
            if (hits.Length > 0)
            {
                willGround = true;
            }
            Material mat = hits[0].collider.GetComponent<MeshRenderer>().material;
        }
        IsGrounded = willGround;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform pose = transform.GetChild(i);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pose.transform.position, pose.transform.position + -pose.up * RayDistance);
        }
    }
}
