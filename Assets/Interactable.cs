using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : NetworkBehaviour
{
    public UnityEvent OnInteract;

    public virtual void Awake()
    {
        
    }

    [Command(requiresAuthority = false)]
    public virtual void Interact()
    {
        OnInteract.Invoke();
    }
}
