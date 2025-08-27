using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : NetworkBehaviour
{
    public UnityEvent OnInteract;
    public float Range = 5;

    public virtual void Awake()
    {
        
    }

    [Command(requiresAuthority = false)]
    public virtual void Interact(NetworkConnectionToClient sender = null)
    {
        if (!ValidateInteract(sender))
        {
            return;
        }

        BaseInteractMethod();
    }

    [Server]
    public void BaseInteractMethod()
    {
        OnInteract.Invoke();
    }

    [Server]
    public bool ValidateInteract(NetworkConnectionToClient sender)
    {
        if (sender == null)
        {
            Debug.LogWarning(gameObject.name + ": Sender is null on interact");
            return false;
        }
        if (Vector3.Distance(transform.position, sender.identity.gameObject.transform.position) <= Range)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
