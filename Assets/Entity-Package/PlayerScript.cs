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

    private AudioSource localSoundSource;

    public AudioClip InteractionIndicatorAppear;
    public AudioClip InteractionIndicatorDisappear;

    public override void Start()
    {
        base.Start();
        interactionReciever = GetComponent<InteractionReciever>();
        uiCamera = gameObject.FindObject("UI Camera").GetComponent<Camera>();
        localSoundSource = gameObject.FindObject("Local Sound Source").GetComponent<AudioSource>();
    }

    public override void Update()
    {
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
                interactionIndicatorCanvas.transform.position = interactable.transform.position;
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
                leaning.SetLeanValue(1);
            }
            else
            {
                leaning.SetLeanValue(0);
            }
        }

        if (Input.GetButtonDown("Lean Right"))
        {
            if (leaning.LeanInput != -1)
            {
                leaning.SetLeanValue(-1);
            }
            else
            {
                leaning.SetLeanValue(0);
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
