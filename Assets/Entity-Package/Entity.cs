using UnityEngine;

public class Entity : BaseEntity
{
    public SlowWalking SlowWalking;
    public Leaning Leaning;
    protected Transform median;

    public override void Start()
    {
        base.Start();
        Leaning = GetComponent<Leaning>();
        SlowWalking = GetComponent<SlowWalking>();
        median = gameObject.FindObject("Median").transform;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

}
