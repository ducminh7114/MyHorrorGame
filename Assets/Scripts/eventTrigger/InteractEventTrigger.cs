using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using TMPro;

public class InteractEventTrigger : MonoBehaviour
{
    // =========================================================
    // ACTION CLASSES
    // =========================================================

    [System.Serializable]
    public class ObjectAction
    {
        public GameObject targetObject;

        public bool useDelay;
        public float delay;
    }

    [System.Serializable]
    public class AnimationAction
    {
        public Animator animator;
        public string triggerName;

        public bool useDelay;
        public float delay;
    }

    [System.Serializable]
    public class MaterialAction
    {
        public Renderer targetRenderer;
        public Material newMaterial;

        public bool useDelay;
        public float delay;
    }

    [System.Serializable]
    public class ScriptAction
    {
        public MonoBehaviour targetScript;

        public bool useDelay;
        public float delay;
    }

    [System.Serializable]
    public class AudioAction
    {
        public AudioSource audioSource;
        public AudioClip audioClip;

        [Range(0f, 1f)]
        public float volume = 1f;

        public bool useDelay;
        public float delay;
    }

    [System.Serializable]
    public class CameraFocusAction
    {
        public CameraFocus cameraFocus;
        public Transform target;

        public float focusTime = 2f;

        [Range(10, 90)]
        public float zoomFOV = 40f;

        public float zoomOutTime = 1f;

        public bool useDelay;
        public float delay;
    }

    [System.Serializable]
    public class MethodAction
    {
        public MonoBehaviour targetScript;

        public string methodName = "StartCutscene";

        public bool useDelay;
        public float delay;
    }

    [System.Serializable]
    public class LightAction
    {
        public Light targetLight;
        public Color newColor = Color.red;

        public bool useDelay;
        public float delay;
    }

    [System.Serializable]
    public class TimelineAction
    {
        public PlayableDirector director;

        [Header("Player")]
        public bool disablePlayerMovement = true;
        public MonoBehaviour playerMovement;

        public bool hidePlayer = false;
        public GameObject player;

        [Header("Teleport After Timeline")]
        public bool teleportPlayer = false;
        public Transform endPosition;

        [Header("Cutscene Root")]
        public GameObject cutsceneRoot;

        [Header("Delay")]
        public bool useDelay;
        public float delay;
    }

    // =========================================================
    // SAVE GAME ACTION
    // =========================================================

    [System.Serializable]
    public class SaveGameAction
    {
        [Header("Player")]
        public GameObject player;

        [Header("Save Message")]
        public TMP_Text saveMessage;

        public string message = "GAME SAVED";

        public float messageDuration = 2f;

        [Header("Delay")]
        public bool useDelay;
        public float delay;
    }

    // =========================================================
    // INTERACTION
    // =========================================================

    [Header("Interaction")]
    public GameObject inttext;
    public bool interactable;

    [Header("Note Interaction")]
    [Tooltip("Enable this when readNote handles the E key.")]
    public bool noteInteractionMode = false;

    // =========================================================
    // SAVE ID
    // =========================================================

    [Header("Persistent Save ID")]
    [Tooltip("Give every InteractEventTrigger a UNIQUE ID.")]
    public string triggerID = "InteractTrigger_01";

    // =========================================================
    // ACTION LISTS
    // =========================================================

    [Header("Enable Objects")]
    public List<ObjectAction> enableObjects = new();

    [Header("Disable Objects")]
    public List<ObjectAction> disableObjects = new();

    [Header("Animations")]
    public List<AnimationAction> animations = new();

    [Header("Materials")]
    public List<MaterialAction> materials = new();

    [Header("Enable Scripts")]
    public List<ScriptAction> enableScripts = new();

    [Header("Disable Scripts")]
    public List<ScriptAction> disableScripts = new();

    [Header("Audio")]
    public List<AudioAction> audioActions = new();

    [Header("Camera Focus")]
    public List<CameraFocusAction> cameraFocusActions = new();

    [Header("Methods")]
    public List<MethodAction> methods = new();

    [Header("Settings")]
    public bool triggerOnce = true;

    [Header("Lights")]
    public List<LightAction> lights = new();

    [Header("Timeline / Cutscene")]
    public List<TimelineAction> timelineActions = new();

    [Header("Save Game")]
    public List<SaveGameAction> saveGameActions = new();

    // =========================================================
    // INTERNAL
    // =========================================================

    private bool hasTriggered;

