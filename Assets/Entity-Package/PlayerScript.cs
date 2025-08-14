using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerScript : BasePlayer
{
    private InteractionReciever interactionReciever;
    public GameObject InteractionIndicatorPrefab;

    public override void Start()
    {
        base.Start();
        
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
            GameObject indicator = Instantiate(InteractionIndicatorPrefab);
            indicator.transform.position = interactable.transform.position;
        }
        else
        {

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
