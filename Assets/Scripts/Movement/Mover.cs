using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private Transform _waypointsParent;
    [SerializeField, Min(0)] private float _speed = 4f;

    private List<Waypoint> _waypoints;

    private int _currentWaypointNumber;

    private void Awake()
    {
        GetWaypoints();
    }

    private void Start()
    {
        if (_waypoints != null)
            StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (isActiveAndEnabled)
        {
            Vector3 targetWaypointPosition = ChooseNextTargetWaypoint().transform.position;

            transform.forward = targetWaypointPosition - transform.position;

            while (transform.position != targetWaypointPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWaypointPosition, _speed * Time.deltaTime);

                yield return null;
            }
        }
    }

    private Waypoint ChooseNextTargetWaypoint()
    {
        Waypoint waypoint = _waypoints[_currentWaypointNumber];

        _currentWaypointNumber++;

        if (_currentWaypointNumber == _waypoints.Count)
            _currentWaypointNumber = 0;

        return waypoint;
    }

    private void GetWaypoints()
    {
        _waypoints = new List<Waypoint>();

        for (int i = 0; i < _waypointsParent.childCount; i++)
        {
            if (_waypointsParent.GetChild(i).TryGetComponent<Waypoint>(out Waypoint waypoint))
                _waypoints.Add(waypoint);
        }
    }
}