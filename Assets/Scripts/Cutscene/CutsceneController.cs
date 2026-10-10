using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    public PlayableDirector director;
    public MonoBehaviour playerMovement;

    private bool isPlaying;

    public void PlayCutscene()
    {
        if (isPlaying)
            return;

        isPlaying = true;
        playerMovement.enabled = false;

        director.Play();
    }

    void Update()
    {
        if (isPlaying && director.state != PlayState.Playing)
        {
            isPlaying = false;
            playerMovement.enabled = true;
        }
    }
}
