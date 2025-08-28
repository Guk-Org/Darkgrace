using DG.Tweening;
using Mirror;
using UnityEngine;

public class Leaning : NetworkBehaviour
{
    private Entity entity;

    public bool AutoDetectMedian = true;
    public Transform median;

    public AudioSource soundSource;

    public AudioClip[] LeanSounds;
    public AudioClip[] UnleanSounds;

    public AudioClip[] QuietLeanSounds;
    public AudioClip[] QuietUnleanSounds;

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

    public void Start()
    {
        soundSource = GetComponent<AudioSource>();
        entity = GetComponent<Entity>();
    }

    [Command]
    public void CmdSetLeanValue(int val)
    {
        LeanInput = val;
        
        if (val != 0)
        {
            if (!entity.SlowWalking.SlowWalk)
            {
                int index = Random.Range(0, LeanSounds.Length);
                RpcPlayLeanSound(index);
            }
            else
            {
                int index = Random.Range(0, QuietLeanSounds.Length);
                RpcPlayQuietLeanSound(index);
            }
        }
        else
        {
            if (!entity.SlowWalking.SlowWalk)
            {
                int index = Random.Range(0, UnleanSounds.Length);
                RpcPlayUnleanSound(index);
            }
            else
            {
                int index = Random.Range(0, QuietUnleanSounds.Length);
                RpcPlayQuietUnleanSound(index);
            }
        }
    }

    [ClientRpc]
    public void RpcPlayLeanSound(int index)
    {
        soundSource.PlayOneShot(LeanSounds[index]);
    }

    [ClientRpc]
    public void RpcPlayUnleanSound(int index)
    {
        soundSource.PlayOneShot(UnleanSounds[index]);
    }

    [ClientRpc]
    public void RpcPlayQuietLeanSound(int index)
    {
        soundSource.PlayOneShot(QuietLeanSounds[index]);
    }

    [ClientRpc]
    public void RpcPlayQuietUnleanSound(int index)
    {
        soundSource.PlayOneShot(QuietUnleanSounds[index]);
    }

    public void OnLeanInputChanged(int oldVal, int newVal)
    {
        median.DOLocalRotate(new Vector3(median.localRotation.x, median.localRotation.y, LeanAmount * newVal), LeanTime).SetEase(Ease.OutSine);
    }
}
