using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ribbonables : MonoBehaviour
{
    
    public UnityEvent<List<Ribbonables>> OnLoopedEvent = new UnityEvent<List<Ribbonables>>();


    public virtual void Awake()
    {
        //OnLoopedEvent.AddListener(OnLooped);
    }

    public virtual void OnLooped(List<Ribbonables> ribbonables)
    {
        int count = 1;
        foreach(Ribbonables ribbonable in ribbonables)
        {
            Debug.Log("Looped with: " + ribbonable.gameObject.name);
            if(ribbonable != this)
            {
                ribbonable.transform.position = transform.position + new Vector3(0, count, 0);
                count++;
                ribbonable.enabled = false;

            }
            if (ribbonable.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.velocity = Vector3.zero;
            }
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public void FuckingFly()
    {
        if(TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Rigidbody component not found on " + gameObject.name);
        }
    }
}
