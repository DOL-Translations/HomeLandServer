using System.Collections.Generic;
using System.Threading.Tasks;
using Fragment.NetSlum.Networking.Attributes;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using Fragment.NetSlum.Networking.Packets.Response.HomeLand;
using Fragment.NetSlum.Networking.Sessions;
using Fragment.NetSlum.Persistence;
using Fragment.NetSlum.Persistence.Entities;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.HomeLandUpdateUserCnt)]
public class HomeLandUpdateUserCntRequest : BaseRequest
{
    private readonly FragmentContext _database;

    public HomeLandUpdateUserCntRequest(FragmentContext database)
    {
        _database = database;
    }

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        /*Request_HomelandUserCount //OPCODE_HOMELAND_USER_COUNT
{
	        byte RegisteredPlayerCount;
	        byte MaxPlayerCount;
        }*/

        byte registeredPlayerCount = request.Data.Span[0];
        byte maxPlayerCount = request.Data.Span[1];

        session.HomeLand!.RegisteredPlayerCnt = registeredPlayerCount;
        session.HomeLand!.MaxPlayerCnt = maxPlayerCount;
        _database.SaveChanges();
        return SingleMessage(new HomeLandUpdateUserCntResponse().Build());
    }
}
