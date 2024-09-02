using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Security;

/// <summary>
/// Encrypts or decrypts data using the .NET implementation of the RSA algorithm. 
/// Optionally stores or retrieves the encryption mechanism in or from a secure keystore. 
/// Pkcs1 padding is used in the cryptographic operation. 
/// <para></para>
/// See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider?view=net-8.0"/> for more information.
/// </summary>
/// <remarks>
/// If app migration is needed, the keys will have to be manually migrated. the location of these keys is dependent on the process privilege with which the app is running. 
/// <para>
/// </para>
/// See <see href="https://learn.microsoft.com/en-us/windows/win32/seccng/key-storage-and-retrieval?redirectedfrom=MSDN"/> for locations and more information.
/// </remarks>
internal sealed class RsaCryptographyService : IDisposable, ICryptographyService
{
    private static readonly int KeySize = 8192;
    private bool DisposedValue { get; set; } = false;
    private RSACryptoServiceProvider RsaProvider { get; set; }


    // transmission of public key through the wire is a vulnerability
    // see Eric Lippert's rant concerning this:
    // https://stackoverflow.com/a/7540173/6368401

    /// <summary>
    /// Creates an instance of an RSA Crypto Service Provider. 
    /// Exporting the public and private keys to the exposed properties. 
    /// The contents are not saved.
    /// </summary>
    public RsaCryptographyService()
    {
        var rsa = RSA.Create();
        this.RsaProvider = new RSACryptoServiceProvider(RsaCryptographyService.KeySize);
        this.PublicKey = Convert.ToBase64String(this.RsaProvider.ExportRSAPublicKey());
        this.PrivateKey = Convert.ToBase64String(this.RsaProvider.ExportRSAPrivateKey());
    }
    /// <summary>
    /// Creates an instance of an RSA Crypto Service Provider. 
    /// Exporting the public and private keys to the exposed properties. 
    /// The contents are saved to a secure keystore using the provided ID.
    /// </summary>
    /// <param name="id">A value used to store and retrieve the cryptographic data in a secure keystore within the operating system.</param>
    public RsaCryptographyService(string id)
    {
        if (!this.GetIfIdExists(id))
            this.CreateNewWithId(id);

        if (this.RsaProvider == default)
            throw new KeyNotFoundException($"An error occurred while trying to access or create {id}");
    }

    /// <summary>
    /// Creates an instance of an RSA Crypto Service Provider using the provided values. 
    /// Exporting the public and private keys to the exposed properties. 
    /// The contents are not saved.
    /// </summary>
    /// <param name="publicKey"></param>
    /// <param name="privateKey"></param>
    public RsaCryptographyService(string publicKey, string? privateKey)
    {
        this.RsaProvider = new RSACryptoServiceProvider(RsaCryptographyService.KeySize);
        this.PublicKey = publicKey;

        this.RsaProvider.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);

