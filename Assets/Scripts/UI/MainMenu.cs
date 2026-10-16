using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string SceneName;
    public string SceneName2;

    [Header("Menu Objects")]
    public GameObject loadingScene;
    public GameObject menuObj;
    public GameObject settingObj;
    public GameObject controlObj;

    [Header("Continue Button")]
    public Button continueButton;

    [Header("Menu Button Effects")]
    public List<MenuButtonEffect> buttonEffects =
        new List<MenuButtonEffect>();

    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UpdateContinueButton();

        foreach (MenuButtonEffect effect in buttonEffects)
        {
            if (effect != null)
            {
                effect.Initialize();
            }
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        UpdateContinueButton();
    }

    void UpdateContinueButton()
    {
        if (continueButton == null)
            return;

        continueButton.interactable =
            GameSaveManager.Instance != null &&
            GameSaveManager.Instance.HasSaveGame();
    }

    // =========================================================
    // PLAY GAME
    // =========================================================
    public void playGame()
    {
        if (GameSaveManager.Instance != null)
            GameSaveManager.Instance.NewGame();

        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
    }


    // =========================================================
    // CONTINUE GAME
    // =========================================================

    public void ContinueGame()
    {
        if (GameSaveManager.Instance == null)
        {
            Debug.LogError(
                "GameSaveManager not found."
            );

            return;
        }

        if (!GameSaveManager.Instance.HasSaveGame())
        {
            Debug.Log(
                "NO SAVE GAME."
            );

            return;
        }

        string scene =
            GameSaveManager.Instance.GetSavedScene();

        Debug.Log(
            "CONTINUE → " +
            scene
        );

        UnityEngine.SceneManagement.SceneManager
            .LoadScene(scene);
    }

    // =========================================================
    // CONTROLS
    // =========================================================

    public void controlMenu()
    {
        if (menuObj != null)
            menuObj.SetActive(false);

        if (controlObj != null)
            controlObj.SetActive(true);

        Debug.Log("Open Control menu");
    }

    // =========================================================
    // QUIT
    // =========================================================

    public void quitGame()
    {
        Debug.Log(
            "This will quit the game in the actual build, not in Unity Editor"
        );

        Application.Quit();
    }

    // =========================================================
    // BACK TO MENU
    // =========================================================

    public void backToMenu()
    {
        if (menuObj != null)
            menuObj.SetActive(true);

        if (controlObj != null)
            controlObj.SetActive(false);

        if (settingObj != null)
            settingObj.SetActive(false);

        Debug.Log("Back to Main menu");
    }
}


// =============================================================
// MENU BUTTON EFFECT
// =============================================================

[System.Serializable]
public class MenuButtonEffect
{
    [Header("Button")]
    public Button button;


    // =========================================================
    // TEXT COLOR
    // =========================================================

    [Header("Text Color")]

    [Tooltip("TextMeshPro text that changes color.")]
    public TMP_Text targetText;

    public bool changeTextColor = true;

    public Color normalTextColor =
        new Color(0.82f, 0.82f, 0.82f, 1f);

    public Color hoverTextColor =
        new Color(1f, 0.28f, 0.49f, 1f);


    // =========================================================
    // MATERIAL
    // =========================================================

    [Header("Material")]

    public bool changeMaterial = false;

    [Tooltip("Renderer whose material will change.")]
    public Renderer targetRenderer;

    [Tooltip("Material when the mouse is not hovering.")]
    public Material normalMaterial;

    [Tooltip("Optional material shown during the transition.")]
    public Material transitionMaterial;

    [Tooltip("Material when the mouse is hovering.")]
    public Material hoverMaterial;

    [Tooltip("Use the transition material.")]
    public bool useTransitionMaterial = false;

    [Tooltip("How long the transition material is displayed.")]
    public float materialTransitionDuration = 0.15f;


    // =========================================================
    // LIGHT COLOR
    // =========================================================

    [Header("Light Color")]

    public bool changeLightColor = false;

    [Tooltip("Light that changes when hovering.")]
    public Light targetLight;

    [Tooltip("Light color when not hovering.")]
    public Color normalLightColor = Color.white;

    [Tooltip("Light color while hovering.")]
    public Color hoverLightColor =
        new Color(1f, 0.28f, 0.49f, 1f);

    [Tooltip("Smoothly transition the light color.")]
    public bool smoothLightTransition = true;

