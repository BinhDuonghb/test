﻿namespace Login.models.Request
{
    public class TokenResponse
    {

        public string Token { get; set; } = string.Empty;
        public string Refresh { get; set; } = string.Empty;
        public bool Success { get; set; }

    }
}
