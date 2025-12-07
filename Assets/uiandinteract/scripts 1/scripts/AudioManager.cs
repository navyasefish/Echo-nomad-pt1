using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source Pool")]
    [SerializeField] private int poolSize = 10;

    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
    private List<AudioSource> activeAudioSources = new List<AudioSource>();

    // 🔥 Dedicated UI audio source (prevents overlap issues)
    private AudioSource uiAudioSource;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        InitializePool();
        InitializeUIAudioSource();
    }

    void InitializeUIAudioSource()
    {
        uiAudioSource = gameObject.AddComponent<AudioSource>();
        uiAudioSource.playOnAwake = false;
        uiAudioSource.spatialBlend = 0f; // 2D UI sound
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject($"PooledAudioSource_{i}");
            obj.transform.SetParent(transform);

            AudioSource source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 1f;
            source.maxDistance = 20f;
            source.rolloffMode = AudioRolloffMode.Linear;

            audioSourcePool.Enqueue(source);
        }
    }

    /// <summary>
    /// ⭐ UI Sounds – only one can play at a time
    /// </summary>
    public void PlayUISound(AudioClip clip, float volume = 1f)
    {
        if (clip == null)
            return;

        // If UI sound is already playing → ignore next clicks
        if (uiAudioSource.isPlaying)
            return;

        uiAudioSource.clip = clip;
        uiAudioSource.volume = volume;
        uiAudioSource.Play();
    }

    /// <summary>
    /// ⭐ 3D sounds (world sounds)
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

    AudioSource GetAvailableSource()
    {
        if (audioSourcePool.Count > 0)
        {
            return audioSourcePool.Dequeue();
        }
        else
        {
            Debug.LogWarning("AudioManager: Pool exhausted, creating extra source");
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
    /// Stop all active sounds (except UI sound)
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