    public bool IsTriggered()
    {
        return hasTriggered;
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        interactable = false;

        if (inttext != null)
            inttext.SetActive(false);

        /*
         * Check the NEW save system.
         *
         * We do NOT use PlayerPrefs anymore.
         */

        if (GameSaveManager.Instance != null &&
            !string.IsNullOrEmpty(triggerID) &&
            GameSaveManager.Instance.IsTriggerCompleted(triggerID))
        {
            hasTriggered = true;

            Debug.Log(
                "Interaction already completed: " +
                triggerID
            );

            /*
             * Restore persistent gameplay effects.
             *
             * We restore things such as:
             * - objects
             * - materials
             * - scripts
             * - lights
             *
             * We intentionally DO NOT replay:
             * - audio
             * - camera focus
             * - timeline
             * - methods
             *
             * Those are one-time effects.
             */
            RestorePersistentState();

            return;
        }

        hasTriggered = false;

        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = true;
    }

    // =========================================================
    // INTERACTION DETECTION
    // =========================================================

    private void OnTriggerStay(Collider other)
    {
        if (hasTriggered)
            return;

        if (other.CompareTag("MainCamera"))
        {
            interactable = true;

            if (inttext != null)
                inttext.SetActive(true);
        }
    }

    // =========================================================
    // EXIT
    // =========================================================

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            interactable = false;

            if (inttext != null)
                inttext.SetActive(false);
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        // readNote handles the E key for this object.
        if (noteInteractionMode)
            return;

        if (!interactable)
            return;

        if (hasTriggered)
            return;

        if (!Input.GetKeyDown(KeyCode.E))
            return;

        // =====================================================
        // MARK AS COMPLETED
        // =====================================================

        hasTriggered = true;

        /*
         * IMPORTANT:
         *
         * Immediately tell GameSaveManager that this
         * interaction has been completed.
         *
         * The checkpoint will later save this information
         * to the JSON save file.
         */

        if (GameSaveManager.Instance != null &&
            !string.IsNullOrEmpty(triggerID))
        {
            GameSaveManager.Instance.MarkTriggerCompleted(
                triggerID
            );

            Debug.Log(
                "INTERACTION COMPLETED: " +
                triggerID
            );
        }

        // =====================================================
        // HIDE INTERACTION TEXT
        // =====================================================

        if (inttext != null)
            inttext.SetActive(false);

        foreach (var action in enableObjects)
        {
            if (action.targetObject != null)
                SaveObjectState.SetActive(action.targetObject, true);
        }

        foreach (var action in disableObjects)
        {
            if (action.targetObject != null)
                SaveObjectState.SetActive(action.targetObject, false);
        }

        // =====================================================
        // ANIMATIONS
        // =====================================================

        foreach (var action in animations)
        {
            StartCoroutine(
                AnimationRoutine(action)
            );
        }

        // =====================================================
        // MATERIALS
        // =====================================================

        foreach (var action in materials)
        {
            StartCoroutine(
                MaterialRoutine(action)
            );
        }

        // =====================================================
        // ENABLE SCRIPTS
        // =====================================================

        foreach (var action in enableScripts)
        {
            StartCoroutine(
                EnableScriptRoutine(action)
            );
        }

        // =====================================================
        // DISABLE SCRIPTS
        // =====================================================

        foreach (var action in disableScripts)
        {
            StartCoroutine(
                DisableScriptRoutine(action)
            );
        }

        // =====================================================
        // AUDIO
        // =====================================================

        foreach (var action in audioActions)
        {
            StartCoroutine(
                AudioRoutine(action)
            );
        }

        // =====================================================
        // CAMERA FOCUS
        // =====================================================

        foreach (var action in cameraFocusActions)
        {
            StartCoroutine(
                CameraFocusRoutine(action)
            );
        }

        // =====================================================
        // METHODS
        // =====================================================

        foreach (var action in methods)
        {
            StartCoroutine(
                MethodRoutine(action)
            );
        }

        // =====================================================
        // LIGHTS
        // =====================================================

        foreach (var action in lights)
        {
            StartCoroutine(
                LightRoutine(action)
            );
        }

        // =====================================================
        // TIMELINES
        // =====================================================

        foreach (var action in timelineActions)
        {
            StartCoroutine(
                TimelineRoutine(action)
            );
        }

        // =====================================================
        // SAVE GAME
        // =====================================================

        foreach (var action in saveGameActions)
        {
            StartCoroutine(
                SaveGameRoutine(action)
            );
        }

        // =====================================================
        // DISABLE INTERACTION
        // =====================================================

