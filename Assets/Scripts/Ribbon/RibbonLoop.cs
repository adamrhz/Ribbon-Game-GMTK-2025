using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RibbonLoop : MonoBehaviour
{

    public bool Looping = false;

    LinkedList<Vector3> ribbonPoints = new LinkedList<Vector3>();
    LineRenderer ribbonRenderer;
    float minLoopDistance = .15f;
    int minLoopPoints = 10;
    public float RibbonLength = 150f;
    
    public Vector3 lastLoopCenter;
    public float lastLoopRadius = 0f;

    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        ribbonRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) || Input.GetKeyUp(KeyCode.L))
        {
            ToggleLooping(Input.GetKey(KeyCode.L));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("Rigidbody component not found on " + gameObject.name);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(lastLoopCenter, lastLoopRadius);
    }
    void OnLoopClosed(List<Vector3> loopPoints)
    {
        Vector3 center = GetLoopCenter(loopPoints);
        float radius = EstimateLoopRadius(loopPoints, center);
        lastLoopCenter = center;
        lastLoopRadius = radius;
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        List<Ribbonables> ribbonables = Physics.OverlapSphere(center, radius)
        .Select(hit => hit.GetComponent<Ribbonables>())
        .Where(r => r != null && r.gameObject != gameObject)
        .ToList();


        foreach (var ribbonable in ribbonables)
        {
            Debug.Log("Hit: " + ribbonable.name);
            if (ribbonable.enabled)
            {
                ribbonable.OnLoopedEvent.Invoke(ribbonables);
            }
            ribbonable.enabled = true;
        }

        loopPoints.Clear();
    }
    bool TryDetectLoop()
    {
        LinkedListNode<Vector3> currentNode = ribbonPoints.First;
        List<Vector3> RibbonPointsArrayList = ribbonPoints.ToList();
        while (currentNode.Next != null)
        {

            if (currentNode.Value == ribbonPoints.Last.Value || currentNode.Value == ribbonPoints.Last.Previous.Value || currentNode.Value == ribbonPoints.Last.Previous.Previous.Value)
            {
                currentNode = currentNode.Next;
                continue;
            }
            if (Vector3.Distance(ribbonPoints.Last.Value, currentNode.Value) < minLoopDistance)
            {
                List<Vector3> loopPoints = ribbonPoints.ToList();
                OnLoopClosed(loopPoints);
                return true;
            }
            RibbonPointsArrayList.Remove(currentNode.Value);
            currentNode = currentNode.Next;
        }
        return false;
    }
    Vector3 GetLoopCenter(List<Vector3> loopPoints)
    {
        Vector3 sum = Vector3.zero;
        foreach (var point in loopPoints)
            sum += point;
        return sum / loopPoints.Count;
    }
    float EstimateLoopRadius(List<Vector3> loopPoints, Vector3 center)
    {
        float total = 0f;
        foreach (var point in loopPoints)
            total += Vector3.Distance(point, center);
        return total / loopPoints.Count;
    }


    public void ToggleLooping(bool toggleOn)
    {
        Looping = toggleOn;
        if (!Looping)
        {
            ribbonPoints.Clear();
            ribbonRenderer.positionCount = 0;
        }
    }
    public void FixedUpdate()
    {

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        rb.velocity = move * 10 + new Vector3(0, rb.velocity.y,0);




        if (Looping)
        {

            if(ribbonPoints.Count > 0 && Vector3.Distance(ribbonPoints.Last(), transform.position) < 0.1f)
            {
                return;
            }

            if(ribbonPoints.Count > RibbonLength)
            {
                ribbonPoints.RemoveFirst(); // Remove oldest point if we exceed a certain count
            }
            ribbonPoints.AddLast(transform.position);
            ribbonRenderer.positionCount = ribbonPoints.Count;
            ribbonRenderer.SetPositions(ribbonPoints.ToArray());
            if (TryDetectLoop())
            {
                ToggleLooping(false);
            }
        }
    }
}
