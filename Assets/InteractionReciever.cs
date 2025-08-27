using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionReciever : NetworkBehaviour
{
    public List<Interactable> InteractablesInRange = new List<Interactable>();
    public LayerMask GroundLayer;
    public Interactable PrimaryInteractable;

    public void OnEnable()
    {
        StartCoroutine(CheckRoutine());
    }

    public void OnDisable()
    {
        InteractablesInRange.Clear();
        StopAllCoroutines();
    }

    [Client]
    private IEnumerator CheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (authority)
            {
                CheckForInteractables();
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void CheckForInteractables()
    {
        InteractablesInRange.Clear();
        Interactable[] interactablesFound = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        for (int i = 0; i < interactablesFound.Length; i++)
        {
            Interactable interactable = interactablesFound[i];
            if (Vector3.Distance(transform.position, interactable.gameObject.transform.position) <= interactable.Range)
            {
                if (!Physics.Linecast(transform.position, interactable.gameObject.transform.position, GroundLayer))
                {
                    InteractablesInRange.Add(interactable);
                }
            }
        }

        InteractablesInRange.Sort((a, b) =>
        Vector3.Distance(transform.position, a.transform.position)
        .CompareTo(Vector3.Distance(transform.position, b.transform.position)));
    }

}
