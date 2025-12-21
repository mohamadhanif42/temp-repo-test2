using Microsoft.Maui.Devices;

namespace CommunitySharing.Services;

public static class ApiConfig
{
    // STATIC property — evaluated at runtime
    public static string BaseUrl
    {
        get
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
                return "http://10.0.2.2:7080";       // Android emulator
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
                return "https://localhost:7080";      // Windows dev
            if (DeviceInfo.Platform == DevicePlatform.iOS)
                return "http://YOUR_PC_IP:7080";      // iOS device
            if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                return "http://YOUR_PC_IP:7080";      // Mac Catalyst

            return "http://YOUR_PC_IP:7080";          // fallback
        }
    }
}