        if (triggerOnce)
        {
            interactable = false;

            Collider col = GetComponent<Collider>();

            if (col != null)
                col.enabled = false;
        }
    }

    // =========================================================
    // ENABLE OBJECT
    // =========================================================

    IEnumerator EnableObjectRoutine(ObjectAction action)
    {
        if (action.useDelay)
            yield return new WaitForSeconds(action.delay);

        if (action.targetObject != null)
            SaveObjectState.SetActive(action.targetObject, true);
    }

    // =========================================================
    // DISABLE OBJECT
    // =========================================================

    IEnumerator DisableObjectRoutine(ObjectAction action)
    {
        if (action.useDelay)
            yield return new WaitForSeconds(action.delay);

        if (action.targetObject != null)
            SaveObjectState.SetActive(action.targetObject, false);
    }

    // =========================================================
    // ANIMATION
    // =========================================================

    IEnumerator AnimationRoutine(AnimationAction action)
    {
        if (action.useDelay)
            yield return new WaitForSeconds(action.delay);

        if (action.animator != null)
        {
            Debug.Log(
                "Triggering animation: " +
                action.triggerName
            );

            action.animator.SetTrigger(
                action.triggerName
            );
        }
    }

    // =========================================================
    // MATERIAL
    // =========================================================

    IEnumerator MaterialRoutine(MaterialAction action)
    {
        if (action.useDelay)
            yield return new WaitForSeconds(action.delay);

        if (action.targetRenderer != null &&
            action.newMaterial != null)
        {
            action.targetRenderer.material =
                action.newMaterial;
        }
    }

    // =========================================================
    // ENABLE SCRIPT
    // =========================================================

    IEnumerator EnableScriptRoutine(ScriptAction action)
    {
        if (action.useDelay)
            yield return new WaitForSeconds(action.delay);

        if (action.targetScript != null)
            action.targetScript.enabled = true;
    }

    // =========================================================
    // DISABLE SCRIPT
    // =========================================================

    IEnumerator DisableScriptRoutine(ScriptAction action)
    {
        if (action.useDelay)
            yield return new WaitForSeconds(action.delay);

        if (action.targetScript != null)
            action.targetScript.enabled = false;
    }

    // =========================================================
    // AUDIO
    // =========================================================

    IEnumerator AudioRoutine(AudioAction action)
    {
        if (action.useDelay && action.delay > 0f)
            yield return new WaitForSeconds(action.delay);

        if (action.audioSource != null)
        {
            if (action.audioClip != null)
            {
                action.audioSource.PlayOneShot(
                    action.audioClip,
                    action.volume
                );
            }
            else
            {
                action.audioSource.Play();
            }
        }
    }

    // =========================================================
    // CAMERA FOCUS
    // =========================================================

    IEnumerator CameraFocusRoutine(
        CameraFocusAction action)
    {
        if (action.useDelay)
            yield return new WaitForSeconds(action.delay);

        if (action.cameraFocus != null &&
            action.target != null)
        {
            action.cameraFocus.FocusOnTarget(
                action.target,
                action.focusTime,
                action.zoomFOV,
                action.zoomOutTime
            );
        }
    }

    // =========================================================
    // METHOD
    // =========================================================

    IEnumerator MethodRoutine(MethodAction action)
    {
        if (action.useDelay)
            yield return new WaitForSeconds(action.delay);

        if (action.targetScript != null)
        {
            action.targetScript.SendMessage(
                action.methodName,
                SendMessageOptions.DontRequireReceiver
            );
        }
    }

    // =========================================================
    // LIGHT
    // =========================================================

    IEnumerator LightRoutine(LightAction action)
    {
        if (action.useDelay)
            yield return new WaitForSeconds(action.delay);

        if (action.targetLight != null)
        {
            action.targetLight.color =
                action.newColor;
        }
    }

    // =========================================================
    // TIMELINE
    // =========================================================

    IEnumerator TimelineRoutine(
        TimelineAction action)
    {
        if (action.useDelay)
            yield return new WaitForSeconds(action.delay);

        if (action.director == null)
            yield break;

        // Enable cutscene objects
        if (action.cutsceneRoot != null)
            action.cutsceneRoot.SetActive(true);

        // Disable player movement
        if (action.disablePlayerMovement &&
            action.playerMovement != null)
        {
            action.playerMovement.enabled = false;
        }

        // Hide player
        if (action.hidePlayer &&
            action.player != null)
        {
            action.player.SetActive(false);
        }

        bool timelineFinished = false;

        void OnTimelineFinished(
            PlayableDirector finishedDirector)
        {
            timelineFinished = true;

            finishedDirector.stopped -=
                OnTimelineFinished;
        }

        action.director.stopped +=
            OnTimelineFinished;

        action.director.Play();

        yield return new WaitUntil(
            () => timelineFinished
        );

        // =====================================================
        // TELEPORT PLAYER
        // =====================================================

        if (action.teleportPlayer &&
            action.player != null &&
            action.endPosition != null)
        {
            CharacterController cc =
                action.player.GetComponent<CharacterController>();

            if (cc != null)
                cc.enabled = false;

            action.player.transform.SetPositionAndRotation(
                action.endPosition.position,
                action.endPosition.rotation
            );

            yield return null;

            if (cc != null)
                cc.enabled = true;
        }

        // =====================================================
        // SHOW PLAYER
        // =====================================================

        if (action.hidePlayer &&
            action.player != null)
        {
            action.player.SetActive(true);
        }

        // =====================================================
        // DISABLE CUTSCENE ROOT
        // =====================================================

        if (action.cutsceneRoot != null)
            action.cutsceneRoot.SetActive(false);

        // =====================================================
        // ENABLE PLAYER MOVEMENT
        // =====================================================

        if (action.disablePlayerMovement &&
            action.playerMovement != null)
        {
            action.playerMovement.enabled = true;
        }
    }

    // =========================================================
    // SAVE GAME
    // =========================================================

    IEnumerator SaveGameRoutine(
        SaveGameAction action)
    {
        if (action.useDelay)
            yield return new WaitForSeconds(
                action.delay
            );

        if (GameSaveManager.Instance == null)
        {
            Debug.LogError(
                "Save Game failed: GameSaveManager not found!"
            );

            yield break;
        }

        if (action.player == null)
        {
            Debug.LogWarning(
                "Save Game: Player is not assigned."
            );

            yield break;
        }

        /*
         * The new save system handles:
         *
         * - scene
         * - player position
         * - player rotation
         * - completed triggers
         * - persistent object states
         */

        GameSaveManager.Instance.SaveCheckpoint(
            action.player
        );

        Debug.Log(
            "GAME SAVED: " +
            UnityEngine.SceneManagement.SceneManager
                .GetActiveScene()
                .name
        );

        // =====================================================
        // SAVE MESSAGE
        // =====================================================

        if (action.saveMessage != null)
        {
            action.saveMessage.gameObject.SetActive(true);

            action.saveMessage.text =
                action.message;

            yield return new WaitForSeconds(
                action.messageDuration
            );

            action.saveMessage.gameObject.SetActive(false);
        }
    }

    // =========================================================
    // RESTORE PERSISTENT STATE
    // =========================================================

    private void RestorePersistentState()
    {
        /*
         * IMPORTANT:
         *
         * We restore the RESULT of the interaction,
         * not the interaction itself.
         *
         * Therefore:
         *
         * DO restore:
         * - objects
         * - materials
         * - scripts
         * - lights
         *
         * DO NOT replay:
         * - audio
         * - camera
         * - timeline
         * - methods
         * - animations
         */

        // =====================================================
        // OBJECTS
        // =====================================================

        foreach (var action in enableObjects)
        {
            if (action.targetObject != null)
                action.targetObject.SetActive(true);
        }

        foreach (var action in disableObjects)
        {
            if (action.targetObject != null)
                action.targetObject.SetActive(false);
        }

        // =====================================================
        // MATERIALS
        // =====================================================

        foreach (var action in materials)
        {
            if (action.targetRenderer != null &&
                action.newMaterial != null)
            {
                action.targetRenderer.material =
                    action.newMaterial;
            }
        }

        // =====================================================
        // SCRIPTS
        // =====================================================

        foreach (var action in enableScripts)
        {
            if (action.targetScript != null)
                action.targetScript.enabled = true;
        }

        foreach (var action in disableScripts)
        {
            if (action.targetScript != null)
                action.targetScript.enabled = false;
        }

        // =====================================================
        // LIGHTS
        // =====================================================

        foreach (var action in lights)
        {
            if (action.targetLight != null)
            {
                action.targetLight.color =
                    action.newColor;
            }
        }

        Debug.Log(
            "Persistent state restored for: " +
            triggerID
        );
    }

    // =========================================================
    // NOTE INTERACTION COMPLETION
    // =========================================================

    public void CompleteNoteInteraction()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;
        interactable = false;

        if (inttext != null)
            inttext.SetActive(false);

        // Register completion with the save system.
        if (GameSaveManager.Instance != null &&
            !string.IsNullOrEmpty(triggerID))
        {
            GameSaveManager.Instance.MarkTriggerCompleted(
                triggerID
            );

            Debug.Log("NOTE INTERACTION COMPLETED: " + triggerID);
        }
    }
}