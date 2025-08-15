using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Colyseus;
using ColyseusSchemas.Response;
using ColyseusSchemas.State;

public class ColyseusManager : MonoBehaviour
{
    public static ColyseusManager Instance;

    [Header("Colyseus Settings")]
    public string serverUrl = "ws://localhost:2567";

    private ColyseusClient client;
    private ColyseusRoom<LoginState> loginRoom;
    private ColyseusRoom<WorldState> worldRoom;
    private bool localServer = false;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        TextManager.Instance.AddLine("Connecting to mainframe..");
        
        client = new ColyseusClient(serverUrl);

        TextManager.Instance.AddLine("Connected.");
        TextManager.Instance.AddLine("Identify yourself.");
        
        _ = Login();
    }
    
    private async Task Login()
    {
        var connectionUrl = serverUrl;
        if (localServer)
        {
            connectionUrl = "ws://localhost:2567";
        }
        
        client = new ColyseusClient(connectionUrl);
        
        var roomOptions = new Dictionary<string, object>
        {
            { "handle", "Pixyl" },
            { "password", "hunter2" }
        };
        
        try
        {
            worldRoom = await client.JoinOrCreate<WorldState>("world", roomOptions);
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ World login failed: " + ex.Message);
            return;
        }

        Debug.Log("✅ Entered world room. Session ID: " + worldRoom.SessionId);
        
        // Optional: handle server state
        worldRoom.OnStateChange += (state, isFirstState) =>
        {
            Debug.Log("🧠 WorldRoom state updated.");
        };

        worldRoom.OnMessage<TerminalMessageResponse>("terminal_msg", msg =>
        {
            TextManager.Instance.AddLine(msg.msg);
        });

        worldRoom.OnMessage<LoginSuccessResponse>("login_success", async data =>
        {
            TextManager.Instance.AddLine("Authenticated as " + data.handle);
            
            Debug.Log("Login success.");
            Debug.Log("Token: " + data.token);
        });
    }

    public async Task SendCommand(string commandString)
    {
        await worldRoom.Send("command", new
        {
            cmd = commandString
        });
    }
    
    private async void OnApplicationQuit()
    {
        if (worldRoom != null)
        {
            await worldRoom.Leave();
            Debug.Log("🔌 Graceful leave sent to server.");
        }
    }
}
