using UnityEngine;

public class BasePlayer : Entity
{

    public override void Start()
    {
        base.Start();

        if (!isLocalPlayer)
        {
            Cam.enabled = false;
            audioListener.enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void Update()
    {
        base.Update();

        if (!isLocalPlayer)
        {
            return;
        }

        MoveInput.x = Input.GetAxisRaw("Horizontal");
        MoveInput.y = Input.GetAxisRaw("Vertical");

        LookInput.x = Input.GetAxis("Mouse X");
        LookInput.y = Input.GetAxis("Mouse Y");

        
    }
}
