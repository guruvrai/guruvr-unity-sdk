using System;

namespace GuruVR.SDK
{
    [Serializable]
    public class AuthSession
    {
        public string accessToken;
        public string refreshToken;
        public long expiresAtUnix;

        public bool HasTokens => !string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken);

        public static AuthSession FromLogin(LoginResponse resp)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return new AuthSession
            {
                accessToken = resp.access_token,
                refreshToken = resp.refresh_token,
                expiresAtUnix = resp.expires_in > 0 ? now + resp.expires_in : 0
            };
        }
    }
}
