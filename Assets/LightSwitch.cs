using UnityEngine;

public class LightSwitch : Interactable
{
    public LightGroup LightGroup;

    public override void Interact()
    {
        base.Interact();
        LightGroup.CmdToggle();
    }
}
