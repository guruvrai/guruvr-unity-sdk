using UnityEngine;

namespace GuruVR.SDK.Auth
{
    public class GuruVRSsoController : MonoBehaviour
    {
        public ApiConfig config;

        [Header("SSO")]
        public string provider = "google";
        public string redirectUri = "https://creator.guruvr.ai/auth/google/callback";

        private SsoApi _sso;

        private void Awake()
        {
            if (config == null)
            {
                Debug.LogError("GuruVRSsoController: ApiConfig not assigned");
                return;
            }

            _sso = new SsoApi(config);
        }

        public void StartSso()
        {
            if (_sso == null)
            {
                Debug.LogError("SSO not initialized");
                return;
            }

            StartCoroutine(
                _sso.GetAuthUrl(
                    provider,
                    redirectUri,
                    Application.OpenURL,
                    err => Debug.LogError("SSO init failed: " + err)
                )
            );
        }
    }
}
