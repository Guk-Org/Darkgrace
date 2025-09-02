using Mirror;
using System.Collections;
using UnityEngine;

public class Beast : Entity
{
    public AudioClip[] TransformSounds;
    public AudioClip[] UntransformSounds;

    public Coroutine TransformCoroutine;

    public float TransformCounter;
    public float TransformMaxCount = 2f;

    public bool BeingControlled;

    [SyncVar]
    public bool TransformInput;

    public override void Start()
    {
        base.Start();
    }

    public IEnumerator TransformRoutine()
    {
        while (TransformCounter < TransformMaxCount)
        {
            yield return new WaitForSeconds(0.1f);
            TransformCounter += 0.1f;
        }
    }

    public override void Update()
    {
        base.Update();

        if (!BeingControlled)
        {
            return;
        }

        TransformInput = Input.GetButton("Transform");
    }

    public void OnTransformInputDown()
    {
        if (TransformCoroutine == null)
        {
            TransformCoroutine = StartCoroutine(TransformRoutine());
        }
    }

    public void OnTransformInputUp()
    {
        if (TransformCoroutine != null)
        {
            StopCoroutine(TransformCoroutine);
            TransformCounter = 0;
        }
    }
}
