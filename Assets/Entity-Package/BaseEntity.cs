using DG.Tweening;
using Mirror;
using UnityEngine;
public class BaseEntity : NetworkBehaviour
{
    protected Rigidbody rb;
    public bool AutoDetectHead = true;
    public Transform Head;
    public Camera Cam;
    protected AudioListener audioListener;


    public Vector2 MoveInput;
    [Header("Running")]
    public float RunSpeed = 6f;
    public float RunAcceleration = 33f;
    public bool Running;
    [Header("Walking")]
    public float WalkSpeed = 4.5f;
    public float WalkAcceleration = 55f;

    public float SpeedSmoothing = 7;
    public float AccelerationSmoothing = 5;

    public float CurrentSpeed;
    public float TargetSpeed;
    public float CurrentAcceleration;
    public float TargetAcceleration;



    public Vector2 LookInput;
    public float LookSensitivity = 155;

    protected float headXRotation;



    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cam = GetComponentInChildren<Camera>();
        audioListener = Cam.gameObject.GetComponent<AudioListener>();

        if (AutoDetectHead)
        {
            Head = gameObject.FindObject("Head").transform;
        }
        headXRotation = Head.localRotation.x;

        TargetSpeed = WalkSpeed;
        TargetAcceleration = WalkAcceleration;

        CurrentSpeed = TargetSpeed;
        CurrentAcceleration = TargetAcceleration;

    }

    public virtual void Update()
    {
        transform.rotation *= Quaternion.Euler(0, LookInput.x * LookSensitivity * Time.deltaTime, 0);
        headXRotation = Mathf.Clamp(headXRotation - LookInput.y * LookSensitivity * Time.deltaTime, -90, 90);
        Head.transform.localRotation = Quaternion.Euler(headXRotation, Head.transform.localRotation.eulerAngles.y, Head.transform.localRotation.eulerAngles.z);
    }

    public virtual void FixedUpdate()
    {
        CurrentSpeed = Mathf.Lerp(CurrentSpeed, TargetSpeed, SpeedSmoothing * Time.fixedDeltaTime);
        CurrentAcceleration = Mathf.Lerp(CurrentAcceleration, TargetAcceleration, AccelerationSmoothing * Time.fixedDeltaTime);

        Vector3 inputDir = new Vector3(MoveInput.x, 0f, MoveInput.y).normalized;
        Vector3 targetVel = (transform.forward * inputDir.z + transform.right * inputDir.x) * CurrentSpeed;

        Vector3 currentVel = rb.linearVelocity;
        Vector3 currentHoriz = new Vector3(currentVel.x, 0f, currentVel.z);

        Vector3 newHoriz = Vector3.MoveTowards(
            currentHoriz,
            targetVel,
            CurrentAcceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(newHoriz.x, currentVel.y, newHoriz.z);
    }

    public virtual void Movement()
    {
        if (!Running)
        {
            TargetSpeed = WalkSpeed;
            TargetAcceleration = WalkAcceleration;
        }
        else
        {
            TargetSpeed = RunSpeed;
            TargetAcceleration = RunAcceleration;
        }
    }
}
