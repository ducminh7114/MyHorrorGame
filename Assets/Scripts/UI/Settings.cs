using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering;

public class Settings : MonoBehaviour
{
    // =========================================================
    // GRAPHICS
    // =========================================================

    [Header("Graphics")]
    public TMP_Dropdown graphicDrop;

    [Header("Resolution")]
    public TMP_Dropdown resoDrop;

    [Header("Audio")]
    public Slider volumeSlider;


    // =========================================================
    // POST PROCESSING
    // =========================================================

    [Header("Post Processing")]

    [Tooltip("Global Volume containing your post-processing overrides.")]
    public Volume globalVolume;

    [Tooltip("List of post-processing effects.")]
    public List<PostProcessingSetting> postProcessing =
        new List<PostProcessingSetting>();


    // =========================================================
    // OTHER
    // =========================================================

    [Header("Settings State")]

    [Tooltip("True when this settings menu is being used in-game.")]
    public bool inGame;


    [Header("Settings Page")]
    public GameObject settingsPage;
    public GameObject previousPage;

    [Header("Save Message")]
    public TMP_Text saveMessage;
    public float saveMessageDuration = 2f;

    // =========================================================
    // GRAPHICS
    // =========================================================

    public void setGraphics()
    {
        if (graphicDrop == null)
            return;

        if (graphicDrop.value == 0)
        {
            QualitySettings.SetQualityLevel(0);

            PlayerPrefs.SetInt("graphics", 0);
            PlayerPrefs.Save();

            Debug.Log("Graphics set to Low");
        }

        else if (graphicDrop.value == 1)
        {
            QualitySettings.SetQualityLevel(1);

            PlayerPrefs.SetInt("graphics", 1);
            PlayerPrefs.Save();

            Debug.Log("Graphics set to Medium");
        }

        else if (graphicDrop.value == 2)
        {
            QualitySettings.SetQualityLevel(2);

            PlayerPrefs.SetInt("graphics", 2);
            PlayerPrefs.Save();

            Debug.Log("Graphics set to High");
        }
    }


    // =========================================================
    // RESOLUTION
    // =========================================================

    public void setResolution()
    {
        if (resoDrop == null)
            return;

        if (resoDrop.value == 0)
        {
            Screen.SetResolution(
                854,
                450,
                true
            );

            PlayerPrefs.SetInt("resolution", 0);
            PlayerPrefs.Save();

            Debug.Log("Resolution set to 854x450");
        }

        else if (resoDrop.value == 1)
        {
            Screen.SetResolution(
                1200,
                730,
                true
            );

            PlayerPrefs.SetInt("resolution", 1);
            PlayerPrefs.Save();

            Debug.Log("Resolution set to 1200x730");
        }

        else if (resoDrop.value == 2)
        {
            Screen.SetResolution(
                1920,
                1080,
                true
            );

            PlayerPrefs.SetInt("resolution", 2);
            PlayerPrefs.Save();

            Debug.Log("Resolution set to 1920x1080");
        }
    }


    // =========================================================
    // VOLUME
    // =========================================================

    public void setVolume()
    {
        if (volumeSlider == null)
            return;

        AudioListener.volume =
            volumeSlider.value;

        PlayerPrefs.SetFloat(
            "masterVolume",
            volumeSlider.value
        );

        PlayerPrefs.Save();

        Debug.Log(
            "Volume set to " +
            volumeSlider.value
        );
    }


    // =========================================================
    // POST PROCESSING
    // =========================================================

    public void SetPostProcessing(
        int index,
        bool enabled)
    {
        if (index < 0 ||
            index >= postProcessing.Count)
            return;

        PostProcessingSetting setting =
            postProcessing[index];

        setting.enabled = enabled;

        ApplyPostProcessing(setting);

        SavePostProcessing(index, enabled);

        Debug.Log(
            setting.effectName +
            (enabled
                ? " Enabled"
                : " Disabled")
        );
    }


    // =========================================================
    // APPLY POST PROCESSING
    // =========================================================

    void ApplyPostProcessing(
        PostProcessingSetting setting)
    {
        if (setting.overrideComponent == null)
            return;

        setting.overrideComponent.active =
            setting.enabled;
    }


    // =========================================================
    // SAVE POST PROCESSING
    // =========================================================

    void SavePostProcessing(
        int index,
        bool enabled)
    {
        PlayerPrefs.SetInt(
            "postProcessing_" + index,
            enabled ? 1 : 0
        );

        PlayerPrefs.Save();
    }


    // =========================================================
    // LOAD POST PROCESSING
    // =========================================================

