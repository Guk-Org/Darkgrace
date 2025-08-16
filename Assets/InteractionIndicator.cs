using UnityEngine;

public class InteractionIndicator : MonoBehaviour
{
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
    }

    private void Update()
    {
        transform.forward = canvas.worldCamera.transform.forward;
        transform.rotation = canvas.worldCamera.transform.rotation;
    }
}
