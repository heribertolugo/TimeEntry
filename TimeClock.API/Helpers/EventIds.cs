namespace TimeClock.Api.Helpers;

public static class EventIds
{
    public static readonly EventId UnknownException = new EventId(0, nameof(UnknownException));
    public static readonly EventId CreateRsaKeys = new EventId(1, nameof(CreateRsaKeys));
    public static readonly EventId CreateDevice = new EventId(2, nameof(CreateDevice));
    public static readonly EventId ParseEntityId = new EventId(3, nameof(ParseEntityId));
    public static readonly EventId RsaKeyAlreadyExists = new EventId(4, nameof(RsaKeyAlreadyExists));
    public static readonly EventId DeviceNameInvalid = new EventId(4, nameof(DeviceNameInvalid));
    public static readonly EventId Jwt = new EventId(5, nameof(Jwt));
    public static readonly EventId GetDevice = new EventId(6, nameof(GetDevice));
    public static readonly EventId CryptoTryGet = new EventId(7, nameof(CryptoTryGet));
    public static readonly EventId JsonDeserialize = new EventId(8, nameof(JsonDeserialize));
    public static readonly EventId User = new EventId(9, nameof(User));
    public static readonly EventId ActionCancelled = new EventId(10, nameof(ActionCancelled));
    public static readonly EventId PunchEntry = new EventId(11, nameof(PunchEntry));
    public static readonly EventId IncompleteOrBadData = new EventId(12, nameof(IncompleteOrBadData));
    public static readonly EventId EntityNotFound = new EventId(13, nameof(EntityNotFound));
}
