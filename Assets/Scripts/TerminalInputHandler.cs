using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TerminalInputHanlder : MonoBehaviour
{
    private TMP_InputField inputField;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        
        inputField.ActivateInputField();
        
        inputField.onSubmit.AddListener(HandleSubmit);
        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    void OnDestroy()
    {
        inputField.onSubmit.RemoveListener(HandleSubmit);
        inputField.onValueChanged.RemoveListener(OnValueChanged);
    }

    void HandleSubmit(string text)
    {
        _ = OnSubmit(text);
    }

    async Task OnSubmit(string command)
    {
        if (string.IsNullOrEmpty(command))
        {
            inputField.ActivateInputField();
            return;
        }
        
        await ProcessCommand(command);
        
        inputField.text = "";
        inputField.ActivateInputField();
    }

    void OnValueChanged(string command)
    {
        AudioManager.Instance.PlaySFX(SFX.BEEP);
    }
    
    async Task ProcessCommand(string command)
    {
        bool isProduction = !Application.isEditor && !Debug.isDebugBuild;
        
        TextManager.Instance.AddLine("\n<color=white>::: " + command + "</color>");
        
        // Internal commands
        if (!isProduction)
        {
            if (command.ToLower() == "dev")
            {
                ColyseusManager.Instance.SetLocal();
                TextManager.Instance.AddLine("Local development enabled.");
                
                return;
            }
        }

        if (command.ToLower() == "login")
        {
            _ = ColyseusManager.Instance.Login();
            return;
        }
        
        await ColyseusManager.Instance.SendCommand(command);        
    }
}
