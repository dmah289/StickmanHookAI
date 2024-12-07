using UnityEngine;
using System.Runtime.InteropServices;

public class WindowManager : MonoBehaviour
{
    // Import functions từ thư viện User32.dll để điều khiển cửa sổ (chỉ trên Windows)
    [DllImport("user32.dll")]
    private static extern int ShowWindow(System.IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern System.IntPtr GetActiveWindow();
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool CloseWindow(System.IntPtr hWnd);

    private const int SW_MINIMIZE = 6;
    private const int SW_MAXIMIZE = 3;
    private const int SW_RESTORE = 9;

    private System.IntPtr windowHandle;

    void Start()
    {
        windowHandle = GetActiveWindow();
    }

    // Hàm xử lý nút thu nhỏ
    public void MinimizeWindow()
    {
        ShowWindow(windowHandle, SW_MINIMIZE);
    }

    // Hàm xử lý nút collapse (toàn màn hình hoặc phục hồi)
    public void ToggleFullscreen()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreen = false; // Thoát khỏi chế độ toàn màn hình
            ShowWindow(windowHandle, SW_RESTORE); // Phục hồi cửa sổ
        }
        else
        {
            Screen.fullScreen = true; // Chuyển sang chế độ toàn màn hình
            ShowWindow(windowHandle, SW_MAXIMIZE); // Phóng to cửa sổ
        }
    }

    // Hàm xử lý nút thoát
    public void ExitApplication()
    {
        Application.Quit();
    }
}
