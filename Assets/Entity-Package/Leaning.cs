using DG.Tweening;
using Mirror;
using UnityEngine;

public class Leaning : NetworkBehaviour
{
    public bool AutoDetectMedian = true;
    public Transform median;

    [SyncVar]
    public int LeanInput;
    public float LeanTime = 0.35f;
    public float LeanAmount = 20f;

    void Start()
    {
        if (AutoDetectMedian)
        {
            median = gameObject.FindObject("Median").transform;
        }

        if (!isLocalPlayer)
        {
            return;
        }
        SetLeanValue(LeanInput);
    }

    [Command]
    public void SetLeanValue(int val)
    {
        LeanInput = val;
        median.DOLocalRotate(new Vector3(median.localRotation.x, median.localRotation.y, LeanAmount * LeanInput), LeanTime).SetEase(Ease.OutSine);
    }
}
