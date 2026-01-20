using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    public AudioClip[] media;
    public AudioSource source;
    public AudioSource musicSource;
    public AudioSource clickSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //PlayIntroAndLoop();
    }

    public void PlaySound(int sound)
    {
        if(sound == 2)
        {
            clickSource.PlayOneShot(media[sound]);  
        }
        else
        {
            source.PlayOneShot(media[sound]);
        }
    }

    public void PlayIntroAndLoop()
    {
        if(!musicSource.isPlaying)
        {
            float delayLength = media[0].length;
            musicSource.PlayOneShot(media[0]);
            musicSource.clip = media[1];
            musicSource.loop = true;
            musicSource.PlayDelayed(delayLength);
        }
    }

    public void StopSong()
    {
        musicSource.Stop();
    }
}
