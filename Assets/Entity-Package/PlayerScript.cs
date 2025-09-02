using DG.Tweening;
using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerScript : BasePlayer
{
    public InteractionReciever InteractionReciever;
    public GameObject InteractionIndicatorPrefab;
    private GameObject interactionIndicator;
    private Canvas interactionIndicatorCanvas;
    private Camera uiCamera;

    

    public AudioSource SoundSource;

    public AudioClip InteractionIndicatorAppear;
    public AudioClip InteractionIndicatorDisappear;

    [SyncVar]
    public Interactable PrimaryInteractable;


    public override void Start()
    {
        base.Start();
        InteractionReciever = CameraHolder.GetComponent<InteractionReciever>();
        uiCamera = gameObject.FindObject("UI Camera").GetComponent<Camera>();
        SoundSource = gameObject.FindObject("Sound Source").GetComponent<AudioSource>();
        gameObject.FindObject("Upper-Torso").transform.parent = median.gameObject.FindObject("Upper-Torso-Corrector").transform;

        StartCoroutine(HandleInteractablesRoutine());

        if (!isLocalPlayer)
        {
            return;
        }

        gameObject.FindObject("Head").GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        gameObject.FindObject("Neck").GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

    }

    public IEnumerator HandleInteractablesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (isServer)
            {
                HandleInteractables();
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
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

        if (PrimaryInteractable)
        {
            if (InteractionIndicatorPrefab != null)
            {
                if (interactionIndicator == null)
                {
                    interactionIndicator = Instantiate(InteractionIndicatorPrefab);
                    interactionIndicatorCanvas = interactionIndicator.GetComponentInChildren<Canvas>();
                    interactionIndicatorCanvas.worldCamera = uiCamera;
                    AudioHelper.PlayOneshot(InteractionIndicatorAppear, SoundSource);
                }
                interactionIndicator.transform.parent = PrimaryInteractable.transform;
                interactionIndicator.transform.localPosition = Vector3.zero;
            }

            if (Input.GetButtonDown("Interact"))
            {
                PrimaryInteractable.Interact();
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
                AudioHelper.PlayOneshot(InteractionIndicatorDisappear, SoundSource);
                Destroy(interactionIndicator);
            }
        }

        if (Input.GetButtonDown("Lean Left"))
        {
            if (Leaning.LeanInput != 1)
            {
                Leaning.CmdSetLeanValue(1);
            }
            else
            {
                Leaning.CmdSetLeanValue(0);
            }
        }

        if (Input.GetButtonDown("Lean Right"))
        {
            if (Leaning.LeanInput != -1)
            {
                Leaning.CmdSetLeanValue(-1);
            }
            else
            {
                Leaning.CmdSetLeanValue(0);
            }

        }

        if (Input.GetButtonDown("Slow Walk"))
        {
            if (!SlowWalking.SlowWalk)
            {
                SlowWalking.CmdSetSlowWalk(true);
            }
            else
            {
                SlowWalking.CmdSetSlowWalk(false);
            }
        }
    }

    [Server]
    public void HandleInteractables()
    {
        if (InteractionReciever.InteractablesInRange.Count > 0)
        {
            PrimaryInteractable = InteractionReciever.InteractablesInRange[0];
        }
        else
        {
            PrimaryInteractable = null;
        }
    }

    

}
