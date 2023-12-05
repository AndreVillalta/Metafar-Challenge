﻿namespace Api.Entities.AppSettings
{
    public class JwtSettings
    {
        public string? Issuer { get; init; }
        public string? Audience { get; init; }
        public string? Key { get; init; }
    }
}
