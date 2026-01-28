using UnityEngine;

namespace GuruVR.SDK
{
    [CreateAssetMenu(menuName = "GuruVR/SDK/ApiConfig")]
    public static class ApiConfig : ScriptableObject
    {
        [Header("Base URL")]
        public string baseUrl = "https://cbackend.guruvr.ai/";

        [Header("Timeouts")]
        public int timeoutSeconds = 30;

        [Header("Auth Paths (from OpenAPI)")]
        public string registerPath = "/auth/register";
        public string loginPath = "/auth/login";
        public string refreshPath = "/auth/refresh";
        public string mePath = "/auth/me";
    }
}
