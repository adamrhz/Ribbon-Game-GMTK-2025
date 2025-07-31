using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float Speed = 1.0f; // Speed of the platform movement
    public Vector3[] Positions;

    private Rigidbody2D Rb;
    private LinkedList<Vector3> _movePositionsLinkedList = new LinkedList<Vector3>();
    private LinkedListNode<Vector3> _targetPosition;
    private Vector3 _startPosition;
    // Start is called before the first frame update
    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        _startPosition = transform.position;
        _movePositionsLinkedList = new LinkedList<Vector3>((Positions).ToList());
        _movePositionsLinkedList.AddFirst(Vector3.zero);
        _targetPosition = _movePositionsLinkedList.First;
    }


    private void OnDrawGizmosSelected()
    {
        Vector3 startPos = _startPosition;
        if (!Application.isPlaying)
        {
            startPos = transform.position;
        }
        List<Vector3> pos = new List<Vector3>();
        pos.Add(Vector3.zero);
        pos.AddRange(Positions);
        pos.Add(Vector3.zero);

        if (pos.Count == 0)
        {
            return;
        }
        Vector3 position = pos[0];

        foreach(Vector3 p in pos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_startPosition + position, _startPosition+p);
            Gizmos.DrawSphere(p, .1f);
            position = p;
        }







    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_movePositionsLinkedList.Count == 0)
        {
            return;
        }

        if(Vector3.Distance(transform.position, _startPosition+_targetPosition.Value) < 0.05f)
        {
            transform.position = _startPosition + _targetPosition.Value;
            _targetPosition = GetNextElement();
        }
        else
        {
            Rb.velocity = Speed * ((_startPosition + _targetPosition.Value) - transform.position).normalized;
        }
    }

    private LinkedListNode<Vector3> GetNextElement()
    {
        if (_targetPosition.Next != null)
        {
            return _targetPosition.Next;
        }
        else
        {
            return _movePositionsLinkedList.First;
        }
    }
}




