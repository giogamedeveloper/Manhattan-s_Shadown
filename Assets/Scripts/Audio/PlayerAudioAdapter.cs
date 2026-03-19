using System;
using UnityEngine;

public class PlayerAudioAdapter : MonoBehaviour
{
    public PlayerController player;
    public string stepSoundName;
    public string landSoundName;
    public string highLandSoundName;
    public string jumpSoundName;
    public string crouchSoundName;

    void OnEnable()
    {
        player.Movement.OnStep += PlayStepSound;
        player.Movement.OnLand += PlayLandSound;
        player.Movement.OnHighLand += PlayHighLandSound;
        player.Movement.OnJumped += PlayJumpSound;
        // player.OnCrouched += PlayCrouchSound;
    }

    void OnDisable()
    {
        player.Movement.OnStep -= PlayStepSound;
        player.Movement.OnLand -= PlayLandSound;
        player.Movement.OnHighLand -= PlayHighLandSound;
        player.Movement.OnJumped -= PlayJumpSound;
        // player.OnCrouched -= PlayCrouchSound;
    }

    public void PlayStepSound()
    {
        AudioController.Instance.PlaySound(stepSoundName);
    }

    public void PlayLandSound()
    {
        AudioController.Instance.PlaySound(landSoundName);
    }

    public void PlayHighLandSound()
    {
        AudioController.Instance.PlaySound(highLandSoundName);
    }

    public void PlayJumpSound()
    {
        AudioController.Instance.PlaySound(jumpSoundName);
    }

    public void PlayCrouchSound()
    {
        AudioController.Instance.PlaySound(crouchSoundName);
    }
}
