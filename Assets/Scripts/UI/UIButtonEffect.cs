
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIButtonEffect : MonoBehaviour
{
    [Header("Button Effects List")]
    public List<ButtonEffect> buttonEffects = new List<ButtonEffect>();

    void Start()
    {
        foreach (ButtonEffect effect in buttonEffects)
        {
            if (effect != null)
                effect.Initialize();
        }
    }
}

[System.Serializable]
public class ButtonEffect
{
    [Header("Button")]
    public Button button;

    [Header("Text Color")]
    public TMP_Text targetText;
    public bool changeTextColor = true;

    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.magenta;
    public Color clickTextColor = Color.white;

    [Header("Button Fill Color")]
    public bool changeFillColor = true;
    public Image fillImage;

    public Color normalFillColor = Color.white;
    public Color hoverFillColor = Color.gray;
    public Color clickFillColor = Color.black;

    [Header("Effect Options")]
    public bool enableHoverEffect = true;
    public bool enableClickEffect = true;

    private bool isHovering;
    private bool isPressed;

    public void Initialize()
    {
        if (button == null)
            return;

        if (targetText == null)
            targetText = button.GetComponentInChildren<TMP_Text>();

        if (fillImage == null)
            fillImage = button.GetComponent<Image>();

        button.transition = Selectable.Transition.None;

        ApplyNormal();

        EventTrigger trigger = button.GetComponent<EventTrigger>();

        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        AddEvent(trigger, EventTriggerType.PointerEnter, OnHoverEnter);
        AddEvent(trigger, EventTriggerType.PointerExit, OnHoverExit);
        AddEvent(trigger, EventTriggerType.PointerDown, OnClickDown);
        AddEvent(trigger, EventTriggerType.PointerUp, OnClickUp);
    }

    void AddEvent(EventTrigger trigger,
        EventTriggerType eventType, System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = eventType;
        entry.callback.AddListener((data) => action());

        trigger.triggers.Add(entry);
    }

    void OnHoverEnter()
    {
        isHovering = true;
        isPressed = false;

        if (enableHoverEffect)
            ApplyHover();
    }

    void OnHoverExit()
    {
        isHovering = false;
        isPressed = false;

        ApplyNormal();
    }

    void OnClickDown()
    {
        if (!enableClickEffect)
            return;

        isPressed = true;
        ApplyClick();
    }

    void OnClickUp()
    {
        isPressed = false;

        if (isHovering && enableHoverEffect)
            ApplyHover();
        else
            ApplyNormal();
    }

    void ApplyNormal()
    {
        if (changeTextColor && targetText != null)
            targetText.color = normalTextColor;

        if (changeFillColor && fillImage != null)
            fillImage.color = normalFillColor;
    }

    void ApplyHover()
    {
        if (changeTextColor && targetText != null)
            targetText.color = hoverTextColor;

        if (changeFillColor && fillImage != null)
            fillImage.color = hoverFillColor;
    }

    void ApplyClick()
    {
        if (changeTextColor && targetText != null)
            targetText.color = clickTextColor;

        if (changeFillColor && fillImage != null)
            fillImage.color = clickFillColor;
    }
}
