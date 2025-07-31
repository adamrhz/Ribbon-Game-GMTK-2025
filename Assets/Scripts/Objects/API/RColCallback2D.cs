using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles collision with objects and more. Courtesy of Strix
/// </summary>
namespace Ribbon
{
    public class RColCallback2D : MonoBehaviour
    {
        public delegate void CollisionEvent(UnityEngine.Collision2D Col);
        public delegate void TriggerEvent(Collider2D Col);

        public event CollisionEvent COnEnter, COnStay, COnExit;
        public event TriggerEvent TOnEnter, TOnStay, TOnExit;

        public void OnCollisionEnter2D(UnityEngine.Collision2D collision) => COnEnter?.Invoke(collision);
        public void OnCollisionStay2D(UnityEngine.Collision2D collision) => COnStay?.Invoke(collision);
        public void OnCollisionExit2D(UnityEngine.Collision2D collision) => COnExit?.Invoke(collision);

        public void OnTriggerEnter2D(Collider2D collider) => TOnEnter?.Invoke(collider);
        public void OnTriggerStay2D(Collider2D collider) => TOnStay?.Invoke(collider);
        public void OnTriggerExit2D(Collider2D collider) => TOnExit?.Invoke(collider);
    }
}