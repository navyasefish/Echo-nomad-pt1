using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source Pool")]
    [SerializeField] private int poolSize = 10;
    //[SerializeField] private GameObject audioSourcePrefab;

    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
    private List<AudioSource> activeAudioSources = new List<AudioSource>();

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePool();
    }

    void InitializePool()
    {
        // Create pool of audio sources
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject($"PooledAudioSource_{i}");
            obj.transform.SetParent(transform);

            AudioSource source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 1f; // 3D sound by default
            source.maxDistance = 20f;
            source.rolloffMode = AudioRolloffMode.Linear;

            audioSourcePool.Enqueue(source);
        }
    }

    /// <summary>
    /// Play a 3D sound at a specific position
    /// </summary>
    public void PlayAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Null clip provided");
            return;
        }

        AudioSource source = GetAvailableSource();
        source.transform.position = position;
        source.spatialBlend = 1f;
        source.clip = clip;
        source.volume = volume;
        source.Play();

        activeAudioSources.Add(source);
        StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
    }

    /// <summary>
    /// Play a 2D UI sound (no position)
    /// </summary>
    public void PlayUISound(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Null clip provided");
            return;
        }

        AudioSource source = GetAvailableSource();
        source.spatialBlend = 0f; // 2D sound
        source.clip = clip;
        source.volume = volume;
        source.Play();

        activeAudioSources.Add(source);
        StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
    }

    AudioSource GetAvailableSource()
    {
        if (audioSourcePool.Count > 0)
        {
            return audioSourcePool.Dequeue();
        }
        else
        {
            // Pool exhausted, create new source
            Debug.LogWarning("AudioManager: Pool exhausted, creating additional source");
            GameObject obj = new GameObject($"ExtraAudioSource_{activeAudioSources.Count}");
            obj.transform.SetParent(transform);
            AudioSource source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }
    }

    System.Collections.IEnumerator ReturnToPoolAfterPlay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay + 0.1f);

        activeAudioSources.Remove(source);
        source.Stop();
        source.clip = null;
        audioSourcePool.Enqueue(source);
    }

    /// <summary>
    /// Stop all currently playing sounds
    /// </summary>
    public void StopAllSounds()
    {
        foreach (AudioSource source in activeAudioSources)
        {
            source.Stop();
            audioSourcePool.Enqueue(source);
        }
        activeAudioSources.Clear();
    }
}