        if (privateKey is not null)
        {
            this.PrivateKey = privateKey;
            this.RsaProvider.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
        }
    }

    /// <summary>
    /// A value which is used to decrypt data which was encrypted with the <see cref="PublicKey"/>. 
    /// This value is meant to be secret using the highest priority, stored securely.
    /// </summary>
    public string PrivateKey { get; private set; } = string.Empty;
    /// <summary>
    /// A value which is used to encrypt data, but cannot be used to decrypt the data. 
    /// This value is for sharing with parties to provide a means of securing data through encryption, 
    /// which can then be opened only by the possessor of the corresponding private key.
    /// </summary>
    public string PublicKey { get; private set; } = string.Empty;
    /// <summary>
    /// A value which is used to store and retrieve the encryption secret/private key.
    /// </summary>
    public string Id { get; private set; } = string.Empty;

    /// <summary>
    /// Encrypts the provided value using the <see cref="PublicKey"/>
    /// </summary>
    /// <param name="value">The value to be encrypted</param>
    /// <returns>The base64 encoded value of the encrypted data</returns>
    public string Encrypt(string value)
    {
        byte[] data = Encoding.UTF8.GetBytes(value);
        byte[] encryptedData = this.RsaProvider.Encrypt(data, RSAEncryptionPadding.Pkcs1);

        return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// Decrypts encrypted data with the <see cref="PrivateKey"/>, which was encrypted using the <see cref="PublicKey"/>
    /// </summary>
    /// <param name="value">The base64 encoded value to be decrypted</param>
    /// <returns>The base64 decoded value of the encrypted data provided</returns>
    public string Decrypt(string value)
    {
        byte[] cipherData = Convert.FromBase64String(value);
        byte[] decryptedData = this.RsaProvider.Decrypt(cipherData, RSAEncryptionPadding.Pkcs1);
        return Encoding.UTF8.GetString(decryptedData);
    }

    /// <summary>
    /// Creates an instance of an RSA Crypto Service Provider. 
    /// Exporting the public and private keys to the exposed parameters and encrypting the provided value. 
    /// The contents are not saved and the RSA Crypto Service Provider is disposed.
    /// </summary>
    /// <param name="value">The value to be encrypted</param>
    /// <param name="publicKey">A base64 encoded value used to encrypt the data</param>
    /// <param name="privateKey">A base64 encoded value used to decrypt the data</param>
    /// <returns></returns>
    public static string Encrypt(string value, out string publicKey, out string privateKey)
    {
        byte[] encryptedData;

        using (RSACryptoServiceProvider rsa = new())
        {
            byte[] publicBytes = rsa.ExportRSAPublicKey();
            byte[] privateBytes = rsa.ExportRSAPrivateKey();

            publicKey = Convert.ToBase64String(publicBytes);
            privateKey = Convert.ToBase64String(privateBytes);

            byte[] data = Encoding.UTF8.GetBytes(value);
            encryptedData = rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        }

        return Convert.ToBase64String(encryptedData);
    }

    /// <summary>
    /// Creates an instance of an RSA Crypto Service Provider using the provided PrivateKey. 
    /// Decrypts the value provided using the provided PrivateKey. 
    /// The contents are not saved and the RSA Crypto Service Provider is disposed.
    /// </summary>
    /// <param name="value">The value to be decrypted</param>
    /// <param name="privateKey">A base64 encoded value used to decrypt the data</param>
    /// <returns></returns>
    public static string Decrypt(string value, string privateKey)
    {
        using (RSACryptoServiceProvider rsa = new())
        {
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

            byte[] cipherData = Convert.FromBase64String(value);
            byte[] decryptedData = rsa.Decrypt(cipherData, RSAEncryptionPadding.Pkcs1);
            return Encoding.UTF8.GetString(decryptedData);
        }
    }

    /// <summary>
    /// Checks if the provided ID corresponds to an existing RSA Crypto Service Provider parameter in the keystore.
    /// </summary>
    /// <param name="id">The ID to check against the keystore</param>
    /// <returns>a boolean representing if the ID exists in the keystore</returns>
    public static bool IdExists(string id)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return !string.IsNullOrWhiteSpace(Task.Run(() => SecureStorage.GetAsync(id)).GetAwaiter().GetResult());

        var cspParams = new CspParameters(1)
        {
            Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.UseExistingKey,
            KeyContainerName = id
        };

        try { using (var provider = new RSACryptoServiceProvider(cspParams)) { }; }
        catch (CryptographicException) { return false; }

        return true;

        // here goes another neat way to check, but not viable across machines as location can change
        // must iterate through each file until finding a hit
        //byte[] bytes = File.ReadAllBytes(@"%APPDATA%\Microsoft\Crypto\RSA\User SID\b06025aeb895fd4cd3344016711f7afe_e87684b5-8bac-45b2-ab57-41307503ccfe");
        //string containerName = Encoding.ASCII.GetString(bytes, 40, bytes[8] - 1);
    }

    /// <summary>
    /// Attempts to retrieve values from the secure keystore using the provided ID. 
    /// If successful will assign a new instance CryptographyService with the retrieved values to the cryptography parameter. 
    /// If unsuccessful will assign null to the cryptography parameter.
    /// </summary>
    /// <param name="id">The ID to check against the keystore</param>
    /// <param name="cryptography"></param>
    /// <returns>a boolean representing if the ID exists in the keystore</returns>
    public static bool TryGet(string id, out ICryptographyService? cryptography)
    {
        if (string.IsNullOrWhiteSpace(id) || !RsaCryptographyService.IdExists(id))
        {
            cryptography = null;
            return false;
        }

        cryptography = new RsaCryptographyService(id);
        return true;
    }

    #region Private Helpers
    private bool GetIfIdExists(string id)
    {
        bool success = false;
        if (RsaCryptographyService.IdExists(id))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var cspParams = new CspParameters(1)
                {
                    Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.UseExistingKey,
                    KeyContainerName = id
                };

                try 
                { 
                    this.RsaProvider = new RSACryptoServiceProvider(RsaCryptographyService.KeySize, cspParams);
                    success = true;
                }
                catch (CryptographicException) { }
            }
            else
            {
                try
                {
                    string? key = SecureStorage.GetAsync(id).GetAwaiter().GetResult();
                    if (string.IsNullOrEmpty(key))
                        return false;

                    RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(RsaCryptographyService.KeySize);
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(key), out _);
                    this.RsaProvider = rsa;
                    success = true;
                }
                catch (Exception) { }
            }
        }

        if (success)
        {
            this.Id = id;
            this.PublicKey = Convert.ToBase64String(this.RsaProvider.ExportRSAPublicKey());
            this.PrivateKey = Convert.ToBase64String(this.RsaProvider.ExportRSAPrivateKey());
        }

        return success;
    }
    private void CreateNewWithId(string id)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CspParameters parameters = new CspParameters(1) { KeyContainerName = id, Flags = CspProviderFlags.UseMachineKeyStore };
            this.RsaProvider = new RSACryptoServiceProvider(RsaCryptographyService.KeySize, parameters);
        }
        else
        {
            this.RsaProvider = new RSACryptoServiceProvider(RsaCryptographyService.KeySize);
#warning need to save to local file on non-Windows OS
        }

        this.Id = id;
        this.PublicKey = Convert.ToBase64String(this.RsaProvider.ExportRSAPublicKey());
        this.PrivateKey = Convert.ToBase64String(this.RsaProvider.ExportRSAPrivateKey());

#warning SecureStorage does NOT work on Linux/MacOS
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            SecureStorage.SetAsync(id, this.PrivateKey).GetAwaiter().GetResult();
    }
    #endregion Private Helpers

    #region IDisposable
    private void Dispose(bool disposing)
    {
        if (!this.DisposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                this.RsaProvider.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.DisposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~RsaCryptographyService()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion IDisposable
}
