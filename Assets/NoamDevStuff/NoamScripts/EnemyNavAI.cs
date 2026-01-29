using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavAI : MonoBehaviour
{
    private enum State { Wander, Chase, Return }

    [Header("References")]
    [Tooltip("If left empty, the script finds an object tagged Player.")]
    public Transform player;

    private NavMeshAgent agent;
    private State state = State.Wander;

    [Header("Waypoints per Room (set per enemy)")]
    public RoomWaypoints[] roomWaypoints;

    public enum WanderPickMode { Random, Sequential }
    public WanderPickMode pickMode = WanderPickMode.Random;

    [Tooltip("If true, keep choosing points from the current room unless we decide to switch.")]
    public bool stickToCurrentRoom = true;

    [Range(0f, 1f)]
    [Tooltip("Only used if stickToCurrentRoom = true. Chance to switch rooms after reaching a point.")]
    public float switchRoomChance = 0.25f;

    [Header("Wander / Linger")]
    public float wanderSpeed = 2.5f;
    public float arriveDistance = 0.6f;
    public float minLingerTime = 0.6f;
    public float maxLingerTime = 2.0f;

    [Header("Detection")]
    public float detectRadius = 8f;

    [Header("Chase")]
    public float chaseSpeed = 4.5f;
    public float loseRadius = 12f;
    public float loseAfterSeconds = 2.0f;
    public float chaseRepathInterval = 0.15f;

    [Header("Return")]
    public bool returnToHomeWhenLost = true;
    public float returnSpeed = 3.0f;
    public float returnArriveDistance = 0.8f;

    private Vector3 homePosition;
    private float lingerTimer = 0f;
    private float loseTimer = 0f;
    private float nextChaseRepathTime = 0f;

    // runtime caches
    private readonly List<string> availableRooms = new();
    private readonly Dictionary<string, List<Transform>> pointsByRoom = new();
    private readonly Dictionary<string, int> sequentialIndexByRoom = new();

    private string currentRoom;
    private Transform currentTarget;

    [Serializable]
    public class RoomWaypoints
    {
        public string room;
        public Transform[] points;
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        homePosition = transform.position;
    }

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Priest");
            if (p != null) player = p.transform;
        }

        BuildWaypointCache();

        agent.speed = wanderSpeed;
        agent.stoppingDistance = 0f;
        agent.autoBraking = true;

        PickNextWanderPoint(forceSwitchRoom: true);
    }

    void Update()
    {
        // Detect -> Chase
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectRadius)
        {
            if (state != State.Chase) EnterChase();
        }

        switch (state)
        {
            case State.Wander: TickWander(); break;
            case State.Chase:  TickChase();  break;
            case State.Return: TickReturn(); break;
        }
    }

    private void BuildWaypointCache()
    {
        availableRooms.Clear();
        pointsByRoom.Clear();
        sequentialIndexByRoom.Clear();

        if (roomWaypoints == null) return;

        foreach (var rw in roomWaypoints)
        {
            if (rw == null || rw.points == null || rw.points.Length == 0) continue;

            if (!pointsByRoom.TryGetValue(rw.room, out var list))
            {
                list = new List<Transform>();
                pointsByRoom[rw.room] = list;
                availableRooms.Add(rw.room);
                sequentialIndexByRoom[rw.room] = -1;
            }

            foreach (var t in rw.points)
                if (t != null) list.Add(t);
        }

        if (availableRooms.Count > 0)
            currentRoom = availableRooms[0];
    }

    private void TickWander()
    {
        if (availableRooms.Count == 0)
        {
            // no points assigned
            agent.SetDestination(homePosition);
            return;
        }

        // If current path is invalid/unreachable, pick another point
        if (!agent.hasPath || agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            PickNextWanderPoint(forceSwitchRoom: !stickToCurrentRoom);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= arriveDistance)
        {
            if (lingerTimer <= 0f)
                lingerTimer = UnityEngine.Random.Range(minLingerTime, maxLingerTime);

            lingerTimer -= Time.deltaTime;

            if (lingerTimer <= 0f)
            {
                bool forceSwitch = !stickToCurrentRoom;
                if (stickToCurrentRoom && UnityEngine.Random.value < switchRoomChance)
                    forceSwitch = true;

                PickNextWanderPoint(forceSwitchRoom: forceSwitch);
            }
        }
    }

    private void TickChase()
    {
        if (player == null)
        {
            ExitChase();
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        if (Time.time >= nextChaseRepathTime)
        {
            agent.SetDestination(player.position);
            nextChaseRepathTime = Time.time + chaseRepathInterval;
        }

        if (dist > loseRadius)
        {
            loseTimer += Time.deltaTime;
            if (loseTimer >= loseAfterSeconds)
                ExitChase();
        }
        else
        {
            loseTimer = 0f;
        }
    }

    private void TickReturn()
    {
        if (!agent.pathPending && agent.remainingDistance <= returnArriveDistance)
        {
            state = State.Wander;
            agent.speed = wanderSpeed;
            lingerTimer = 0f;
            PickNextWanderPoint(forceSwitchRoom: true);
        }
    }

    private void EnterChase()
    {
        state = State.Chase;
        agent.speed = chaseSpeed;
        loseTimer = 0f;
        nextChaseRepathTime = 0f; // repath immediately
    }

    private void ExitChase()
    {
        loseTimer = 0f;

        if (returnToHomeWhenLost)
        {
            state = State.Return;
            agent.speed = returnSpeed;
            agent.SetDestination(homePosition);
        }
        else
        {
            state = State.Wander;
            agent.speed = wanderSpeed;
            PickNextWanderPoint(forceSwitchRoom: true);
        }
    }

    private void PickNextWanderPoint(bool forceSwitchRoom)
    {
        lingerTimer = 0f;

        if (availableRooms.Count == 0) return;

        // choose room
        if (forceSwitchRoom || !pointsByRoom.ContainsKey(currentRoom))
            currentRoom = availableRooms[UnityEngine.Random.Range(0, availableRooms.Count)];

        if (!pointsByRoom.TryGetValue(currentRoom, out var points) || points.Count == 0)
            return;

        Transform next = null;

        if (pickMode == WanderPickMode.Random)
        {
            // avoid picking the same target if possible
            for (int tries = 0; tries < 10; tries++)
            {
                var candidate = points[UnityEngine.Random.Range(0, points.Count)];
                if (candidate != null && candidate != currentTarget) { next = candidate; break; }
            }
            next ??= points[UnityEngine.Random.Range(0, points.Count)];
        }
        else // Sequential (per room)
        {
            int idx = sequentialIndexByRoom.TryGetValue(currentRoom, out var v) ? v : -1;

            for (int i = 0; i < points.Count; i++)
            {
                idx = (idx + 1) % points.Count;
                if (points[idx] != null) { next = points[idx]; break; }
            }

            sequentialIndexByRoom[currentRoom] = idx;
            next ??= points[0];
        }

        if (next == null) return;
        currentTarget = next;

        // Keep destination on navmesh
        if (NavMesh.SamplePosition(next.position, out var hit, 2.0f, NavMesh.AllAreas))
        {
            agent.speed = wanderSpeed;
            agent.SetDestination(hit.position);
        }
        else
        {
            // If waypoint isn't on/near navmesh, try again
            for (int i = 0; i < 10; i++)
            {
                var t = points[UnityEngine.Random.Range(0, points.Count)];
                if (t != null && NavMesh.SamplePosition(t.position, out hit, 2.0f, NavMesh.AllAreas))
                {
                    currentTarget = t;
                    agent.SetDestination(hit.position);
                    return;
                }
            }
        }
    }

    // Optional: visualize in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        Gizmos.color = Color.yellow;
        if (roomWaypoints == null) return;
        foreach (var rw in roomWaypoints)
        {
            if (rw?.points == null) continue;
            foreach (var t in rw.points)
                if (t != null) Gizmos.DrawSphere(t.position, 0.12f);
        }
    }
}
