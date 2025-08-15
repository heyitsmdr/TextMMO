using System.Collections;
using FronkonGames.Retro.CRTTV;
using UnityEngine;

public class CrtManager : MonoBehaviour
{
    public static CrtManager Instance;

    private CRTTV crtInstance;
    public float transitionDuration = 5f; // Duration to bring signal from 1 to 0
    public float startSignal = 1f;
    public float endSignal = 0f;
    public float startFisheye = 5f;
    public float endFisheye = 0.05f;
    
    void Start()
    {
        // Ensure only one instance exists (i.e. a singleton)
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one CRTManager in scene.");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Persist across scenes.
        DontDestroyOnLoad(gameObject);
        
        crtInstance = CRTTV.Instance;

        Debug.Log("CRTManager has been instantiated");
        
        // Start the signal adjustment
        StartCoroutine(PowerOnRoutine());
    }

    private IEnumerator PowerOnRoutine()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            // Lerp from startSignal to endSignal over time and set the property
            crtInstance.settings.interferenceStrength = Mathf.Lerp(startSignal, endSignal, elapsedTime / transitionDuration);

            // Lerp from startFisheye to endFisheye over time and set the property
            // crtInstance.settings.fishEyeStrength = Mathf.Lerp(startFisheye, endFisheye, elapsedTime / transitionDuration);

            yield return null; // Wait for the next frame
        }

        // Ensure the value is exactly 0 at the end
        crtInstance.settings.interferenceStrength = 0;
    }

    public void SwitchScreens()
    {
        StartCoroutine(_SwitchScreenRoutine());
    }
    
    private IEnumerator _SwitchScreenRoutine()
    {
        crtInstance.settings.interferenceStrength = 1f;
        yield return new WaitForSeconds(0.3f);
        crtInstance.settings.interferenceStrength = 0f;
    }
}