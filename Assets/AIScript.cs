using UnityEngine;
using System.Collections;
using Pathfinding;

public class AIScript : MonoBehaviour
{
    public Transform target;
    void Start()
    {
        Seeker seeker = GetComponent<Seeker>();
        seeker.StartPath(transform.position, target.position);
    }
}