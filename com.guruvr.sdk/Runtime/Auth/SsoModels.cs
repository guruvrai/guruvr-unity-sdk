using System;

namespace GuruVR.SDK
{
    [Serializable]
    public class SsoLoginInitResponse
    {
        public string auth_url;
    }

    [Serializable]
    public class SsoExchangeRequest
    {
        public string code;
        public string redirect_uri;
        public string state;
    }
}
