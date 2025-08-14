using UnityEngine;

public class SlowWalking : MonoBehaviour
{
    private BaseEntity entity;
    public float SlowWalkSpeed = 3f;
    public float SlowWalkAcceleration = 22;
    public bool SlowWalk = false;

    public void Start()
    {
        entity = GetComponent<BaseEntity>();
    }

    public void FixedUpdate()
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
