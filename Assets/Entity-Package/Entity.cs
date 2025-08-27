using UnityEngine;

public class Entity : BaseEntity
{
    public SlowWalking SlowWalking;
    public Leaning Leaning;

    public override void Start()
    {
        base.Start();
        Leaning = GetComponent<Leaning>();
        SlowWalking = GetComponent<SlowWalking>();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

}
