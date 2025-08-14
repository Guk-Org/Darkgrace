using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public float RayDistance = 1;
    public bool IsGrounded;
    public LayerMask GroundMask;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        bool willGround = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform pose = transform.GetChild(i);
            if (Physics.Raycast(pose.position, -pose.up, RayDistance, GroundMask))
            {
                willGround = true;
            }
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
