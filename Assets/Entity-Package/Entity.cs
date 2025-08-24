using UnityEngine;

public class Entity : BaseEntity
{
    protected SlowWalking slowWalking;
    protected Leaning leaning;

    public override void Awake()
    {
        base.Awake();
        leaning = GetComponent<Leaning>();
        slowWalking = GetComponent<SlowWalking>();
    }

}
