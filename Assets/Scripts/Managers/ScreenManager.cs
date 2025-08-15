using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class KeyCodeExtensions
{
    public static KeyCode ToKeyCode(this string fKey)
    {
        if (System.Enum.TryParse(fKey, out KeyCode keyCode))
        {
            return keyCode;
        }

        Debug.LogWarning($"Invalid F-Key string: {fKey}");
        return KeyCode.None;
    }
}

[System.Serializable]
public class ScreenConfig
{
    public GameObject screen;
    public string name;
    public string fKey;
}

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance;
    
    public List<ScreenConfig> screenConfigs;
    public TextMeshProUGUI windowNavigator;
    
    private int currentScreenIndex = 0;
    
    void Awake() 
    {
        // Ensure only one instance exists (i.e. a singleton)
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one ScreenManager in scene.");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Persist across scenes.
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UpdateWindowNavigator();
    }

    void UpdateWindowNavigator()
    {
        // Go through each screen and set the Window Navigator text
        string navigatorText = "";
        for (int i = 0; i < screenConfigs.Count; i++)
        {
            if (i == currentScreenIndex)
            {
                navigatorText += $"<color=white><b>{screenConfigs[i].fKey}</b>: {screenConfigs[i].name}</color>";   
            }
            else
            {
                navigatorText += $"<b>{screenConfigs[i].fKey}</b>: {screenConfigs[i].name}";
            }
        
            if (i < screenConfigs.Count - 1)
            {
                navigatorText += "   ";
            }
        }
        
        windowNavigator.SetText($">>> {navigatorText}");
    }
    
    void Update()
    {
        // Handle Tab key for toggling screens
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                // Move to the next screen
                currentScreenIndex = (currentScreenIndex + 1) % screenConfigs.Count;
            }
            else
            {
                // Move to the previous screen
                currentScreenIndex = (currentScreenIndex - 1 + screenConfigs.Count) % screenConfigs.Count;
            }

            ShowScreen();
        }

        // Handle F-key shortcuts for screens
        // TODO: Cache the key codes and prevent having to call ToKeyCode every frame.
        for (int i = 0; i < screenConfigs.Count; i++)
        {
            if (Input.GetKeyDown(screenConfigs[i].fKey.ToKeyCode()) && i != currentScreenIndex)
            {
                currentScreenIndex = i;
                ShowScreen();
            }
        }
    }
    
    private void ShowScreen()
    {
        CrtManager.Instance.SwitchScreens();
        AudioManager.Instance.PlaySFX(SFX.GLITCH);
        
        // Hide all screens
        foreach (var sc in screenConfigs)
        {
            if (sc.screen != null)
            {
                sc.screen.SetActive(false);
            }
        }

        ScreenConfig screenConfig = screenConfigs[currentScreenIndex];
        
        // Show the selected screen
        if (screenConfig.screen != null)
        {
            screenConfig.screen.SetActive(true);
        }
        
        UpdateWindowNavigator();
    }
}
