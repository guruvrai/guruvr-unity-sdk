using System;

namespace GuruVR.SDK
{
    public static class SsoApi
    {
        // Swagger ref:
        // https://cbackenddev.guruvrmetaversity.com/docs#/Authentication/sso_login_auth_sso__provider__login_get
        // GET /auth/sso/{provider}/login?redirect_uri=...

        public static string BuildSsoLoginUrl(ApiConfig cfg, string provider, string redirectUri)
        {
            var baseUrl = (cfg?.baseUrl ?? "").TrimEnd('/');
            if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentException("ApiConfig.baseUrl is empty");
            if (string.IsNullOrEmpty(provider)) throw new ArgumentException("provider is empty");
            if (string.IsNullOrEmpty(redirectUri)) throw new ArgumentException("redirectUri is empty");

            var p = Uri.EscapeDataString(provider);
            var r = Uri.EscapeDataString(redirectUri);

            return $"{baseUrl}/auth/sso/{p}/login?redirect_uri={r}";
        }
    }
}
