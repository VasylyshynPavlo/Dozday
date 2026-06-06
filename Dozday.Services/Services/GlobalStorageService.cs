using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Dozday.Services.Services
{
    public class GlobalStorageService
    {
        private string? _token;
        public string? UserId { get; private set; } = null;
        public Action? OnTokenChanged;
        public string? Token
        {
            get => _token;
            set
            {
                _token = value;

                if (string.IsNullOrWhiteSpace(value))
                {
                    UserId = null;
                    OnTokenChanged?.Invoke();
                    return;
                }

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(value);

                UserId = jwt.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

                OnTokenChanged?.Invoke();
            }
        }
    }
}
