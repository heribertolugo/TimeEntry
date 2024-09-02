using System.Formats.Asn1;
using System.Security.Cryptography;

namespace TimeClock.Core.Security;

public class EcdhPublicKey : ECDiffieHellmanPublicKey
{
    private static readonly string EcdhPublicKeyOid = "1.2.840.10045.2.1";
    private static readonly byte PointIndicator = 0x04;

    private static Dictionary<string, int> EcCurvesPointLength = new()
    {
        { ECCurve.NamedCurves.nistP256.Oid.Value!, 32 },
        { ECCurve.NamedCurves.nistP384.Oid.Value!, 48 },
        { ECCurve.NamedCurves.nistP521.Oid.Value!, 66 }
    };

    private bool IsLittleEndian { get; init; } // we should always assume big-endian
    private byte[] KeyBlob { get; init; }
    private byte[] KeyBlobX509 { get; init; }
    private ECCurve Curve { get; init; }

    public EcdhPublicKey() : base() { this.KeyBlob = []; this.KeyBlobX509 = []; }

    public EcdhPublicKey(byte[] bytes, ECCurve curve)//, bool areBytesLittleEndian = false
    {
        //this.IsLittleEndian = areBytesLittleEndian;
        // we should always assume big-endian. if we need to handle, then we would handle here and always keep in big-endian

        if (bytes[0] == EcdhPublicKey.PointIndicator)
        {
            this.KeyBlob = (byte[])bytes.Clone();
            this.KeyBlobX509 = EcdhPublicKey.ExportToX509(bytes, curve);
        }
        else if (bytes.Length == EcdhPublicKey.EcCurvesPointLength[curve.Oid.Value!])
        {
            this.KeyBlob = new byte[bytes.Length + 1];
            this.KeyBlob[0] = EcdhPublicKey.PointIndicator;
            Buffer.BlockCopy(bytes, 0, this.KeyBlob, 1, bytes.Length);
            this.KeyBlobX509 = EcdhPublicKey.ExportToX509(this.KeyBlob, curve);
        }
        else
        {
            this.KeyBlob = EcdhPublicKey.ConvertX509PublicKeyToPointFormat(bytes);
            this.KeyBlobX509 = bytes;
        }
        this.Curve = curve;
    }
    public EcdhPublicKey(ECParameters ecParameters)//, bool areBytesLittleEndian = false
    {
        //this.IsLittleEndian = areBytesLittleEndian;
        // we should always assume big-endian. if we need to handle, then we would handle here and always keep in big-endian
        this.KeyBlob = EcdhPublicKey.CreateByteArrayFromECParameters(ecParameters);
        this.Curve = ecParameters.Curve;
    }

    public override ECParameters ExportExplicitParameters()
    {
        return EcdhPublicKey.DeserializeBytes(this.KeyBlob, this.IsLittleEndian, this.Curve);
    }

    public override ECParameters ExportParameters()
    {
        return EcdhPublicKey.DeserializeBytes(this.KeyBlob, this.IsLittleEndian, this.Curve);
    }

    public override byte[] ExportSubjectPublicKeyInfo()
    {
        byte[] bytes = (byte[])this.KeyBlob.Clone();
        //if (BitConverter.IsLittleEndian != this.IsLittleEndian)
        //{
        //    Array.Reverse(bytes);
        //}
        return EcdhPublicKey.ExportToX509(bytes, this.Curve);
    }

    public override bool TryExportSubjectPublicKeyInfo(Span<byte> destination, out int bytesWritten)
    {
        byte[] bytes = (byte[])this.KeyBlob.Clone();
        //if (BitConverter.IsLittleEndian != this.IsLittleEndian)
        //{
        //    Array.Reverse(bytes);
        //}
        return EcdhPublicKey.TryExportToX509(bytes, this.Curve, destination, out bytesWritten);
    }

    private static byte[] CreateByteArrayFromECParameters(ECParameters ecParameters)
    {
        int pointLen = ecParameters.Q.X?.Length ?? ecParameters.Q.Y?.Length ?? ecParameters.D?.Length ?? 0;

        if (pointLen < 1)
        {
            throw new ArgumentException("ECParameters must include X, Y, or D values.");
        }

        int offset = 1;
        int totalLength = offset + (ecParameters.Q.X?.Length ?? 0) + (ecParameters.Q.Y?.Length ?? 0) + (ecParameters.D?.Length ?? 0);
        byte[] byteArray = new byte[totalLength];

        byteArray[0] = EcdhPublicKey.PointIndicator; // Uncompressed point indicator

        if ((ecParameters.Q.X?.Length ?? 0) != 0)
        {
            Buffer.BlockCopy(ecParameters.Q.X!, 0, byteArray, offset, pointLen);
            offset += pointLen;
        }
        if ((ecParameters.Q.Y?.Length ?? 0) != 0)
        {
            Buffer.BlockCopy(ecParameters.Q.Y!, 0, byteArray, offset, pointLen);
            offset += pointLen;
        }
        if ((ecParameters.D?.Length ?? 0) != 0)
        {
            Buffer.BlockCopy(ecParameters.D!, 0, byteArray, offset, pointLen);
        }

        return byteArray;
    }

