using System;
using System.Collections;
using UnityEngine;

namespace GuruVR.SDK
{
    public class AuthApi:MonoBehaviour
    {_cfg
        private readonly ApiConfig _cfg;

        public AuthApi(ApiConfig cfg) => _cfg = cfg;

        private string Url(string path) => _cfg.baseUrl.TrimEnd('/') + "/" + path.TrimStart('/');

        public IEnumerator Login(string username, string password,
            Action<LoginResponse> onOk,
            Action<string> onFail)
        {
            var form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);

            LoginResponse parsed = null;

            yield return UnityHttp.PostForm(
                Url(_cfg.loginPath),
                form,
                _cfg.timeoutSeconds,
                (code, text) =>
                {
                    try { parsed = JsonUtility.FromJson<LoginResponse>(text); }
                    catch (Exception e) { onFail("Login parse error: " + e.Message + "\nRaw: " + text); }
                },
                (code, text) => onFail($"Login failed HTTP {code}: {text}")
            );

            if (parsed != null) onOk(parsed);
        }

        public IEnumerator Refresh(string refreshToken,
            Action<LoginResponse> onOk,
            Action<string> onFail)
        {
            var body = JsonUtility.ToJson(new RefreshTokenRequest { refresh_token = refreshToken });
            LoginResponse parsed = null;

            yield return UnityHttp.SendJson(
                "POST",
                Url(_cfg.refreshPath),
                body,
                bearerToken: null,
                timeoutSeconds: _cfg.timeoutSeconds,
                onOk: (code, text) =>
                {
                    try { parsed = JsonUtility.FromJson<LoginResponse>(text); }
                    catch (Exception e) { onFail("Refresh parse error: " + e.Message + "\nRaw: " + text); }
                },
                onFail: (code, text) => onFail($"Refresh failed HTTP {code}: {text}")
            );

            if (parsed != null) onOk(parsed);
        }

        public IEnumerator GetMe(string accessToken,
            Action<UserBase> onOk,
            Action<string> onFail)
        {
            UserBase parsed = null;

            yield return UnityHttp.SendJson(
                "GET",
                Url(_cfg.mePath),
                jsonBody: null,
                bearerToken: accessToken,
                timeoutSeconds: _cfg.timeoutSeconds,
                onOk: (code, text) =>
                {
                    try { parsed = JsonUtility.FromJson<UserBase>(text); }
                    catch (Exception e) { onFail("GetMe parse error: " + e.Message + "\nRaw: " + text); }
                },
                onFail: (code, text) => onFail($"GetMe failed HTTP {code}: {text}")
            );

            if (parsed != null) onOk(parsed);
        }
    }
}
