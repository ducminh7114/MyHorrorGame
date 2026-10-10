using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using TMPro;

public class eventTrigger : MonoBehaviour
{
    // =========================================================
    // OBJECT ACTION
    // =========================================================

    [System.Serializable]
    public class ObjectAction
    {
        public GameObject targetObject;

        public bool useDelay;
        public float delay;
    }

    // =========================================================
    // ANIMATION ACTION
    // =========================================================

    [System.Serializable]
    public class AnimationAction
    {
        public Animator animator;
        public string triggerName;

        public bool useDelay;
        public float delay;
    }

    // =========================================================
    // MATERIAL ACTION
    // =========================================================

    [System.Serializable]
    public class MaterialAction
    {
        public Renderer targetRenderer;
        public Material newMaterial;

        public bool useDelay;
        public float delay;
    }

    // =========================================================
    // SCRIPT ACTION
    // =========================================================

    [System.Serializable]
    public class ScriptAction
    {
        public MonoBehaviour targetScript;

        public bool useDelay;
        public float delay;
    }

    // =========================================================
    // AUDIO ACTION
    // =========================================================

    [System.Serializable]
    public class AudioAction
    {
        public AudioSource audioSource;

        public bool useDelay;
        public float delay;
    }

    // =========================================================
    // CAMERA FOCUS
    // =========================================================

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

    // =========================================================
    // METHOD ACTION
    // =========================================================

    [System.Serializable]
    public class MethodAction
    {
        public MonoBehaviour targetScript;
        public string methodName;

        public bool useDelay;
        public float delay;
    }

    // =========================================================
    // LIGHT ACTION
    // =========================================================

    [System.Serializable]
    public class LightAction
    {
        public Light targetLight;
        public Color newColor = Color.red;

        public bool useDelay;
        public float delay;
    }

    // =========================================================
    // TIMELINE ACTION
    // =========================================================

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

        [Header("Save")]
        public bool savePlayerPosition = true;
        public bool savePlayerRotation = true;

        [Header("Save Message")]
        public TMP_Text saveMessage;
        public string message = "GAME SAVED";
        public float messageDuration = 2f;

        [Header("Delay")]
        public bool useDelay;
        public float delay;
    }

    // =========================================================
    // PERSISTENT TRIGGER ID
    // =========================================================

    [Header("Persistent Save ID")]
    [Tooltip("Must be UNIQUE for every event trigger in the game.")]
    public string triggerID = "Trigger_01";

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
        // No GameSaveManager = nothing to restore
        if (GameSaveManager.Instance == null)
            return;

        // Check if this trigger was completed before
        if (!string.IsNullOrEmpty(triggerID) &&
            GameSaveManager.Instance.IsTriggerCompleted(triggerID))
        {
            hasTriggered = true;

            Debug.Log(
                "Trigger already completed: " +
                triggerID
            );
        }
    }

    // =========================================================
    // TRIGGER
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Prevent trigger from running again
        if (triggerOnce && hasTriggered)
            return;

        hasTriggered = true;

        // =====================================================
        // REGISTER TRIGGER AS COMPLETED
        // =====================================================

        if (GameSaveManager.Instance != null &&
            !string.IsNullOrEmpty(triggerID))
        {
            GameSaveManager.Instance.MarkTriggerCompleted(
                triggerID
            );

            Debug.Log(
                "EVENT TRIGGER COMPLETED AND REGISTERED: " +
                triggerID
            );
        }

        // =====================================================
        // ENABLE OBJECTS
        // =====================================================

        foreach (var action in enableObjects)
        {
            StartCoroutine(
                EnableObjectRoutine(action)
            );
        }

        // =====================================================
        // DISABLE OBJECTS
        // =====================================================

        foreach (var action in disableObjects)
        {
            StartCoroutine(
                DisableObjectRoutine(action)
            );
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
        // TIMELINE
        // =====================================================

        foreach (var action in timelineActions)
        {
            StartCoroutine(
                TimelineRoutine(action)
            );
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
            action.targetObject.SetActive(true);
    }

    // =========================================================
    // DISABLE OBJECT
    // =========================================================

    IEnumerator DisableObjectRoutine(ObjectAction action)
    {
        if (action.useDelay)
            yield return new WaitForSeconds(action.delay);

        if (action.targetObject != null)
            action.targetObject.SetActive(false);
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
        if (action.useDelay)
            yield return new WaitForSeconds(action.delay);

        if (action.audioSource != null)
            action.audioSource.Play();
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

        // =====================================================
        // ENABLE CUTSCENE OBJECTS
        // =====================================================

        if (action.cutsceneRoot != null)
            action.cutsceneRoot.SetActive(true);

        // =====================================================
        // DISABLE PLAYER MOVEMENT
        // =====================================================

        if (action.disablePlayerMovement &&
            action.playerMovement != null)
        {
            action.playerMovement.enabled = false;
        }

        // =====================================================
        // HIDE PLAYER
        // =====================================================

        if (action.hidePlayer &&
            action.player != null)
        {
            action.player.SetActive(false);
        }

        // =====================================================
        // WAIT FOR TIMELINE
        // =====================================================

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
}