using System;
using System.Collections;
using UnityEngine;

namespace GuruVR.SDK
{
    public partial class SsoApi
    {
        /// <summary>
        /// POST /auth/sso/{provider}/exchange
        /// Body: { code, redirect_uri, state }
        /// Returns: LoginResponse (access_token, refresh_token, expires_in...)
        /// </summary>
        public IEnumerator ExchangeCodeForSession(
            string provider,
            string redirectUri,
            string code,
            string state,
            Action<AuthSession> onOk,
            Action<string> onFail)
        {
            if (string.IsNullOrEmpty(provider)) { onFail("provider is empty"); yield break; }
            if (string.IsNullOrEmpty(redirectUri)) { onFail("redirectUri is empty"); yield break; }
            if (string.IsNullOrEmpty(code)) { onFail("code is empty"); yield break; }

            var p = Uri.EscapeDataString(provider);
            var url = _cfg.baseUrl.TrimEnd('/') + $"/auth/sso/{p}/exchange";

            var body = JsonUtility.ToJson(new SsoExchangeRequest
            {
                code = code,
                redirect_uri = redirectUri,
                state = state
            });

            LoginResponse parsed = default;

            yield return UnityHttp.SendJson(
                method: "POST",
                url: url,
                jsonBody: body,
                bearerToken: null,
                timeoutSeconds: _cfg.timeoutSeconds,
                onOk: (http, text) =>
                {
                    try { parsed = JsonUtility.FromJson<LoginResponse>(text); }
                    catch (Exception e) { onFail("SSO exchange parse error: " + e.Message + "\nRaw: " + text); }
                },
                onFail: (http, text) => onFail($"SSO exchange failed HTTP {http}: {text}")
            );

            if (parsed == null || string.IsNullOrEmpty(parsed.access_token))
            {
                onFail("SSO exchange success but access_token missing");
                yield break;
            }

            onOk(AuthSession.FromLogin(parsed));
        }
    }
}
