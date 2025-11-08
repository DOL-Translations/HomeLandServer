namespace Fragment.NetSlum.Networking.Constants;

public enum Result : byte
{
    Ok = 0x00,
    Fail = 0x01,
    Maintenance = 0x02,

    AcctNotFound = 0x03,
    AcctMatchingServerFull = 0x04,
    AcctMatchingServerNotStarted = 0x06,

    PublishFailed = 0x07,
    PublishFailedFirewall = 0x08,
    PublishFailed2 = 0x09,
    PublishFailedWaitOneHour = 0x0A,

    FirewallCheckFailed = 0x0B,

    JoinSuspendedHomeLand = 0x0C,
    JoinRemovedHomeLand = 0x0D,

    UpdatedUnpublishedHomeLand = 0x0E,
    UpdatedUnpublishedLobby = 0x0F,

    DeleteUnpublishedHomeLand = 0x10,
    DeletePublishedHomeLand = 0x11,

    MatchingServerNotFound = 0x12,

    AcctInfoError = 0x15,
    BetaTestNotStarted = 0x16,
    AcctInfoSuccess = 0x17,
}
