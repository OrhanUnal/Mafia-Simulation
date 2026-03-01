using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCBase : MonoBehaviour, IInteractable
{
    [Header("NPC Data")]
    public NPCInfosSO info;

    [Header("Patrol Settings")]
    [Tooltip("Leave empty if you want the NPC to stand still.")]
    [SerializeField] private List<Transform> activePatrolPoints;
    [SerializeField] private List<Transform> idlePatrolPoints;
    [SerializeField] private float waitTimeAtPoint = 2f;

    private NavMeshAgent _agent;
    private Transform _player;
    private List<Transform> currentPatrolPoints;
    private int _currentPatrolIndex = 0;
    private float _waitTimer = 0f;
    private bool _isWaiting = false;
    private bool _inDialogue = false;

    protected void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    protected void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
        else
            Debug.LogWarning($"[{info.npcID}] Could not find Player tag in scene.");
        SetPatrolPoints();
    }

    protected void Update()
    {
        if (_inDialogue) return;
        HandlePatrol();
    }
    #region PatrolCode
    private bool HasRoute() => currentPatrolPoints != null && currentPatrolPoints.Count > 0;

    private void SetPatrolPoints()
    {
        //if(info.ShouldApproachOnDay(DayCycleManager.Instance.currentDay))
            //currentPatrolPoints = activePatrolPoints;
        //else
            currentPatrolPoints = idlePatrolPoints;
        _currentPatrolIndex = 0;
        if (!HasRoute())
        {
            _agent.ResetPath();
        }
        GoToNextPatrolPoint();
    }

    private void HandlePatrol()
    {
        if (!HasRoute()) return;
        if (_agent.pathPending) return;
        if (_agent.remainingDistance > _agent.stoppingDistance) return;

        if (!_isWaiting)
        {
            _isWaiting = true;
            _waitTimer = waitTimeAtPoint;
        }

        _waitTimer -= Time.deltaTime;
        if (_waitTimer <= 0f)
        {
            _isWaiting = false;
            GoToNextPatrolPoint();
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (!HasRoute()) return;
        _agent.SetDestination(currentPatrolPoints[_currentPatrolIndex].position);
        _currentPatrolIndex = (_currentPatrolIndex + 1) % currentPatrolPoints.Count;
    }
    #endregion
    #region DialogueCode
    public virtual void OnDayChanged(int newDay)
    {
        if (_inDialogue) return;
        SetPatrolPoints();
    }

    public virtual void InteractLogic()
    {
        StartDialogue();
    }

    protected void StartDialogue()
    {
        if (string.IsNullOrEmpty(info.conversationTitle))
        {
            Debug.LogWarning($"[{info.npcID}] No conversation title set in SO!");
            return;
        }

        _inDialogue = true;
        _agent.ResetPath();
        FacePlayer();

        DialogueManager.StartConversation(info.conversationTitle, transform);
    }

    protected virtual void OnConversationEnd(Transform actor)
    {
        _inDialogue = false;

        if (HasRoute())
            GoToNextPatrolPoint();
    }

    private void FacePlayer()
    {
        if (_player == null) return;
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    #endregion
}