using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G4_SoundManager : MonoBehaviour
{
    [SerializeField] List<AudioSource> audioSources;
    Queue<AudioSource> sfxSources;
    private void Awake()
    {
        sfxSources = new Queue<AudioSource>(audioSources);
    }
    public void OnInit()
    {
    }
    public void PlayOneShot(AudioClip clip)
    {
        AudioSource source = sfxSources.Dequeue();
        source.clip = clip;
        source.Play();
        sfxSources.Enqueue(source);
    }
}