    void LoadPostProcessing()
    {
        for (int i = 0;
             i < postProcessing.Count;
             i++)
        {
            PostProcessingSetting setting =
                postProcessing[i];

            int defaultValue =
                setting.enabledByDefault ? 1 : 0;

            int savedValue =
                PlayerPrefs.GetInt(
                    "postProcessing_" + i,
                    defaultValue
                );

            setting.enabled =
                savedValue == 1;

            ApplyPostProcessing(setting);

            // Update the UI toggle
            if (setting.toggle != null)
            {
                setting.toggle.SetIsOnWithoutNotify(
                    setting.enabled
                );
            }
        }
    }


    // =========================================================
    // SAVE ALL SETTINGS
    // =========================================================

    public void saveSettings()
    {
        // Graphics
        if (graphicDrop != null)
        {
            PlayerPrefs.SetInt(
                "graphics",
                graphicDrop.value
            );
        }

        // Resolution
        if (resoDrop != null)
        {
            PlayerPrefs.SetInt(
                "resolution",
                resoDrop.value
            );
        }

        // Volume
        if (volumeSlider != null)
        {
            PlayerPrefs.SetFloat(
                "masterVolume",
                volumeSlider.value
            );

            AudioListener.volume = volumeSlider.value;
        }

        // Post Processing
        for (int i = 0; i < postProcessing.Count; i++)
        {
            PlayerPrefs.SetInt(
                "postProcessing_" + i,
                postProcessing[i].enabled ? 1 : 0
            );
        }

        // Mark settings as saved
        PlayerPrefs.SetInt("setingSaved", 1);

        // Actually save everything
        PlayerPrefs.Save();

        Debug.Log("All settings saved.");

        // Show save message
        if (saveMessage != null)
        {
            StartCoroutine(ShowSaveMessage());
        }
    }


    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        // =====================================================
        // FIRST TIME SETTINGS
        // =====================================================

        if (PlayerPrefs.GetInt(
            "setingSaved",
            0) == 0)
        {
            PlayerPrefs.SetInt(
                "graphics",
                0
            );

            PlayerPrefs.SetInt(
                "resolution",
                0
            );

            PlayerPrefs.SetFloat(
                "masterVolume",
                1.0f
            );

            // Set default post-processing values
            for (int i = 0;
                 i < postProcessing.Count;
                 i++)
            {
                PlayerPrefs.SetInt(
                    "postProcessing_" + i,
                    postProcessing[i]
                        .enabledByDefault
                        ? 1
                        : 0
                );
            }

            PlayerPrefs.Save();
        }


        // =====================================================
        // GRAPHICS
        // =====================================================

        int graphics =
            PlayerPrefs.GetInt(
                "graphics",
                0
            );

        graphicDrop.value =
            graphics;

        QualitySettings.SetQualityLevel(
            graphics
        );


        // =====================================================
        // RESOLUTION
        // =====================================================

        int resolution =
            PlayerPrefs.GetInt(
                "resolution",
                0
            );

        resoDrop.value =
            resolution;

        if (resolution == 0)
        {
            Screen.SetResolution(
                854,
                450,
                true
            );
        }

        else if (resolution == 1)
        {
            Screen.SetResolution(
                1200,
                730,
                true
            );
        }

        else if (resolution == 2)
        {
            Screen.SetResolution(
                1920,
                1080,
                true
            );
        }


        // =====================================================
        // VOLUME
        // =====================================================

        float volume =
            PlayerPrefs.GetFloat(
                "masterVolume",
                1.0f
            );

        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(
                volume
            );
        }

        AudioListener.volume =
            volume;


        // =====================================================
        // POST PROCESSING
        // =====================================================

        LoadPostProcessing();
    }


    // =========================================================
    // POST PROCESSING SETTING CLASS
    // =========================================================

    [System.Serializable]
    public class PostProcessingSetting
    {
        [Header("Effect")]

        [Tooltip("Name shown in the Inspector.")]
        public string effectName =
            "Post Processing Effect";


        [Tooltip(
            "The Volume Override component " +
            "from your Global Volume Profile."
        )]
        public VolumeComponent overrideComponent;


        [Header("UI")]

        [Tooltip(
            "Toggle that controls this effect."
        )]
        public Toggle toggle;


        [Header("Default")]

        [Tooltip(
            "Should this effect be enabled " +
            "the first time the game starts?"
        )]
        public bool enabledByDefault = false;


        [HideInInspector]
        public bool enabled;
    }

    public void ReturnToPreviousPage()
    {
        // Disable Settings page
        if (settingsPage != null)
        {
            settingsPage.SetActive(false);
        }

        // Enable previous page
        if (previousPage != null)
        {
            previousPage.SetActive(true);
        }
    }

    IEnumerator ShowSaveMessage()
    {
        saveMessage.gameObject.SetActive(true);

        saveMessage.text = "SETTINGS SAVED";

        yield return new WaitForSeconds(saveMessageDuration);

        saveMessage.gameObject.SetActive(false);
    }
}