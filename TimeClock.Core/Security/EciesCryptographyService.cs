using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Security
{
    internal sealed class EciesCryptographyService : IDisposable, ICryptographyService
    {
        private bool DisposedValue { get; set; } = false;
        private ECDiffieHellman EcdProvider { get; set; }
        private Aes AesProvider { get; set; }

#warning CONTINUATION
        /*
         * need to implement pattern used in RsaCryptographyService
         * next, use Ecies class for implementation details
         * see link below for how to use windows key container for private key
         * https://billatnapier.medium.com/persistent-keys-in-aes-on-windows-1cfae7727023
         */

        public EciesCryptographyService()
        {
            this.EcdProvider = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP521);
            this.AesProvider = Aes.Create();

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
            if (!this.GetIfIdExists(id))
                this.CreateNewWithId(id);

            if (this.EcdProvider == default || this.AesProvider == default)
                throw new KeyNotFoundException($"An error occured while trying to access or create {id}");
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
            throw new NotImplementedException(); 
        }

        public string Encrypt(string value)
        {
            this.EcdProvider.DeriveKeyMaterial(new PubKey(new byte[0]));
#warning CONTINUATION
            throw new NotImplementedException();
        }

        public static bool IdExists(string id)
        {
            var bobo = ECDiffieHellmanCng.Create();
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
        private bool GetIfIdExists(string id)
        {
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
        #endregion

        private void Dispose(bool disposing)
        {
            if (!this.DisposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.EcdProvider.Dispose();
                    this.AesProvider.Dispose();
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

    public class PubKey : ECDiffieHellmanPublicKey
    {
        private byte[] _key;
        public PubKey(byte[] key)
        {
            this._key = key;
        }

        public override ECParameters ExportExplicitParameters()
        {
            return base.ExportExplicitParameters();
        }

        public override ECParameters ExportParameters()
        {
            return base.ExportParameters();
        }

        public override bool TryExportSubjectPublicKeyInfo(Span<byte> destination, out int bytesWritten)
        {
            return base.TryExportSubjectPublicKeyInfo(destination, out bytesWritten);
        }

        public override byte[] ExportSubjectPublicKeyInfo()
        {
            //var bobo = new SubjectPublicKeyInfo();
            //var pki = new PublicKey(new Oid(""), new AsnEncodedData("", //PublicKeyInfo();
            int intVal = 123456;
            byte[] bytes = new byte[4];
            BinaryPrimitives.WriteInt32BigEndian(bytes, intVal);
            return this._key;
        }
    }
}
