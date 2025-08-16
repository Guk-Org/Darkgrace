using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionReciever : MonoBehaviour
{
    public List<Interactable> InteractablesInRange = new List<Interactable>();
    public float Range = 5;
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

    private IEnumerator CheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            CheckForInteractables();
        }
    }

    public void CheckForInteractables()
    {
        InteractablesInRange.Clear();
        Interactable[] interactablesFound = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        for (int i = 0; i < interactablesFound.Length; i++)
        {
            Interactable interactable = interactablesFound[i];
            if (Vector3.Distance(transform.position, interactable.gameObject.transform.position) <= Range)
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
