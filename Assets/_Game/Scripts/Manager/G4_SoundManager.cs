using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SoundFX
{
    PopBall,
    Win,
    Lose
}
public class G4_SoundManager : G4_GOSingleton<G4_SoundManager>
{
    [SerializeField] List<AudioSource> audioSources;
    [SerializeField] AudioSource musicSource;
    Dictionary<SoundFX,AudioClip> clipSounds = new Dictionary<SoundFX, AudioClip>();
    Queue<AudioSource> sfxSources;
    private void Awake()
    {
        OnInit();
    }
    public void OnInit()
    {
        sfxSources = new Queue<AudioSource>(audioSources);
        clipSounds[SoundFX.PopBall] = Resources.Load<AudioClip>("Sounds/PopBall");
        clipSounds[SoundFX.Win] = Resources.Load<AudioClip>("Sounds/Win");
        clipSounds[SoundFX.Lose] = Resources.Load<AudioClip>("Sounds/Lose");
        EnableMusic(UserData.IsMusicOn);
    }
    public void PlayOneShot(SoundFX soundFX)
    {
        if (!UserData.IsSoundOn)
        {
            return;
        }
        AudioSource source = sfxSources.Dequeue();
        source.clip = clipSounds[soundFX];
        source.Play();
        sfxSources.Enqueue(source);
    }
    public void EnableMusic(bool active)
    {
        if (active)
        {
            musicSource.Play();
        }
        else
        {
            musicSource.Stop();
        }
    }
}
