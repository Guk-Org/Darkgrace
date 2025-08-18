using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerScript : BasePlayer
{
    private InteractionReciever interactionReciever;
    public GameObject InteractionIndicatorPrefab;
    private GameObject interactionIndicator;
    private Canvas interactionIndicatorCanvas;
    private Camera uiCamera;

    private Transform median;

    private AudioSource localSoundSource;

    public AudioClip InteractionIndicatorAppear;
    public AudioClip InteractionIndicatorDisappear;


    public override void Start()
    {
        base.Start();
        interactionReciever = CameraHolder.GetComponent<InteractionReciever>();
        uiCamera = gameObject.FindObject("UI Camera").GetComponent<Camera>();
        localSoundSource = gameObject.FindObject("Local Sound Source").GetComponent<AudioSource>();
        median = gameObject.FindObject("Median").transform;
        gameObject.FindObject("Upper_Torso").transform.parent = median;
        
        if (!isLocalPlayer)
        {
            return;
        }

        gameObject.FindObject("Head").GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        gameObject.FindObject("Neck").GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    }

    public override void Update()
    {
        gameObject.FindObject("Neck").transform.parent = gameObject.FindObject("Neck_Holder").transform;
        gameObject.FindObject("Neck_Holder").transform.forward = CameraHolder.transform.up;

        base.Update();

        if (!isLocalPlayer)
        {
            return;
        }

        if (interactionReciever.InteractablesInRange.Count > 0)
        {
            Interactable interactable = interactionReciever.InteractablesInRange[0];
            if (InteractionIndicatorPrefab != null)
            {
                if (interactionIndicator == null)
                {
                    interactionIndicator = Instantiate(InteractionIndicatorPrefab);
                    interactionIndicatorCanvas = interactionIndicator.GetComponentInChildren<Canvas>();
                    interactionIndicatorCanvas.worldCamera = uiCamera;
                    AudioHelper.PlayOneshot(InteractionIndicatorAppear, localSoundSource);
                }
                interactionIndicator.transform.parent = interactable.transform;
                interactionIndicator.transform.localPosition = Vector3.zero;
            }

            if (Input.GetButtonDown("Interact"))
            {
                interactable.Interact();
                if (interactionIndicatorCanvas != null)
                {
                    interactionIndicatorCanvas.transform.DOScale(Vector3.one * 0.001f, 0.05f);

                }
            }

            if (Input.GetButtonUp("Interact"))
            {
                if (interactionIndicatorCanvas != null)
                {
                    interactionIndicatorCanvas.transform.DOScale(Vector3.one * 0.003f, 0.18f);

                }
            }

        }
        else
        {
            if (interactionIndicator)
            {
                AudioHelper.PlayOneshot(InteractionIndicatorDisappear, localSoundSource);
                Destroy(interactionIndicator);
            }
        }

        if (Input.GetButtonDown("Lean Left"))
        {
            if (leaning.LeanInput != 1)
            {
                leaning.CmdSetLeanValue(1);
            }
            else
            {
                leaning.CmdSetLeanValue(0);
            }
        }

        if (Input.GetButtonDown("Lean Right"))
        {
            if (leaning.LeanInput != -1)
            {
                leaning.CmdSetLeanValue(-1);
            }
            else
            {
                leaning.CmdSetLeanValue(0);
            }

        }

        if (Input.GetButtonDown("Slow Walk"))
        {
            if (!slowWalking.SlowWalk)
            {
                slowWalking.SlowWalk = true;
            }
            else
            {
                slowWalking.SlowWalk = false;
            }
        }
    }
}
