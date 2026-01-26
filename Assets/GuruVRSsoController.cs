using System;
using UnityEngine;
using GuruVR.SDK;

public class GuruVRSsoController : MonoBehaviour
{
    public ApiConfig config;

    [Header("SSO")]
    public string provider = "google";
    public string redirectUri = "guruvr://auth/callback"; // deep link your app can receive

    [Header("Tokens (debug)")]
    public string accessToken;
    public string refreshToken;

    private void Awake()
    {
        Application.deepLinkActivated += OnDeepLinkActivated;

        // If app is launched by deep link while closed
        if (!string.IsNullOrEmpty(Application.absoluteURL))
            OnDeepLinkActivated(Application.absoluteURL);
    }

    public void StartSso()
    {
        var url = SsoApi.BuildSsoLoginUrl(config, provider, redirectUri);
        Debug.Log("Opening SSO URL: " + url);
        Application.OpenURL(url); // system browser
    }

    private void OnDeepLinkActivated(string url)
    {
        Debug.Log("Deep link received: " + url);

        // Expect: guruvr://auth/callback?access_token=...&refresh_token=...
        accessToken = GetQueryParam(url, "access_token");
        refreshToken = GetQueryParam(url, "refresh_token");

        if (!string.IsNullOrEmpty(accessToken))
            Debug.Log("✅ SSO Success. Access token received.");
        else
            Debug.LogWarning("SSO callback received but access_token is missing.");
    }

    private static string GetQueryParam(string url, string key)
    {
        try
        {
            var uri = new Uri(url);
            var query = uri.Query; // includes leading '?'
            if (string.IsNullOrEmpty(query)) return null;

            query = query.TrimStart('?');
            var parts = query.Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var kv = part.Split('=', 2);
                if (kv.Length != 2) continue;
                if (Uri.UnescapeDataString(kv[0]) == key)
                    return Uri.UnescapeDataString(kv[1]);
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    private void OnDestroy()
    {
        Application.deepLinkActivated -= OnDeepLinkActivated;
    }
}
