using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    static string code = "";
    static bool isPressed = false;

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();

            GUILayout.Label(isPressed ? "Loading..." : "");
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    static void StartButtons()
    {
        /*
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();*/

        if (GUILayout.Button("Host"))
        {
            StartHosting();
        }
        code = GUILayout.TextField(code);
        if (GUILayout.Button("Client"))
        {
            StartClient();
        }
    }

    static async void StartHosting()
    {
        if (isPressed) return;
        isPressed = true;

        //Initialize the Unity Services engine
        await UnityServices.InitializeAsync();
        //Always autheticate your users beforehand
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //If not already logged, log the user in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        var serverData = new RelayServerData(allocation, "udp");

        Debug.Log(joinCode);
        code = joinCode;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
        NetworkManager.Singleton.StartHost();
    }

    static async void StartClient()
    {
        if (isPressed) return;
        isPressed = true;

        //Initialize the Unity Services engine
        await UnityServices.InitializeAsync();
        //Always authenticate your users beforehand
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //If not already logged, log the user in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        //Ask Unity Services to join a Relay allocation based on our join code
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(code);
        var serverData = new RelayServerData(allocation, "udp");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
        NetworkManager.Singleton.StartClient();
    }

    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        GUILayout.Label("JoinCode: " + code);
    }
}
