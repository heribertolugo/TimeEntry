using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Helpers;
internal class DeviceGuidHelper
{

    /// <summary>
    /// Generates a string using a GUID. 
    /// The first 3 sets of the GUID are summed individually, 
    /// and the last 2 sets are base64 encoded. 
    /// WARNING: the generated string does NOT have the uniqueness guarantee as an actual GUID would
    /// </summary>
    /// <returns>a friendly version of a GUID as a string</returns>
    /// <remarks>
    /// This method greatly reduces the collision factor that a GUID would return.
    /// Therefore, its uniqueness is also greatly reduced. 
    /// For the case of a few hundred generated values, there should be minimal if any collisions for 
    /// value returned by this method. A GUID is actually a collection of 3 integers followed by 8 bytes. 
    /// For example, the GUID d8e70e3b-b84d-4d15-9771-f4b9e6afac79 
    /// is a representation of 3639021115-47181-19733-[151,113]-[244,185,230,175,172,121]. Both the GUID and its true 
    /// value would be difficult for a user to spell or keep track of, mainly due to its length. For this reason, we are generating a simpler, 
    /// friendlier value that will be unique within the scope of its use. Creating a base64 representation of 
    /// the GUID actual value does reduce complexity, but still a little long in length. The previous GUID 
    /// would become: Ow7n2E24FU2XcfS55q+seQ==. So instead this method takes the first 3 sets as 
    /// byte values. Sums each set of byte values individually. Then the remaining 2 sets are base64 encoded. 
    /// This results in a fairly unique value, that is higher in simplicity. The aforementioned 
    /// GUID gets transformed to: 520-261-98-l3H0ueavrHk
    /// </remarks>
    public static string GuidToFriendly(Guid guid)
    {
        byte[] bytes = guid.ToByteArray();
        StringBuilder deviceId = new();
        // the length in bytes of each section in a GUID
        int[] guidSetLengths = [4, 2, 2, 2, 6];

        // get the first set (first 4 bytes) and sum the values
        deviceId.Append(bytes.Take(guidSetLengths[0]).Sum(b => (int)b));
        deviceId.Append('-');
        // skip first 4 bytes and sum the next 2
        deviceId.Append(bytes.Skip(guidSetLengths[0]).Take(guidSetLengths[1]).Sum(b => (int)b));
        deviceId.Append('-');
        // skip first 6 bytes and sum next 2
        deviceId.Append(bytes.Skip(guidSetLengths[0] + guidSetLengths[1]).Take(guidSetLengths[2]).Sum(b => (int)b));
        deviceId.Append('-');
        // base64 encode last 8 bytes, substituting non URL safe characters (the + and = signs)
        deviceId.Append(DeviceGuidHelper.Base64UrlEncode(bytes[(guidSetLengths[3] + guidSetLengths[4])..]));

        return deviceId.ToString().ToUpperInvariant();

        // return a clean version of encoded bytes to make it easier to read
        // we use ToByteArray because a GUID is in fact a series of hex numbers.
        // this makes the resulting string shorter, without losing its definition
        //return Base64UrlEncode(Guid.NewGuid().ToByteArray());
    }

    /// <summary>
    /// GUID bytes are not stored in direct correlation to a GUID string representation. 
    /// Depending on operating system, on Windows the first 3 sets of data in GUID is reversed. 
    /// This reverse happens individually per set. Consider the GUID d8e70e3b-b84d-4d15-9771-f4b9e6afac79. 
    /// When converted to bytes it will become: b3e07e8d-d48b-51d4-9771-f4b9e6afac79. 
    /// This method will reverse the necessary bytes and convert that value back to its original string representation of the GUID. 
    /// This method specifically targets Windows Endianess.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    private static string GuidBytesToString(byte[] bytes)
    {
        StringBuilder hex = new(bytes.Length * 2);
        string[] arr = new string[4];
        int i = 3;
        int flips = 0;
        foreach (byte b in bytes)
        {
            if (i > -1 && flips < 3)
            {
                arr[i] = string.Format("{0:x2}", b);
                i--;
                if (i < 0 && flips < 3)
                {
                    hex.Append(string.Join("", arr));
                    if (flips == 0)
                        Array.Resize(ref arr, 2);
                    i = 1;
                    flips++;
                }
                continue;
            }
            hex.AppendFormat("{0:x2}", b);
        }

        return hex.ToString();
    }

    /// <summary>
    /// RFC 7515 compliant base 64 encoding. 
    /// See <see href="https://datatracker.ietf.org/doc/html/rfc7515#appendix-C"/>
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private static string Base64UrlEncode(byte[] arg)
    {
        string s = Convert.ToBase64String(arg); // Regular base64 encoder
        s = s.Split('=')[0]; // Remove any trailing '='s
        s = s.Replace('+', '-'); // 62nd char of encoding
        s = s.Replace('/', '_'); // 63rd char of encoding
        return s;
    }
    static byte[] Base64UrlDecode(string arg)
    {
        string s = arg;
        s = s.Replace('-', '+'); // 62nd char of encoding
        s = s.Replace('_', '/'); // 63rd char of encoding
        switch (s.Length % 4) // Pad with trailing '='s
        {
            case 0: break; // No pad chars in this case
            case 2: s += "=="; break; // Two pad chars
            case 3: s += "="; break; // One pad char
            default:
                throw new Exception("Illegal base64url string!");
        }
        return Convert.FromBase64String(s); // Standard base64 decoder
    }
}
