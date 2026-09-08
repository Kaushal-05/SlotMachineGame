using UnityEngine;

// Hnadles all the Audio for the game, including spin, win, and bet change sounds. 

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Header("Clips")]
    [SerializeField] private AudioClip spinSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip betChangeSound;
    [SerializeField] private AudioClip jackpotSound;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySpin()
    {
        if (spinSound) source.PlayOneShot(spinSound);
    }

    public void PlayWin()
    {
        if (winSound) source.PlayOneShot(winSound);
    }

    public void PlayBetChange()
    {
        if (betChangeSound) source.PlayOneShot(betChangeSound);
    }

    public void PlayJackpot()
    {
        if (jackpotSound) source.PlayOneShot(jackpotSound);
    }
}
