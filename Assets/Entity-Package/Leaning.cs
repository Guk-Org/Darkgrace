using DG.Tweening;
using Mirror;
using UnityEngine;

public class Leaning : NetworkBehaviour
{
    public bool AutoDetectMedian = true;
    public Transform median;

    [SyncVar(hook = nameof(OnLeanInputChanged))]
    public int LeanInput;
    public float LeanTime = 0.35f;
    public float LeanAmount = 20f;

    void Awake()
    {
        if (AutoDetectMedian)
        {
            median = gameObject.FindObject("Median").transform;
        }

        if (!isLocalPlayer)
        {
            return;
        }
        CmdSetLeanValue(LeanInput);
    }

    [Command]
    public void CmdSetLeanValue(int val)
    {
        LeanInput = val;
    }

    public void OnLeanInputChanged(int oldVal, int newVal)
    {
        median.DOLocalRotate(new Vector3(median.localRotation.x, median.localRotation.y, LeanAmount * newVal), LeanTime).SetEase(Ease.OutSine);
    }
}
