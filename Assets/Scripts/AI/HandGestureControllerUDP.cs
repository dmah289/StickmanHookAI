using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class HandGestureControllerUDP : MonoBehaviour
{
    UdpClient udpServer;
    IPEndPoint remoteEndPoint;
    bool isRunning = true;

    private Process pythonProcess;
    private string model = "HandGestureUDP";

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        // Khởi tạo UDP server chỉ lắng nghe trên localhost và cổng 6060
        udpServer = new UdpClient();
        udpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpServer.Client.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6060));

        remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6060);
        print("UDP Server is listening on localhost:6060...");

        System.Threading.Thread serverThread = new System.Threading.Thread(() => ListenForData());
        serverThread.IsBackground = true;
        serverThread.Start();

        // StartPythonProcess();
    }

    void StartPythonProcess()
    {
        string exePath = Application.streamingAssetsPath + $"/{model}.exe";
        pythonProcess = new Process();
        pythonProcess.StartInfo.FileName = exePath;
        pythonProcess.StartInfo.WorkingDirectory = Application.streamingAssetsPath;
        pythonProcess.StartInfo.CreateNoWindow = true;
        pythonProcess.StartInfo.UseShellExecute = false;
        pythonProcess.StartInfo.RedirectStandardOutput = true;
        pythonProcess.StartInfo.RedirectStandardError = true;
        pythonProcess.Start();
    }

    void ListenForData()
    {
        while (isRunning)
        {
            try
            {
                byte[] buffer = udpServer.Receive(ref remoteEndPoint);
                string data = Encoding.UTF8.GetString(buffer);
                print(data);

                if (DataTransition.instance.gameState == GameState.GAMEPLAY)
                {
                    if (data == "SPACE_DOWN")
                        InputManager.TriggerSpacePressed();
                    else if (data == "SPACE_UP")
                        InputManager.TriggerSpaceReleased();
                }
                else if (DataTransition.instance.gameState == GameState.LOBBY)
                {
                    if(data == "LEFT")
                        InputManager.TriggerLeftPressed();
                    else if(data == "RIGHT")
                        InputManager.TriggerRightPressed();
                    else if(data == "PLAY")
                        InputManager.TriggerGameplayEntered();
                }
            }
            catch (Exception e) { }
        }
    }

    void OnApplicationQuit()
    {
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            // Hủy tất cả các tiến trình con trước khi hủy tiến trình cha
            foreach (var childProcess in Process.GetProcessesByName(model))
            {
                try
                {
                    childProcess.Kill();
                    childProcess.WaitForExit();
                }
                catch (Exception ex)
                {
                    print("Error stopping process: " + ex.Message);
                }
            }

            if (pythonProcess != null && !pythonProcess.HasExited)
            {
                try
                {
                    pythonProcess.Kill();
                    pythonProcess.WaitForExit();
                    pythonProcess.Dispose();
                }
                catch (InvalidOperationException ex)
                {
                    UnityEngine.Debug.LogWarning("Process has already exited: " + ex.Message);
                }
            }
        }

        isRunning = false;
        udpServer.Close();
    }
}
