using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Security;

internal sealed class EciesCryptographyService : IDisposable, ICryptographyService
{
    private bool DisposedValue { get; set; } = false;
    private ECDiffieHellman EcdProvider { get; set; }
    private ECCurve Curve;

#warning CONTINUATION
    /*
     * need to implement pattern used in RsaCryptographyService
     * next, use Ecies class for implementation details
     * see link below for how to use windows key container for private key
     * https://billatnapier.medium.com/persistent-keys-in-aes-on-windows-1cfae7727023
     */

    public EciesCryptographyService()
    {
        this.Curve = ECCurve.NamedCurves.nistP256;
        this.EcdProvider = ECDiffieHellman.Create(this.Curve);

        this.PublicKey = Convert.ToBase64String(this.EcdProvider.ExportSubjectPublicKeyInfo());
        this.PrivateKey = Convert.ToBase64String(this.EcdProvider.ExportPkcs8PrivateKey());
    }
    /// <summary>
    /// Creates an instnace of the default Elliptic Curve Diffie-Hellman (ECDH) algorithm. 
    /// Exporting the public and private keys to the exposed properties. 
    /// The contents are saved to a secure keystore using the provided ID.
    /// </summary>
    /// <param name="id">A value used to store and retreive the cryptographic data in a secure keystore within the operating system.</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public EciesCryptographyService(string id) : base()
    {
        throw new NotImplementedException();
        if (!this.GetIfIdExists(id))
            this.CreateNewWithId(id);
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Creates an instnace of an RSA Crypto Service Provider using the provided values. 
    /// Exporting the public and private keys to the exposed properties. 
    /// The contents are not saved.
    /// </summary>
    /// <param name="publicKey"></param>
    /// <param name="privateKey"></param>
    public EciesCryptographyService(string publicKey, string? privateKey) : base()
    {
        this.EcdProvider!.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);
        this.PublicKey = publicKey;

        if (privateKey is not null)
        {
            this.PrivateKey = privateKey;
            this.EcdProvider.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey), out _);
        }
    }


    public string Id { get; } = string.Empty;
    public string PrivateKey { get; } = string.Empty;
    public string PublicKey { get; } = string.Empty;

    public string Decrypt(string value)
    {
        var result = EciesCryptographyService.DeserializeEciesResult(value);
        return Encoding.UTF8.GetString(Ecies.Decrypt(this.EcdProvider, result, HashAlgorithmName.SHA512));
    }

    public string Encrypt(string value)
    {
        var result = Ecies.Encrypt(
            new EcdhPublicKey(Convert.FromBase64String(this.PublicKey), ECCurve.NamedCurves.nistP256)
            , Encoding.UTF8.GetBytes(value), HashAlgorithmName.SHA512);

        return EciesCryptographyService.SerializeEciesResult(result);
    }

    public static bool IdExists(string id)
    {
        throw new NotImplementedException();
    }

    public static string Decrypt(string value, string privateKey)
    {
        throw new NotImplementedException();
    }

    public static string Encrypt(string value, out string publicKey, out string privateKey)
    {
        throw new NotImplementedException();
    }

    public static bool TryGet(string id, out ICryptographyService? cryptography)
    {
        throw new NotImplementedException();
    }


    #region Private Helpers
    private static string SerializeEciesResult(EciesResult eciesResult)
    {
        byte[] combined = new byte[6 + eciesResult.EncodedEphemeralPoint.Length + eciesResult.Tag.Length + eciesResult.Ciphertext.Length];
        Buffer.BlockCopy(BitConverter.GetBytes((ushort)eciesResult.EncodedEphemeralPoint.Length), 0, combined, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes((ushort)eciesResult.Tag.Length), 0, combined, 2, 2);
        Buffer.BlockCopy(BitConverter.GetBytes((ushort)eciesResult.Ciphertext.Length), 0, combined, 4, 2);
        Buffer.BlockCopy(eciesResult.EncodedEphemeralPoint, 0, combined, 0 + 6, eciesResult.EncodedEphemeralPoint.Length);
        Buffer.BlockCopy(eciesResult.Tag, 0, combined, eciesResult.EncodedEphemeralPoint.Length + 6, eciesResult.Tag.Length);
        Buffer.BlockCopy(eciesResult.Ciphertext, 0, combined, eciesResult.EncodedEphemeralPoint.Length + eciesResult.Tag.Length + 6, eciesResult.Ciphertext.Length);
        return Convert.ToBase64String(combined);
    }
    private static EciesResult DeserializeEciesResult(string eciesResult)
    {
        byte[] combined = Convert.FromBase64String(eciesResult);
        ushort ephemeralLength = BitConverter.ToUInt16(combined[..2], 0);
        ushort hmacLength = BitConverter.ToUInt16(combined[2..2], 0);
        ushort cipherLength = BitConverter.ToUInt16(combined[4..2], 0);
        byte[] ephemeral = combined[6..ephemeralLength];
        byte[] hmac = combined[(6 + ephemeral.Length)..ephemeralLength];
        byte[] cipher = combined[(6 + ephemeral.Length + hmac.Length)..ephemeralLength];

        return new EciesResult(ephemeral, cipher, hmac);
    }
    private bool GetIfIdExists(string id)
    {
        throw new NotImplementedException();
        bool success = false;
        if (RsaCryptographyService.IdExists(id))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var cspParams = new CspParameters()
                {
                    Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.UseExistingKey,
                    KeyContainerName = id
                };

                try
                {
                    //this.AesProvider = new RSACryptoServiceProvider(RsaCryptographyService.KeySize, cspParams);
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

                    //RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(RsaCryptographyService.KeySize);
                    //rsa.ImportRSAPrivateKey(Convert.FromBase64String(key), out _);
                    //this.RsaProvider = rsa;
                    success = true;
                }
                catch (Exception) { }
            }
        }

        if (success)
        {
            //this.Id = id;
            //this.PublicKey = Convert.ToBase64String(this.RsaProvider.ExportRSAPublicKey());
            //this.PrivateKey = Convert.ToBase64String(this.RsaProvider.ExportRSAPrivateKey());
        }

        return success;
    }
    private void CreateNewWithId(string id)
    {
        throw new NotImplementedException();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CspParameters parameters = new CspParameters { KeyContainerName = id, Flags = CspProviderFlags.UseMachineKeyStore };
            //this.RsaProvider = new RSACryptoServiceProvider(RsaCryptographyService.KeySize, parameters);
        }
        //else
        //    this.RsaProvider = new RSACryptoServiceProvider(RsaCryptographyService.KeySize);

        //this.Id = id;
        //this.PublicKey = Convert.ToBase64String(this.RsaProvider.ExportRSAPublicKey());
        //this.PrivateKey = Convert.ToBase64String(this.RsaProvider.ExportRSAPrivateKey());

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            SecureStorage.SetAsync(id, this.PrivateKey).GetAwaiter().GetResult();
    }

    public static ICryptographyService ImportFromString(string data)
    {
        throw new NotImplementedException();
    }

    public string GetExportString()
    {
        throw new NotImplementedException();
    }
    #endregion

    private void Dispose(bool disposing)
    {
        if (!this.DisposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                this.EcdProvider.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.DisposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~EcdhCryptographyService()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