    [Tooltip("Speed of the light color transition.")]
    public float lightTransitionSpeed = 8f;


    // =========================================================
    // HOVER GAMEOBJECT
    // =========================================================

    [Header("Hover GameObject")]

    public bool changeHoverObject = false;

    public GameObject hoverObject;


    // =========================================================
    // CLICK TEXT COLOR
    // =========================================================

    [Header("Click Text Color")]

    public bool changeClickTextColor = false;

    public Color clickTextColor = Color.white;


    // =========================================================
    // CLICK MATERIAL
    // =========================================================

    [Header("Click Material")]

    public bool changeClickMaterial = false;

    public Material clickMaterial;


    // =========================================================
    // CLICK GAMEOBJECT
    // =========================================================

    [Header("Click GameObject")]

    public bool changeClickObject = false;

    public GameObject clickObject;


    // =========================================================
    // INTERNAL VARIABLES
    // =========================================================

    private Coroutine materialCoroutine;
    private Coroutine lightCoroutine;

    private bool isHovering = false;


    // =========================================================
    // INITIALIZE
    // =========================================================

    public void Initialize()
    {
        if (button == null)
            return;

        // Automatically find TMP text
        if (targetText == null)
        {
            targetText =
                button.GetComponentInChildren<TMP_Text>();
        }

        // Automatically find Renderer
        if (targetRenderer == null)
        {
            targetRenderer =
                button.GetComponent<Renderer>();
        }

        // -----------------------------------------
        // TEXT
        // -----------------------------------------

        if (targetText != null)
        {
            targetText.color = normalTextColor;
        }

        // -----------------------------------------
        // MATERIAL
        // -----------------------------------------

        if (changeMaterial &&
            targetRenderer != null &&
            normalMaterial != null)
        {
            targetRenderer.material = normalMaterial;
        }

        // -----------------------------------------
        // LIGHT
        // -----------------------------------------

        if (changeLightColor &&
            targetLight != null)
        {
            targetLight.color = normalLightColor;
        }

        // -----------------------------------------
        // HOVER OBJECT
        // -----------------------------------------

        if (changeHoverObject &&
            hoverObject != null)
        {
            hoverObject.SetActive(false);
        }

        // -----------------------------------------
        // CLICK
        // -----------------------------------------

        button.onClick.AddListener(OnClick);

        // -----------------------------------------
        // HOVER EVENTS
        // -----------------------------------------

        SetupEventTrigger();
    }


    // =========================================================
    // EVENT TRIGGER
    // =========================================================

    void SetupEventTrigger()
    {
        EventTrigger trigger =
            button.GetComponent<EventTrigger>();

        if (trigger == null)
        {
            trigger =
                button.gameObject.AddComponent<EventTrigger>();
        }

        // -----------------------------------------
        // MOUSE ENTER
        // -----------------------------------------

        EventTrigger.Entry enterEntry =
            new EventTrigger.Entry();

        enterEntry.eventID =
            EventTriggerType.PointerEnter;

        enterEntry.callback.AddListener(
            (data) => OnHoverEnter()
        );

        trigger.triggers.Add(enterEntry);


        // -----------------------------------------
        // MOUSE EXIT
        // -----------------------------------------

        EventTrigger.Entry exitEntry =
            new EventTrigger.Entry();

        exitEntry.eventID =
            EventTriggerType.PointerExit;

        exitEntry.callback.AddListener(
            (data) => OnHoverExit()
        );

        trigger.triggers.Add(exitEntry);
    }


    // =========================================================
    // HOVER ENTER
    // =========================================================

    void OnHoverEnter()
    {
        isHovering = true;

        // -----------------------------------------
        // TEXT COLOR
        // -----------------------------------------

        if (changeTextColor &&
            targetText != null)
        {
            targetText.color =
                hoverTextColor;
        }


        // -----------------------------------------
        // MATERIAL
        // -----------------------------------------

        if (changeMaterial &&
            targetRenderer != null)
        {
            StartMaterialTransition(
                hoverMaterial
            );
        }


        // -----------------------------------------
        // LIGHT
        // -----------------------------------------

        if (changeLightColor &&
            targetLight != null)
        {
            StartLightTransition(
                hoverLightColor
            );
        }


        // -----------------------------------------
        // GAMEOBJECT
        // -----------------------------------------

        if (changeHoverObject &&
            hoverObject != null)
        {
            hoverObject.SetActive(true);
        }
    }


