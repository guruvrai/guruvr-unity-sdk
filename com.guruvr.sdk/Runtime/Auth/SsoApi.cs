using System;
using System.Collections;
using UnityEngine;

namespace GuruVR.SDK
{
    public class SsoApi
    {
        private readonly ApiConfig _cfg;

        public SsoApi(ApiConfig cfg) => _cfg = cfg;

        private string Url(string path) => _cfg.baseUrl.TrimEnd('/') + "/" + path.TrimStart('/');

        // Calls: GET /auth/sso/{provider}/login?redirect_uri=...
        public IEnumerator GetAuthUrl(string provider, string redirectUri,
            Action<string> onOk,
            Action<string> onFail)
        {
            if (string.IsNullOrEmpty(provider)) { onFail("provider is empty"); yield break; }
            if (string.IsNullOrEmpty(redirectUri)) { onFail("redirectUri is empty"); yield break; }

            var p = Uri.EscapeDataString(provider);
            var r = Uri.EscapeDataString(redirectUri);

            var url = Url($"/auth/sso/{p}/login?redirect_uri={r}");

            SsoLoginInitResponse parsed = null;

            yield return UnityHttp.SendJson(
                "GET",
                url,
                jsonBody: null,
                bearerToken: null,
                timeoutSeconds: _cfg.timeoutSeconds,
                onOk: (code, text) =>
                {
                    try { parsed = JsonUtility.FromJson<SsoLoginInitResponse>(text); }
                    catch (Exception e) { onFail("SSO parse error: " + e.Message + "\nRaw: " + text); }
                },
                onFail: (code, text) => onFail($"SSO init failed HTTP {code}: {text}")
            );

            if (parsed == null || string.IsNullOrEmpty(parsed.auth_url))
            {
                onFail("SSO init success but auth_url missing");
                yield break;
            }

            onOk(parsed.auth_url);
        }
    }
}
