using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent OnInteract;


    public virtual void Interact()
    {
        OnInteract.Invoke();
    }
}
