using Mirror;
using UnityEngine;

public class RaytracedAudioListener : NetworkBehaviour
{


    public GameObject RaytracedAudioClonePrefab;

    public int RaysToShoot = 30;
    public int MaxBounces = 5;

    void Start()
    {
        
    }

    
    void FixedUpdate()
    {
        
    }

    public void FindAudioSources()
    {
        float angle = 0;

        for (int i = 0; i < RaysToShoot; i++)
        {
            float x = Mathf.Sin(angle);
            float y = Mathf.Cos(angle);
            angle += 2 * Mathf.PI / RaysToShoot;

            Vector3 dir = new Vector3(transform.position.x + x, transform.position.y + y, 0);
            RaycastHit hit;
            Debug.DrawLine(transform.position, dir, Color.red);
            if (Physics.Raycast(transform.position, dir, out hit))
            {

            }
        }
    }
}
