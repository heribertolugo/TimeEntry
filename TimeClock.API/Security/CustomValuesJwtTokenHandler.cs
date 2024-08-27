using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TimeClock.Api.Helpers;

namespace TimeClock.Api.Security
{
    internal class CustomValuesJwtTokenHandler : JwtSecurityTokenHandler
    {
        private ITimeClockCacheDataProvider Cache {  get; init; }
        private SecretsProvider SecretsProvider { get; init; }

        public CustomValuesJwtTokenHandler(ITimeClockCacheDataProvider cache, SecretsProvider secretsProvider)
        {
            this.Cache = cache;
            this.SecretsProvider = secretsProvider;
        }

        protected override async void ValidateAudience(IEnumerable<string> audiences, JwtSecurityToken jwtToken, TokenValidationParameters validationParameters)
        {
            validationParameters.ValidAudiences = (await this.Cache.GetDevices()).Select(i => $"{this.SecretsProvider.JwtAudience}{i.Key:N}");
            base.ValidateAudience(audiences, jwtToken, validationParameters);
        }

        protected override string ValidateIssuer(string issuer, JwtSecurityToken jwtToken, TokenValidationParameters validationParameters)
        {
            validationParameters.ValidIssuer = this.SecretsProvider.GetJwtIssuer() ?? string.Empty;
            return base.ValidateIssuer(issuer, jwtToken, validationParameters);
        }

        protected override SecurityKey ResolveIssuerSigningKey(string token, JwtSecurityToken jwtToken, TokenValidationParameters validationParameters)
        {
#warning possible issue here when jwt has not been accessed in a month or so
            return base.ResolveIssuerSigningKey(token, jwtToken, validationParameters);
        }

        protected override JwtSecurityToken ValidateSignature(string token, TokenValidationParameters validationParameters)
        {
            return base.ValidateSignature(token, validationParameters);
        }
    }
}
