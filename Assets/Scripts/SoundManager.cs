using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip FP_tile_on;
    public AudioClip FP_tile_off;
    public AudioClip BJP_door_open;
    public AudioClip BJP_stick;
    public AudioClip polaroid;
    public AudioClip knock;
    public AudioClip gunshot;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponents<AudioSource>()[2];
    }


    public void Play(string name) {
        audioSource.Stop();
        if (name == "FP_tile_on") {
            audioSource.clip = FP_tile_on;
            audioSource.Play();
        }
        if (name == "FP_tile_off") {
            audioSource.clip = FP_tile_off;
            audioSource.Play();
        }
        if (name == "BJP_door_open") {
            audioSource.clip = BJP_door_open;
            audioSource.Play();
        }
        if (name == "BJP_stick") {
            audioSource.clip = BJP_stick;
            audioSource.Play();
        }
        if (name == "polaroid") {
            audioSource.clip = polaroid;
            audioSource.Play();
        }
        if (name == "knock") {
            audioSource.clip = knock;
            audioSource.Play();
        }
        if (name == "gunshot") {
            audioSource.clip = gunshot;
            audioSource.Play();
        }
    }
}
