using System;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

namespace GuruVR.SDK
{
    public static class UnityHttp
    {
        public static IEnumerator SendJson(
            string method,
            string url,
            string jsonBody,
            string bearerToken,
            int timeoutSeconds,
            Action<long, string> onOk,
            Action<long, string> onFail)
        {
            using var req = new UnityWebRequest(url, method);
            req.timeout = timeoutSeconds;

            if (!string.IsNullOrEmpty(jsonBody))
            {
                var bytes = Encoding.UTF8.GetBytes(jsonBody);
                req.uploadHandler = new UploadHandlerRaw(bytes);
                req.SetRequestHeader("Content-Type", "application/json");
            }

            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Accept", "application/json");

            if (!string.IsNullOrEmpty(bearerToken))
                req.SetRequestHeader("Authorization", "Bearer " + bearerToken);

            yield return req.SendWebRequest();

            var text = req.downloadHandler?.text ?? "";

            if (req.result == UnityWebRequest.Result.Success && req.responseCode >= 200 && req.responseCode < 300)
                onOk(req.responseCode, text);
            else
                onFail(req.responseCode, text);
        }

        public static IEnumerator PostForm(
            string url,
            WWWForm form,
            int timeoutSeconds,
            Action<long, string> onOk,
            Action<long, string> onFail)
        {
            using var req = UnityWebRequest.Post(url, form);
            req.timeout = timeoutSeconds;
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Accept", "application/json");

            yield return req.SendWebRequest();

            var text = req.downloadHandler?.text ?? "";

            if (req.result == UnityWebRequest.Result.Success && req.responseCode >= 200 && req.responseCode < 300)
                onOk(req.responseCode, text);
            else
                onFail(req.responseCode, text);
        }
    }
}
