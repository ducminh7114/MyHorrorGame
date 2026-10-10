using UnityEngine;
using UnityEngine.AI;

public class EnemyJumpscareAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float slowWalkSpeed = 1.2f;
    [SerializeField] private float patrolDistance = 10f;
    private Vector3 patrolTarget;

    [SerializeField] private float chaseSpeed = 4.5f;
    [SerializeField] private float stoppingDistance = 1.2f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Ignore Timer")]
    [SerializeField] private float timeBeforeChase = 8f;

    [Header("Jumpscare")]
    [SerializeField] private JumpscareController jumpscareController;
    [SerializeField] private float disappearDelay = 2f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float lookAtPlayerDistance = 2f;

    private NavMeshAgent agent;
    private float ignoreTimer;
    private bool chasing;
    private bool jumpscareTriggered;

    // Prevent triggering the same animation repeatedly
    private enum AnimationState
    {
        Idle,
        SadWalk,
        Run
    }

    private AnimationState currentAnimation;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }
    }


    private void Start()
    {
        agent.speed = slowWalkSpeed;
        agent.stoppingDistance = stoppingDistance;
        agent.updateRotation = false;

        patrolTarget =
            transform.position +
            transform.forward * patrolDistance;

        // Start with Idle
        PlayIdle();
    }


    private void Update()
    {
        if (jumpscareTriggered || player == null || agent == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        ignoreTimer += Time.deltaTime;

        if (!chasing)
        {
            WalkStraight();

            if (ignoreTimer >= timeBeforeChase)
                StartChase();

            return;
        }

        // Chase player
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance <= lookAtPlayerDistance)
        {
            RotateTowardsPlayer();
        }
        else
        {
            RotateTowardsPath();
        }

        // Run animation
        PlayRun();
    }


    private void WalkStraight()
    {
        Debug.Log("Enemy is walking - triggering SadWalk");

        agent.speed = slowWalkSpeed;

        if (!agent.hasPath || agent.remainingDistance < 0.5f)
        {
            patrolTarget =
                transform.position +
                transform.forward * patrolDistance;

            agent.SetDestination(patrolTarget);
        }

        RotateTowardsMovement();

        PlaySadWalk();
    }


    // =========================================================
    // ANIMATION
    // =========================================================

    private void PlayIdle()
    {
        if (animator == null)
            return;

        if (currentAnimation == AnimationState.Idle)
            return;

        animator.ResetTrigger("SadWalk");
        animator.ResetTrigger("Run");

        animator.SetTrigger("Idle");

        currentAnimation = AnimationState.Idle;
    }


    private void PlaySadWalk()
    {
        if (animator == null)
            return;

        if (currentAnimation == AnimationState.SadWalk)
            return;

        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Run");

        animator.SetTrigger("SadWalk");

        currentAnimation = AnimationState.SadWalk;
    }


    private void PlayRun()
    {
        if (animator == null)
            return;

        if (currentAnimation == AnimationState.Run)
            return;

        animator.ResetTrigger("Idle");
        animator.ResetTrigger("SadWalk");

        animator.SetTrigger("Run");

        currentAnimation = AnimationState.Run;
    }


    private void RotateTowardsMovement()
    {
        Vector3 direction = agent.desiredVelocity;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.05f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * 180f * Time.deltaTime
        );
    }


    private void StartChase()
    {
        chasing = true;

        agent.speed = chaseSpeed;

        // Change from Sad Walk → Run
        PlayRun();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (jumpscareTriggered)
            return;

        if (other.CompareTag("Player"))
        {
            TriggerJumpscare();
        }
    }


    private void RotateTowardsPath()
    {
        if (agent.pathPending)
            return;

        if (agent.path.corners.Length < 2)
        {
            RotateTowardsMovement();
            return;
        }

        Vector3 targetCorner = agent.path.corners[1];

        Vector3 direction =
            targetCorner - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.05f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * 180f * Time.deltaTime
        );
    }


    private void RotateTowardsPlayer()
    {
        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.05f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * 180f * Time.deltaTime
        );
    }


    private void OnEnable()
    {
        jumpscareTriggered = false;
        chasing = false;
        ignoreTimer = 0f;

        currentAnimation = (AnimationState)(-1);

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = slowWalkSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = false;

            if (agent.isOnNavMesh)
                agent.ResetPath();
        }

        if (animator != null)
        {
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("SadWalk");
            animator.ResetTrigger("Run");

            animator.SetTrigger("Idle");
        }
    }

    private void TriggerJumpscare()
    {
        jumpscareTriggered = true;

        agent.isStopped = true;

        // Stop and return to Idle
        PlayIdle();

        if (jumpscareController != null)
            jumpscareController.PlayJumpscare();

        Invoke(nameof(Disappear), disappearDelay);
    }


    private void Disappear()
    {
        gameObject.SetActive(false);
    }
}