    // =========================================================
    // HOVER EXIT
    // =========================================================

    void OnHoverExit()
    {
        isHovering = false;

        // -----------------------------------------
        // TEXT COLOR
        // -----------------------------------------

        if (changeTextColor &&
            targetText != null)
        {
            targetText.color =
                normalTextColor;
        }


        // -----------------------------------------
        // MATERIAL
        // -----------------------------------------

        if (changeMaterial &&
            targetRenderer != null)
        {
            StartMaterialTransition(
                normalMaterial
            );
        }


        // -----------------------------------------
        // LIGHT
        // -----------------------------------------

        if (changeLightColor &&
            targetLight != null)
        {
            StartLightTransition(
                normalLightColor
            );
        }


        // -----------------------------------------
        // GAMEOBJECT
        // -----------------------------------------

        if (changeHoverObject &&
            hoverObject != null)
        {
            hoverObject.SetActive(false);
        }
    }


    // =========================================================
    // MATERIAL TRANSITION
    // =========================================================

    void StartMaterialTransition(
        Material finalMaterial)
    {
        if (finalMaterial == null ||
            targetRenderer == null)
            return;

        // Stop previous transition
        if (materialCoroutine != null)
        {
            MainMenuCoroutineRunner.Instance.StopCoroutine(
                materialCoroutine
            );
        }

        materialCoroutine =
            MainMenuCoroutineRunner.Instance.StartCoroutine(
                MaterialTransition(finalMaterial)
            );
    }


    IEnumerator MaterialTransition(
        Material finalMaterial)
    {
        // -----------------------------------------
        // TRANSITION MATERIAL
        // -----------------------------------------

        if (useTransitionMaterial &&
            transitionMaterial != null)
        {
            targetRenderer.material =
                transitionMaterial;

            yield return new WaitForSeconds(
                materialTransitionDuration
            );
        }

        // -----------------------------------------
        // FINAL MATERIAL
        // -----------------------------------------

        targetRenderer.material =
            finalMaterial;

        materialCoroutine = null;
    }


    // =========================================================
    // LIGHT TRANSITION
    // =========================================================

    void StartLightTransition(
        Color targetColor)
    {
        if (targetLight == null)
            return;

        // Stop previous transition
        if (lightCoroutine != null)
        {
            MainMenuCoroutineRunner.Instance.StopCoroutine(
                lightCoroutine
            );
        }

        lightCoroutine =
            MainMenuCoroutineRunner.Instance.StartCoroutine(
                LightColorTransition(targetColor)
            );
    }


    IEnumerator LightColorTransition(
        Color targetColor)
    {
        Color startingColor =
            targetLight.color;

        if (!smoothLightTransition)
        {
            targetLight.color =
                targetColor;

            lightCoroutine = null;
            yield break;
        }

        float time = 0f;

        while (time < 1f)
        {
            time +=
                Time.unscaledDeltaTime *
                lightTransitionSpeed;

            targetLight.color =
                Color.Lerp(
                    startingColor,
                    targetColor,
                    time
                );

            yield return null;
        }

        targetLight.color =
            targetColor;

        lightCoroutine = null;
    }


    // =========================================================
    // CLICK
    // =========================================================

    void OnClick()
    {
        // -----------------------------------------
        // TEXT COLOR
        // -----------------------------------------

        if (changeClickTextColor &&
            targetText != null)
        {
            targetText.color =
                clickTextColor;
        }


        // -----------------------------------------
        // MATERIAL
        // -----------------------------------------

        if (changeClickMaterial &&
            targetRenderer != null &&
            clickMaterial != null)
        {
            targetRenderer.material =
                clickMaterial;
        }


        // -----------------------------------------
        // GAMEOBJECT
        // -----------------------------------------

        if (changeClickObject &&
            clickObject != null)
        {
            clickObject.SetActive(
                !clickObject.activeSelf
            );
        }
    }
}


// =============================================================
// COROUTINE RUNNER
// =============================================================
// Allows the serializable MenuButtonEffect class to run
// coroutines even though it is not a MonoBehaviour.

public class MainMenuCoroutineRunner : MonoBehaviour
{
    private static MainMenuCoroutineRunner instance;

    public static MainMenuCoroutineRunner Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj =
                    new GameObject(
                        "MainMenu Coroutine Runner"
                    );

                instance =
                    obj.AddComponent<
                        MainMenuCoroutineRunner>();

                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }
}