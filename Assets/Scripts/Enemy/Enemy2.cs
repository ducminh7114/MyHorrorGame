using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy2 : MonoBehaviour
{
    public enum ChaseMode
    {
        WalkOnly,
        TeleportOnly,
        WalkAndTeleport
    }

    [Header("Preferences")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Chase Mode")]
    public ChaseMode chaseMode = ChaseMode.WalkAndTeleport;

    [Header("Teleport")]
    public float teleportDistance = 5f;

    [Tooltip("How often the enemy teleports.")]
    public float teleportInterval = 3f;

    [Tooltip("Only used for WalkAndTeleport mode.")]
    public float navMeshSearchDistance = 2f;

    [Header("Walk")]
    public float StoppingDistance = 2f;

    [Header("Catch")]
    public float catchDistance = 1.2f;

    [Header("Kill Animation")]
    public string killAnimationState = "stab";

    [Header("Death UI")]
    public GameObject loseUI;
    public CanvasGroup fadeCanvas;

    [Tooltip("How long it takes to fade to black.")]
    public float fadeDuration = 2f;

    [Tooltip("Delay before YOU DIED appears.")]
    public float loseUIDelay = 0.3f;

    [Header("Player Movement")]
    public MonoBehaviour playerMovement;

    [Header("Death Cameras")]
    public Camera mainCamera;
    public Camera killCamera;

    private float teleportTimer;
    private bool caught = false;

    void Start()
    {
        // Get components automatically if not assigned
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Kill camera starts disabled
        if (killCamera != null)
        {
            killCamera.enabled = false;
        }

        // YOU DIED starts hidden
        if (loseUI != null)
        {
            loseUI.SetActive(false);
        }

        // Fade starts transparent
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
            fadeCanvas.blocksRaycasts = false;
        }

        // Start teleport timer
        teleportTimer = teleportInterval;

        UpdateMovementMode();
    }

    void Update()
    {
        if (player == null || caught)
            return;

        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        // =========================================
        // PLAYER ALREADY CLOSE ENOUGH
        // =========================================

        if (distanceToPlayer <= catchDistance)
        {
            CatchPlayer();
            return;
        }

        // =========================================
        // WALK
        // =========================================

        if (chaseMode == ChaseMode.WalkOnly ||
            chaseMode == ChaseMode.WalkAndTeleport)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.stoppingDistance = StoppingDistance;
                agent.SetDestination(player.position);
            }
        }

        // =========================================
        // TELEPORT
        // =========================================

        if (chaseMode == ChaseMode.TeleportOnly ||
            chaseMode == ChaseMode.WalkAndTeleport)
        {
            teleportTimer -= Time.deltaTime;

            if (teleportTimer <= 0f)
            {
                if (chaseMode == ChaseMode.TeleportOnly)
                {
                    TeleportDirectlyToPlayer();
                }
                else
                {
                    TeleportTowardPlayer();
                }

                teleportTimer = teleportInterval;
            }
        }
    }

    void UpdateMovementMode()
    {
        if (agent == null)
            return;

        if (chaseMode == ChaseMode.TeleportOnly)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
        else
        {
            agent.isStopped = false;
        }
    }

    // =========================================================
    // TELEPORT ONLY
    // =========================================================

    void TeleportDirectlyToPlayer()
    {
        if (player == null || caught)
            return;

        // Direction from enemy toward player
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            CatchPlayer();
            return;
        }

        direction.Normalize();

        // How far the enemy moves toward the player THIS teleport
        float currentDistance =
            Vector3.Distance(transform.position, player.position);

        // If the next teleport would put the enemy inside
        // the catch radius, place it just inside the catch radius.
        float targetDistance;

        if (currentDistance - teleportDistance <= catchDistance)
        {
            targetDistance = catchDistance * 0.5f;
        }
        else
        {
            targetDistance = currentDistance - teleportDistance;
        }

        // Position at targetDistance from player
        Vector3 targetPosition =
            player.position - direction * targetDistance;

        bool teleported = false;

        // Try NavMesh first
        if (agent != null && agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(
                targetPosition,
                out NavMeshHit hit,
                navMeshSearchDistance,
                NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                teleported = true;
            }
        }

        // If NavMesh couldn't find a suitable point,
        // directly move the enemy.
        if (!teleported)
        {
            transform.position = targetPosition;

            if (agent != null)
            {
                agent.nextPosition = transform.position;
            }
        }

        FacePlayer();

        // Check AFTER teleporting
        float newDistance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        Debug.Log(
            "Enemy teleported. " +
            "Distance: " + newDistance
        );

        // =========================================
        // CATCH
        // =========================================

        if (newDistance <= catchDistance)
        {
            CatchPlayer();
        }
    }

    // =========================================================
    // WALK + TELEPORT
    // =========================================================

    void TeleportTowardPlayer()
    {
        if (player == null || caught)
            return;

        if (agent == null || !agent.isOnNavMesh)
            return;

        Vector3 direction =
            (player.position - transform.position).normalized;

        Vector3 targetPosition =
            transform.position + direction * teleportDistance;

        if (NavMesh.SamplePosition(
            targetPosition,
            out NavMeshHit hit,
            navMeshSearchDistance,
            NavMesh.AllAreas))
        {
            float currentDistance =
                Vector3.Distance(
                    transform.position,
                    player.position);

            float newDistance =
                Vector3.Distance(
                    hit.position,
                    player.position);

            // Only teleport closer.
            if (newDistance < currentDistance)
            {
                agent.Warp(hit.position);

                FacePlayer();

                // IMPORTANT:
                // If teleporting brought the enemy into
                // catch distance, immediately kill player.
                if (Vector3.Distance(
                    transform.position,
                    player.position) <= catchDistance)
                {
                    CatchPlayer();
                }
            }
        }
    }

    // =========================================================
    // FACE PLAYER
    // =========================================================

    void FacePlayer()
    {
        if (player == null)
            return;

        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }
    }

    // =========================================================
    // CATCH PLAYER
    // =========================================================

    void CatchPlayer()
    {
        if (caught)
            return;

        caught = true;

        Debug.Log("PLAYER CAUGHT - STARTING KILL SEQUENCE");

        // Stop enemy
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        // Stop player
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Face player
        FacePlayer();

        // Start death sequence
        StartCoroutine(KillSequence());
    }

    // =========================================================
    // KILL SEQUENCE
    // =========================================================

    IEnumerator KillSequence()
    {
        // Small delay
        yield return new WaitForSeconds(0.1f);

        // =========================================
        // 1. PLAY KILL ANIMATION
        // =========================================

        if (animator != null)
        {
            Debug.Log(
                "Playing animation: " +
                killAnimationState
            );

            animator.CrossFade(
                killAnimationState,
                0.05f,
                0
            );
        }
        else
        {
            Debug.LogError(
                "Enemy2: Animator is NULL!"
            );
        }

        // =========================================
        // 2. ACTIVATE KILL CAMERA
        // =========================================

        if (killCamera != null)
        {
            ActivateKillCamera();
        }
        else
        {
            Debug.LogError(
                "Enemy2: Kill Camera is NULL!"
            );
        }

        // =========================================
        // 3. FADE TO BLACK
        // =========================================

        if (fadeCanvas != null)
        {
            fadeCanvas.blocksRaycasts = true;

            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;

                fadeCanvas.alpha =
                    Mathf.Lerp(
                        0f,
                        1f,
                        timer / fadeDuration
                    );

                yield return null;
            }

            fadeCanvas.alpha = 1f;
        }

        // =========================================
        // 4. SHOW YOU DIED
        // =========================================

        yield return new WaitForSeconds(loseUIDelay);

        if (loseUI != null)
        {
            loseUI.SetActive(true);
        }
    }

    // =========================================================
    // KILL CAMERA
    // =========================================================

    void ActivateKillCamera()
    {
        if (mainCamera != null)
            mainCamera.enabled = false;

        if (killCamera != null)
            killCamera.enabled = true;

        Debug.Log("Kill camera activated.");
    }
}