    private static ECParameters DeserializePrivateKeyBytes(byte[] bytes, bool areBytesLittleEndian, ECCurve curve)
    {
        int pointLen = EcdhPublicKey.EcCurvesPointLength[curve.Oid.Value!];

        if (bytes.Length != pointLen)
        {
            throw new ArgumentException("Invalid byte array length for the specified curve.");
        }

        int offset = 1;
        byte[] paddedBytes = new byte[(pointLen * 3) + offset];

        paddedBytes[0] = EcdhPublicKey.PointIndicator; // Uncompressed point indicator
        Buffer.BlockCopy(bytes, 0, paddedBytes, paddedBytes.Length - pointLen, pointLen);

        return EcdhPublicKey.DeserializeBytes(paddedBytes, areBytesLittleEndian, curve);
    }

    private static ECParameters DeserializeBytes(byte[] bytes, bool areBytesLittleEndian, ECCurve curve)
    {
        if (bytes == null || bytes.Length < 1 || bytes[0] != EcdhPublicKey.PointIndicator)
        {
            throw new ArgumentException("Invalid encoded point format.");
        }

        int pointLen = EcdhPublicKey.EcCurvesPointLength[curve.Oid.Value!];
        byte[]? x = new byte[pointLen];
        byte[]? y = new byte[pointLen];
        byte[]? d = new byte[pointLen];

        int offset = 1;

        Buffer.BlockCopy(bytes, offset, x, 0, pointLen);
        if (bytes.Length > offset + pointLen)
            Buffer.BlockCopy(bytes, offset + pointLen, y, 0, pointLen);
        else
            y = null;
        if (bytes.Length > offset + 2 * pointLen)
            Buffer.BlockCopy(bytes, offset + (2 * pointLen), d, 0, pointLen);
        else
            d = null;

        ECParameters ecParameters = new()
        {
            Q = new ECPoint
            {
                X = x,
                Y = y
            },
            D = d,
            Curve = curve
        };

        //if (BitConverter.IsLittleEndian != areBytesLittleEndian)
        //{
        //    if (ecParameters.D != null)
        //        Array.Reverse(ecParameters.D);
        //    if (ecParameters.Q.X != null)
        //        Array.Reverse(ecParameters.Q.X);
        //    if (ecParameters.Q.Y != null)
        //        Array.Reverse(ecParameters.Q.Y);
        //}

        return ecParameters;
    }

    private static byte[] ExportToX509(byte[] publicKeyBytes, ECCurve curve)
    {
        // Define the algorithm identifier for ECDSA with the NIST P-256 curve
        Oid algorithmOid = new(EcdhPublicKey.EcdhPublicKeyOid); // id-ecPublicKey
        Oid curveOid = curve.Oid;// new("1.2.840.10045.3.1.7"); // secp256r1

        // Create the algorithm identifier sequence
        AsnWriter writer = new(AsnEncodingRules.DER);
        writer.PushSequence();
        writer.WriteObjectIdentifier(algorithmOid.Value!);
        writer.PushSequence();
        writer.WriteObjectIdentifier(curveOid.Value!);
        writer.PopSequence();
        writer.PopSequence();
        byte[] algorithmIdentifier = writer.Encode();

        // Create the SubjectPublicKeyInfo structure
        writer = new(AsnEncodingRules.DER);
        writer.PushSequence();
        writer.WriteEncodedValue(algorithmIdentifier);
        writer.WriteBitString(publicKeyBytes);
        writer.PopSequence();

        return writer.Encode();
    }

    private static bool TryExportToX509(byte[] publicKeyBytes, ECCurve curve, Span<byte> destination, out int bytesWritten)
    {
        // Define the algorithm identifier for ECDSA with the NIST P-256 curve
        Oid algorithmOid = new(EcdhPublicKey.EcdhPublicKeyOid); // id-ecPublicKey
        Oid curveOid = curve.Oid;// new Oid("1.2.840.10045.3.1.7"); // secp256r1

        // Create the algorithm identifier sequence
        AsnWriter writer = new(AsnEncodingRules.DER);
        writer.PushSequence();
        writer.WriteObjectIdentifier(algorithmOid.Value!);
        writer.PushSequence();
        writer.WriteObjectIdentifier(curveOid.Value!);
        writer.PopSequence();
        writer.PopSequence();
        byte[] algorithmIdentifier = writer.Encode();

        // Create the SubjectPublicKeyInfo structure
        writer = new(AsnEncodingRules.DER);
        writer.PushSequence();
        writer.WriteEncodedValue(algorithmIdentifier);
        writer.WriteBitString(publicKeyBytes);
        writer.PopSequence();

        byte[] encodedKey = writer.Encode();

        if (encodedKey.Length > destination.Length)
        {
            bytesWritten = 0;
            return false;
        }

        encodedKey.CopyTo(destination);
        bytesWritten = encodedKey.Length;
        return true;
    }

    private static byte[] ConvertX509PublicKeyToPointFormat(byte[] bytes)
    {
        AsnReader reader = new(bytes, AsnEncodingRules.DER);
        AsnReader sequenceReader = reader.ReadSequence();
        AsnReader algorithmIdentifierReader = sequenceReader.ReadSequence();
        algorithmIdentifierReader.ReadObjectIdentifier(); // Algorithm identifier
        algorithmIdentifierReader.ReadObjectIdentifier();
        //algorithmIdentifierReader.ReadNull(); // Parameters (NULL)
        byte[] rawPublicKey = sequenceReader.ReadBitString(out _);

        // Add the uncompressed point indicator (0x04)
        byte[] uncompressedPublicKey = new byte[rawPublicKey.Length];// + 1]
        //uncompressedPublicKey[0] = EcdhPublicKey.PointIndicator;
        Buffer.BlockCopy(rawPublicKey, 0, uncompressedPublicKey, 0, rawPublicKey.Length);

        return uncompressedPublicKey;
    }
}
