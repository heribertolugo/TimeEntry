using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Security
{
    /// <summary>
    /// Provides a contract for cryptographical wrapper implementations. 
    /// When implementing this Interface, the following static methods must be overriden: 
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             <see cref="Encrypt(string, out string, out string)"/>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             <see cref="Decrypt(string, string)"/>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             <see cref="IdExists(string)"/>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             <see cref="TryGet(string, out ICryptographyService?)"/>
    ///         </description>
    ///     </item>
    /// </list>
    /// Not doing so will throw <see href="https://learn.microsoft.com/en-us/dotnet/api/system.notimplementedexception?view=net-8.0">NotImplementedException</see> when the methods are called.
    /// </summary>
    public interface ICryptographyService : IDisposable
    {
        string Id { get; }
        string PrivateKey { get; }
        string PublicKey { get; }

        string Decrypt(string value);
        string Encrypt(string value);
        public static virtual string Encrypt(string value, out string publicKey, out string privateKey)
        {
            throw new NotImplementedException();
        }
        public static virtual string Decrypt(string value, string privateKey)
        {
            throw new NotImplementedException();
        }
        public static virtual bool IdExists(string id)
        {
            throw new NotImplementedException();
        }
        public static virtual bool TryGet(string id, out ICryptographyService? cryptography)
        {
            throw new NotImplementedException();
        }
    }
}
