using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // Ensure only one instance exists (i.e. a singleton)
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one GameManager in scene.");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Persist across scenes.
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("Game Manager has been instantiated");
    }

    void Start()
    {
        AudioManager.Instance.PlaySFX(SFX.POWER_ON, 0, 2.5f);
        StartCoroutine(AudioManager.Instance.PlayMusicAfterDelay(1f));
    }
}
