
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class JumpscareController : MonoBehaviour
{
    [System.Serializable]
    public class JumpscareImage
    {
        public RawImage image;
    }

    [Header("Jumpscare UI")]
    [SerializeField] private GameObject jumpscarePanel;

    [Header("Image Sequence")]
    [SerializeField] private List<JumpscareImage> images = new List<JumpscareImage>();

    [Header("First Image - Fixed Duration")]
    [SerializeField] private float firstImageDuration = 2f;

    [Header("Other Images - Random Duration")]
    [SerializeField] private float randomMinDuration = 0.2f;
    [SerializeField] private float randomMaxDuration = 1.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource jumpscareAudio;

    private bool isPlaying;

    private void Awake()
    {
        HideAllImages();

        if (jumpscarePanel != null)
            jumpscarePanel.SetActive(false);
    }

    public void PlayJumpscare()
    {
        if (isPlaying)
            return;

        StartCoroutine(PlayJumpscareSequence());
    }

    public IEnumerator PlayJumpscareSequence()
    {
        if (isPlaying)
            yield break;

        isPlaying = true;

        if (jumpscarePanel != null)
            jumpscarePanel.SetActive(true);

        HideAllImages();

        if (jumpscareAudio != null)
            jumpscareAudio.Play();

        for (int i = 0; i < images.Count; i++)
        {
            RawImage currentImage = images[i].image;

            if (currentImage == null)
                continue;

            // Show current image
            currentImage.enabled = true;

            float duration;

            // First image uses fixed duration
            if (i == 0)
            {
                duration = firstImageDuration;
            }
            else
            {
                // Other images use random duration
                duration = Random.Range(
                    randomMinDuration,
                    randomMaxDuration
                );
            }

            yield return new WaitForSeconds(duration);

            // Hide current image before showing the next
            currentImage.enabled = false;
        }

        // End sequence
        if (jumpscarePanel != null)
            jumpscarePanel.SetActive(false);

        isPlaying = false;
    }

    private void HideAllImages()
    {
        foreach (JumpscareImage item in images)
        {
            if (item.image != null)
                item.image.enabled = false;
        }
    }
}
