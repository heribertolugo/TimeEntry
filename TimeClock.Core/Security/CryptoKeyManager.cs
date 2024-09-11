using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace TimeClock.Core.Security;

internal interface ICryptoKeyManager
{
    public Task<bool> IdExists(string id);
    public Task<bool> TryGet<T>(string id, out T? cryptography) where T : ICryptographyService;
    public Task<bool> Save(string id, ICryptographyService cryptography);
}

internal class MobilePlatformCryptoKeyManager : ICryptoKeyManager
{
    /// <summary>
    /// Checks if the provided ID corresponds to an existing RSA Crypto Service Provider parameter in the keystore.
    /// </summary>
    /// <param name="id">The ID to check against the keystore</param>
    /// <returns>a boolean representing if the ID exists in the keystore</returns>
    public async Task<bool> IdExists(string id)
    {
#if (ANDROID || IOS)
        string? key = await SecureStorage.GetAsync(id);
        return !(string.IsNullOrEmpty(key));
#else
        throw new NotSupportedException("Current platform is not supported");
#endif

        return false;
    }

    public Task<bool> Save(string id, ICryptographyService cryptography)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryGet<T>(string id, out T? cryptography) where T : ICryptographyService
    {
        throw new NotImplementedException();
    }
}

internal class FileCryptoKeyManager : ICryptoKeyManager
{
    public Task<bool> IdExists(string id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Save(string id, ICryptographyService cryptography)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryGet<T>(string id, out T? cryptography) where T : ICryptographyService
    {
        throw new NotImplementedException();
    }
}

internal class AzureKeyVaultCryptoKeyManager : ICryptoKeyManager
{
    public Task<bool> IdExists(string id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Save(string id, ICryptographyService cryptography)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryGet<T>(string id, out T? cryptography) where T : ICryptographyService
    {
        throw new NotImplementedException();
    }
}