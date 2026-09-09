using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý nhạc nền (BGM) chạy xuyên suốt và liền mạch qua tất cả các scene (Level 1 -> Dress_Game -> Level 2 -> Level 3,...).
/// Tự động phát ngay từ đầu Level 1 và giữ liên tục không gián đoạn.
/// </summary>
public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    [Header("BGM Settings")]
    [SerializeField] private AudioClip bgmClip;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.2f;
    [SerializeField] private bool autoPlayOnStart = true;
    [SerializeField] private bool loop = true;

    private AudioSource _audioSource;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoBootstrap()
    {
        if (Instance == null)
        {
            GameObject bgmObject = new GameObject("[BackgroundMusicManager]");
            bgmObject.AddComponent<BackgroundMusicManager>();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureAudioSource();
        EnsureAudioClipLoaded();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        AudioListener.volume = 1f;
        AudioListener.pause = false;

        EnsureAudioSource();
        EnsureAudioClipLoaded();

        if (autoPlayOnStart && bgmClip != null && !_audioSource.isPlaying)
        {
            PlayBGM(bgmClip, volume);
        }
    }

    private void Update()
    {
        // Đảm bảo Level 1 phát ngay từ frame đầu tiên nếu Awake/Start chưa kịp phát
        if (autoPlayOnStart && _audioSource != null && !_audioSource.isPlaying)
        {
            EnsureAudioClipLoaded();
            if (bgmClip != null)
            {
                PlayBGM(bgmClip, volume);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioListener.volume = 1f;
        AudioListener.pause = false;

        EnsureAudioSource();
        EnsureAudioClipLoaded();

        if (autoPlayOnStart && bgmClip != null && !_audioSource.isPlaying)
        {
            PlayBGM(bgmClip, volume);
        }
    }

    private void EnsureAudioSource()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
        }

        _audioSource.playOnAwake = false;
        _audioSource.loop = loop;
        _audioSource.spatialBlend = 0f; // 2D Stereo
        _audioSource.dopplerLevel = 0f;
        _audioSource.volume = volume;
        _audioSource.mute = false;
        _audioSource.ignoreListenerPause = true;
    }

    private void EnsureAudioClipLoaded()
    {
        if (bgmClip != null)
            return;

        bgmClip = Resources.Load<AudioClip>("Audios/BGM");
        if (bgmClip == null)
            bgmClip = Resources.Load<AudioClip>("BGM");
        if (bgmClip == null)
            bgmClip = Resources.Load<AudioClip>("geoffreyburch-the-ghost-of-shepardx27s-pie-glbml-112816");

        if (bgmClip == null)
        {
            AudioClip[] allClips = Resources.FindObjectsOfTypeAll<AudioClip>();
            foreach (AudioClip clip in allClips)
            {
                if (clip == null) continue;
                if (clip.name.Contains("geoffreyburch") || clip.name.Contains("BGM") || clip.name.Contains("shepard"))
                {
                    bgmClip = clip;
                    break;
                }
            }
        }

        if (bgmClip == null)
        {
            Debug.LogWarning("[BackgroundMusicManager] Không tìm thấy AudioClip BGM trong Resources.");
        }
    }

    public void PlayBGM(AudioClip clip, float targetVolume = -1f)
    {
        EnsureAudioSource();

        if (clip == null)
            return;

        bgmClip = clip;

        if (targetVolume >= 0f)
        {
            volume = Mathf.Clamp01(targetVolume);
            _audioSource.volume = volume;
        }

        // Nếu đang phát chính bài này thì giữ nguyên liền mạch
        if (_audioSource.isPlaying && _audioSource.clip == clip)
            return;

        AudioListener.volume = 1f;
        AudioListener.pause = false;

        _audioSource.clip = clip;
        _audioSource.loop = true;
        _audioSource.volume = volume;
        _audioSource.mute = false;
        _audioSource.Play();

        Debug.Log($"[BackgroundMusicManager] Đang phát nhạc nền liền mạch: {clip.name} (Volume: {_audioSource.volume:P0})");
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (_audioSource != null)
            _audioSource.volume = volume;
    }

    public void PauseBGM()
    {
        if (_audioSource != null && _audioSource.isPlaying)
            _audioSource.Pause();
    }

    public void ResumeBGM()
    {
        if (_audioSource != null && !_audioSource.isPlaying)
            _audioSource.UnPause();
    }

    public void StopBGM()
    {
        if (_audioSource != null)
            _audioSource.Stop();
    }
}
