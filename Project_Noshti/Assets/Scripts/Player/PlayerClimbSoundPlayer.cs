using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbSoundPlayer : MonoBehaviour
{
    public AudioSource soundPlayer;

    public int smallGripQuality = 2;
    public int mediumGripQuality = 5;
    public int largeGripQuality = 8;

    public AudioClip smallGripSound;
    public AudioClip mediumGripSound;
    public AudioClip largeGripSound;

    private void Start()
    {
        soundPlayer.loop = false;
    }

    public void ClimbMoveHappened(PlayerClimbingController.Move move)
    {
        PlaySoundBasedOnGripSize(move.ConnectingGrip.Quality);
    }

    private void PlaySoundBasedOnGripSize(int gripQuality)
    {
        if (gripQuality <= smallGripQuality)
        {
            PlaySound(smallGripSound);
            print("Played small");
        }
        else if (gripQuality <= mediumGripQuality)
        {
            PlaySound(mediumGripSound);
            print("Played medium");
        }
        else if (gripQuality <= largeGripQuality)
        {
            PlaySound(largeGripSound);
            print("Played large");

        }
    }

    private void PlaySound(AudioClip sound)
    {
        soundPlayer.Stop();
        soundPlayer.clip = sound;
        soundPlayer.Play();
    }

}
