using System;
using System.Collections;
using UnityEngine;

namespace GuruVR.SDK
{
    /// <summary>
    /// Listens for deep link callbacks on Quest/Android and completes SSO.
    /// Attach to a GameObject in your login scene.
    /// </summary>
    public class QuestDeepLinkReceiver : MonoBehaviour
    {
        [SerializeField] private ApiConfig config;
        [SerializeField] private string provider = "google";
        [SerializeField] private string redirectUri = "guruvr://auth/google/callback";

        public event Action<AuthSession> OnSsoSuccess;
        public event Action<string> OnSsoFailed;

        private SsoApi _sso;

        private void Awake()
        {
            _sso = new SsoApi(config);

            Application.deepLinkActivated += OnDeepLinkActivated;

            // cold start case (app launched by deep link)
            if (!string.IsNullOrEmpty(Application.absoluteURL))
                OnDeepLinkActivated(Application.absoluteURL);
        }

        private void OnDestroy()
        {
            Application.deepLinkActivated -= OnDeepLinkActivated;
        }

        private void OnDeepLinkActivated(string url)
        {
            // Example:
            // guruvr://auth/google/callback?code=...&state=...
            if (string.IsNullOrEmpty(url)) return;

            var code = GetQueryParam(url, "code");
            var state = GetQueryParam(url, "state");
            var error = GetQueryParam(url, "error");

            if (!string.IsNullOrEmpty(error))
            {
                OnSsoFailed?.Invoke("SSO cancelled/failed: " + error);
                return;
            }

            if (string.IsNullOrEmpty(code))
            {
                // Ignore unrelated deep links
                return;
            }

            StartCoroutine(_sso.ExchangeCodeForSession(
                provider,
                redirectUri,
                code,
                state,
                onOk: session => OnSsoSuccess?.Invoke(session),
                onFail: err => OnSsoFailed?.Invoke(err)
            ));
        }

        private static string GetQueryParam(string url, string key)
        {
            try
            {
                var qIndex = url.IndexOf("?", StringComparison.Ordinal);
                if (qIndex < 0) return null;

                var query = url.Substring(qIndex + 1);
                var parts = query.Split('&');
                foreach (var p in parts)
                {
                    var kv = p.Split('=');
                    if (kv.Length != 2) continue;

                    if (Uri.UnescapeDataString(kv[0]) == key)
                        return Uri.UnescapeDataString(kv[1]);
                }
            }
            catch { /* ignore */ }

            return null;
        }
    }
}
