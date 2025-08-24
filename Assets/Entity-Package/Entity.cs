using UnityEngine;

public class Entity : BaseEntity
{
    protected SlowWalking slowWalking;
    protected Leaning leaning;

    public override void Start()
    {
        base.Start();
        leaning = GetComponent<Leaning>();
        slowWalking = GetComponent<SlowWalking>();
    }

}
