using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavAI : MonoBehaviour
{
    // ADDED InvestigateThrowable
    private enum State { Wander, Chase, Return, InvestigateThrowable }

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

    [Header("Investigate Throwable")]
    [Tooltip("Speed while investigating a thrown object.")]
    [SerializeField] private float investigateSpeed = 4.0f;

    [Tooltip("Stop this far from the throwable target.")]
    [SerializeField] private float investigateStopDistance = 0.8f;

    [Tooltip("How close counts as 'arrived' at the throwable.")]
    [SerializeField] private float investigateArriveDistance = 1.0f;

    [Tooltip("Optional: while investigating, repath this often (helps if throwable moves).")]
    [SerializeField] private float investigateRepathInterval = 0.25f;

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

    // NEW: investigate target
    private Transform _throwableTarget;
    private float _nextInvestigateRepathTime;

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
        // If priest hides, stop chasing / investigating and go back to wandering/return logic
        if (priestLogic != null && priestLogic.IsHiding)
        {
            // If chasing -> leave chase
            if (_state == State.Chase)
                ExitChase();
            // If investigating -> stop investigating and go wander
            if (_state == State.InvestigateThrowable)
                ExitInvestigateThrowable(toReturnHome: false);
        }
        else
        {
            // Normal detection: only start chase if not investigating
            if (_state != State.InvestigateThrowable &&
                player && Vector3.Distance(transform.position, player.position) <= detectRadius)
            {
                if (_state != State.Chase) EnterChase();
            }
        }

        switch (_state)
        {
            case State.Wander: TickWander(); break;
            case State.Chase: TickChase(); break;
            case State.Return: TickReturn(); break;
            case State.InvestigateThrowable: TickInvestigateThrowable(); break;
        }
    }

    // NEW: called by EnemyManager
    public void OnThrowableHeard(Transform throwable)
    {
        if (throwable == null) return;

        // Lose interest in player immediately
        if (_state == State.Chase)
            ExitChase();

        EnterInvestigateThrowable(throwable);
    }

    private void EnterInvestigateThrowable(Transform throwable)
    {
        _throwableTarget = throwable;
        _state = State.InvestigateThrowable;

        _agent.speed = investigateSpeed;
        _agent.stoppingDistance = investigateStopDistance;
        _agent.isStopped = false;

        _nextInvestigateRepathTime = 0f;
        _lingerTimer = 0f;
        _loseTimer = 0f;

        SetInvestigateDestination();
    }

    private void TickInvestigateThrowable()
    {
        if (_throwableTarget == null)
        {
            ExitInvestigateThrowable(toReturnHome: false);
            return;
        }

        float dist = Vector3.Distance(transform.position, _throwableTarget.position);

        // Stop before reaching the exact point
        if (dist <= investigateStopDistance)
        {
            if (!_agent.isStopped) _agent.isStopped = true;
        }
        else
        {
            if (_agent.isStopped) _agent.isStopped = false;

            if (Time.time >= _nextInvestigateRepathTime)
            {
                SetInvestigateDestination();
                _nextInvestigateRepathTime = Time.time + Mathf.Max(0.05f, investigateRepathInterval);
            }
        }

        // Arrived -> go back to wander
        if (!_agent.pathPending && _agent.remainingDistance <= investigateArriveDistance)
        {
            ExitInvestigateThrowable(toReturnHome: false);
        }
    }

    private void SetInvestigateDestination()
    {
        if (_throwableTarget == null) return;

        Vector3 pos = _throwableTarget.position;

        if (NavMesh.SamplePosition(pos, out var hit, 2.0f, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
        else
            _agent.SetDestination(pos);
    }

    private void ExitInvestigateThrowable(bool toReturnHome)
    {
        _throwableTarget = null;
        _agent.isStopped = false;
        _agent.stoppingDistance = 0f;

        if (toReturnHome && returnToHomeWhenLost)
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

        if (dist <= attackRange)
            TryAttack();

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
        _nextAttackTime = 0f;
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

        _state = State.Wander;

        _lingerTimer = 0f;
        _loseTimer = 0f;
        _nextChaseRepathTime = 0f;
        _nextAttackTime = 0f;

        _currentTarget = null;
        _throwableTarget = null;
        _nextInvestigateRepathTime = 0f;

        if (_agent)
        {
            _agent.isStopped = false;
            _agent.speed = wanderSpeed;
            _agent.stoppingDistance = 0f;

            _agent.ResetPath();
            _homePosition = transform.position;
        }

        BuildWaypointCache();
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
