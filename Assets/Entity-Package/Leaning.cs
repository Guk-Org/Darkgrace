using DG.Tweening;
using UnityEngine;

public class Leaning : MonoBehaviour
{
    public bool AutoDetectNeck = true;
    public Transform neck;

    public int LeanInput;
    public float LeanTime = 0.35f;
    public float LeanAmount = 20f;

    void Start()
    {
        if (AutoDetectNeck)
        {
            neck = gameObject.FindObject("Neck").transform;
        }    
    }

    
    void Update()
    {
        neck.DOLocalRotate(new Vector3(neck.localRotation.x, neck.localRotation.y, LeanAmount * LeanInput), LeanTime).SetEase(Ease.Linear);
    }

    public void SetLeanValue(int val)
    {
        LeanInput = val;
    }
}
