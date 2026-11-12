using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameAudio : MonoBehaviour
{
    public static GameAudio instance { get; private set; }
    public AudioClip hit, heal, repair, launch, victory;
    [Range(0, 1)] public float volume = 0.65f;
    AudioSource source;
    void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0;
    }
    void OnDestroy() { if (instance == this) instance = null; }
    public void Play(AudioClip clip)
    {
        if (clip != null) source.PlayOneShot(clip, volume);
    }
}
