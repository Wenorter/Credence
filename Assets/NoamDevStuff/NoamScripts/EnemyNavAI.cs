using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavAI : MonoBehaviour
{
    private enum State { Wander, Chase, Return }

    [Header("References")]
    [Tooltip("If left empty, the script finds an object tagged Priest.")]
    [SerializeField] private Transform player;
    [SerializeField] private GameManager gameManager;

    private NavMeshAgent _agent;
    private State _state = State.Wander;
    public PriestActions priestLogic;
    [Header("Waypoints per Room (set per enemy)")]
    [SerializeField] private RoomWaypoints[] roomWaypoints;

    public enum WanderPickMode { Random, Sequential }
    [SerializeField] private WanderPickMode pickMode = WanderPickMode.Random;

    [Tooltip("If true, keep choosing points from the current room unless we decide to switch.")]
    [SerializeField] private bool stickToCurrentRoom = true;

    [Range(0f, 1f)]
    [Tooltip("Only used if stickToCurrentRoom = true. Chance to switch rooms after reaching a point.")]
    [SerializeField] private float switchRoomChance = 0.25f;

    [Header("Wander / Linger")]
    [SerializeField] private float wanderSpeed = 2.5f;
    [SerializeField] private float arriveDistance = 0.6f;
    [SerializeField] private float minLingerTime = 0.6f;
    [SerializeField] private float maxLingerTime = 2.0f;

    [Header("Detection")]
    [SerializeField] private float detectRadius = 8f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4.5f;
    [SerializeField] private float loseRadius = 12f;
    [SerializeField] private float loseAfterSeconds = 2.0f;
    [SerializeField] private float chaseRepathInterval = 0.15f;

    [Header("Chase - Stop Before Target")]
    [Tooltip("How far from the target the enemy should stop while chasing (prevents pushing).")]
    [SerializeField] private float chaseStopDistance = 1.2f;

    [Header("Attack")]
    [Tooltip("Seconds between attacks while in range.")]
    [SerializeField] private float attackInterval = 1.0f;
    [Tooltip("Distance at which the enemy can attack. Usually <= chaseStopDistance.")]
    [SerializeField] private float attackRange = 1.2f;

    [Header("Return")]
    [SerializeField] private bool returnToHomeWhenLost = true;
    [SerializeField] private float returnSpeed = 3.0f;
    [SerializeField] private float returnArriveDistance = 0.8f;

    private Vector3 _homePosition;
    private float _lingerTimer;
    private float _loseTimer;
    private float _nextChaseRepathTime;
    private float _nextAttackTime;
    
    // runtime caches
    private readonly List<string> _availableRooms = new();
    private readonly Dictionary<string, List<Transform>> _pointsByRoom = new();
    private readonly Dictionary<string, int> _sequentialIndexByRoom = new();

    private string _currentRoom;
    private Transform _currentTarget;

    [Serializable]
    private class RoomWaypoints
    {
        public string room;
        public Transform[] points;
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _homePosition = transform.position;
    }

    private void Start()
    {
        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Priest");
            if (p) player = p.transform;
        }

        if (!gameManager)
            gameManager = FindFirstObjectByType<GameManager>();

        BuildWaypointCache();

        _agent.speed = wanderSpeed;
        _agent.stoppingDistance = 0f;
        _agent.autoBraking = true;

        PickNextWanderPoint(forceSwitchRoom: true);
    }

    private void Update()
    {
        if (priestLogic.IsHiding)
        {
            ExitChase();
        }
        else if (player && Vector3.Distance(transform.position, player.position) <= detectRadius)
        {
            if (_state != State.Chase) EnterChase();
        }

        switch (_state)
        {
            case State.Wander: TickWander(); break;
            case State.Chase:  TickChase();  break;
            case State.Return: TickReturn(); break;
        }
    }

    private void BuildWaypointCache()
    {
        _availableRooms.Clear();
        _pointsByRoom.Clear();
        _sequentialIndexByRoom.Clear();

        if (roomWaypoints == null) return;

        foreach (var rw in roomWaypoints)
        {
            if (rw?.points == null || rw.points.Length == 0) continue;

            if (!_pointsByRoom.TryGetValue(rw.room, out var list))
            {
                list = new List<Transform>();
                _pointsByRoom[rw.room] = list;
                _availableRooms.Add(rw.room);
                _sequentialIndexByRoom[rw.room] = -1;
            }

            foreach (var t in rw.points)
                if (t) list.Add(t);
        }

        if (_availableRooms.Count > 0)
            _currentRoom = _availableRooms[0];
    }

    private void TickWander()
    {
        if (_availableRooms.Count == 0)
        {
            _agent.SetDestination(_homePosition);
            return;
        }

        if (!_agent.hasPath || _agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            PickNextWanderPoint(forceSwitchRoom: !stickToCurrentRoom);
            return;
        }

        if (!_agent.pathPending && _agent.remainingDistance <= arriveDistance)
        {
            if (_lingerTimer <= 0f)
                _lingerTimer = UnityEngine.Random.Range(minLingerTime, maxLingerTime);

            _lingerTimer -= Time.deltaTime;

            if (_lingerTimer <= 0f)
            {
                var forceSwitch = !stickToCurrentRoom;
                if (stickToCurrentRoom && UnityEngine.Random.value < switchRoomChance)
                    forceSwitch = true;

                PickNextWanderPoint(forceSwitchRoom: forceSwitch);
            }
        }
    }

    private void TickChase()
    {
        if (!player)
        {
            ExitChase();
            return;
        }

        var playerPos = player.position;
        var dist = Vector3.Distance(transform.position, playerPos);

        // 1) Attack if in range and interval elapsed
        if (dist <= attackRange)
        {
            TryAttack();
        }

        // 2) Stop before target to avoid pushing
        if (dist <= chaseStopDistance)
        {
            if (!_agent.isStopped) _agent.isStopped = true;
        }
        else
        {
            if (_agent.isStopped) _agent.isStopped = false;

            if (Time.time >= _nextChaseRepathTime)
            {
                if (NavMesh.SamplePosition(playerPos, out var hit, 2.0f, NavMesh.AllAreas))
                    _agent.SetDestination(hit.position);
                else
                    _agent.SetDestination(playerPos);

                _nextChaseRepathTime = Time.time + chaseRepathInterval;
            }
        }

        // 3) Lose target logic
        if (dist > loseRadius)
        {
            _loseTimer += Time.deltaTime;
            if (_loseTimer >= loseAfterSeconds)
                ExitChase();
        }
        else
        {
            _loseTimer = 0f;
        }
    }

    private void TryAttack()
    {
        if (Time.time < _nextAttackTime) return;

        _nextAttackTime = Time.time + Mathf.Max(0.01f, attackInterval);

        // Damage is always 1, so we just notify the GameManager.
        if (gameManager)
            gameManager.OnPriestAttacked();
        else
            Debug.LogWarning($"{name}: No GameManager assigned/found. Attack happened but no one received it.");
    }

    private void TickReturn()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= returnArriveDistance)
        {
            _state = State.Wander;
            _agent.speed = wanderSpeed;
            _agent.stoppingDistance = 0f;
            _lingerTimer = 0f;

            PickNextWanderPoint(forceSwitchRoom: true);
        }
    }

    private void EnterChase()
    {
        _state = State.Chase;
        _agent.speed = chaseSpeed;
        _agent.stoppingDistance = chaseStopDistance;
        _agent.isStopped = false;

        _loseTimer = 0f;
        _nextChaseRepathTime = 0f;
        _nextAttackTime = 0f; // can attack immediately if in range
    }

    private void ExitChase()
    {
        _loseTimer = 0f;
        _agent.isStopped = false;
        _agent.stoppingDistance = 0f;

        if (returnToHomeWhenLost)
        {
            _state = State.Return;
            _agent.speed = returnSpeed;
            _agent.SetDestination(_homePosition);
        }
        else
        {
            _state = State.Wander;
            _agent.speed = wanderSpeed;
            PickNextWanderPoint(forceSwitchRoom: true);
        }
    }

    private void PickNextWanderPoint(bool forceSwitchRoom)
    {
        _lingerTimer = 0f;

        if (_availableRooms.Count == 0) return;

        if (forceSwitchRoom || !_pointsByRoom.ContainsKey(_currentRoom))
            _currentRoom = _availableRooms[UnityEngine.Random.Range(0, _availableRooms.Count)];

        if (!_pointsByRoom.TryGetValue(_currentRoom, out var points) || points.Count == 0)
            return;

        Transform next = null;

        if (pickMode == WanderPickMode.Random)
        {
            for (var tries = 0; tries < 10; tries++)
            {
                var candidate = points[UnityEngine.Random.Range(0, points.Count)];
                if (candidate && candidate != _currentTarget) { next = candidate; break; }
            }

            next ??= points[UnityEngine.Random.Range(0, points.Count)];
        }
        else
        {
            var idx = _sequentialIndexByRoom.TryGetValue(_currentRoom, out var v) ? v : -1;

            for (var i = 0; i < points.Count; i++)
            {
                idx = (idx + 1) % points.Count;
                if (points[idx] != null) { next = points[idx]; break; }
            }

            _sequentialIndexByRoom[_currentRoom] = idx;
            next ??= points[0];
        }

        if (!next) return;
        _currentTarget = next;

        if (NavMesh.SamplePosition(next.position, out var hit, 2.0f, NavMesh.AllAreas))
        {
            _agent.speed = wanderSpeed;
            _agent.stoppingDistance = 0f;
            _agent.isStopped = false;
            _agent.SetDestination(hit.position);
        }
        else
        {
            for (var i = 0; i < 10; i++)
            {
                var t = points[UnityEngine.Random.Range(0, points.Count)];
                if (t && NavMesh.SamplePosition(t.position, out hit, 2.0f, NavMesh.AllAreas))
                {
                    _currentTarget = t;
                    _agent.stoppingDistance = 0f;
                    _agent.isStopped = false;
                    _agent.SetDestination(hit.position);
                    return;
                }
            }
        }
    }
    public void OnResetDay(Transform newTransform)
    {
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
        // Forget runtime state
        _state = State.Wander;

        _lingerTimer = 0f;
        _loseTimer = 0f;
        _nextChaseRepathTime = 0f;
        _nextAttackTime = 0f;

        _currentTarget = null;

        // Reset agent movement to "fresh start"
        if (_agent)
        {
            _agent.isStopped = false;
            _agent.speed = wanderSpeed;
            _agent.stoppingDistance = 0f;

            // Clear current path
            _agent.ResetPath();

            // Treat current position as the new "home" for this day (optional, but usually matches "reload")
            _homePosition = transform.position;
        }

        // Rebuild cache in case rooms/points changed (safe even if unchanged)
        BuildWaypointCache();

        // Pick a new starting wander point like Start()
        PickNextWanderPoint(forceSwitchRoom: true);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        if (roomWaypoints == null) return;

        foreach (var rw in roomWaypoints)
        {
            if (rw?.points == null) continue;
            foreach (var t in rw.points)
                if (t) Gizmos.DrawSphere(t.position, 0.12f);
        }
    }
}
