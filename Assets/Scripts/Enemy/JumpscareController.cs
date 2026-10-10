using UnityEngine;

public class JumpscareController : MonoBehaviour
{
    [Header("Jumpscare UI")]
    [SerializeField] private GameObject jumpscarePanel;
    [SerializeField] private AudioSource jumpscareAudio;

    [Header("Timing")]
    [SerializeField] private float jumpscareDuration = 2f;

    private void Awake()
    {
        if (jumpscarePanel != null)
            jumpscarePanel.SetActive(false);
    }

    public void PlayJumpscare()
    {
        StartCoroutine(JumpscareRoutine());
    }

    private System.Collections.IEnumerator JumpscareRoutine()
    {
        if (jumpscarePanel != null)
            jumpscarePanel.SetActive(true);

        if (jumpscareAudio != null)
            jumpscareAudio.Play();

        yield return new WaitForSeconds(jumpscareDuration);

        if (jumpscarePanel != null)
            jumpscarePanel.SetActive(false);
    }
}