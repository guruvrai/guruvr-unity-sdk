using System;

namespace GuruVR.SDK
{
    [Serializable]
    public class LoginResponse
    {
        public string access_token;
        public string refresh_token;
        public string token_type;
        public int expires_in;
    }

    [Serializable]
    public class RefreshTokenRequest
    {
        public string refresh_token;
    }

    [Serializable]
    public class UserBase
    {
        public string user_id;
        public string email;
        public string name;
        public string org_id;
    }
}
