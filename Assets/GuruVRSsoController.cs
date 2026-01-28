using UnityEngine;
using GuruVR.SDK;

public class GuruVRSsoController : MonoBehaviour
{
    public ApiConfig config;

    [Header("SSO")]
    public string provider = "google";

    // Your current swagger example uses web callback:
    public string redirectUri = "https://creator.guruvr.ai/auth/google/callback";

    private SsoApi _sso;

    private void Awake()
    {
        _sso = new SsoApi(config);
    }

    public void StartSso()
    {
        StartCoroutine(_sso.GetAuthUrl(
            provider,
            redirectUri,
            authUrl =>
            {
                Debug.Log("Opening browser SSO URL...");
                Application.OpenURL(authUrl);
            },
            err => Debug.LogError("SSO init failed: " + err)
        ));
    }
}
