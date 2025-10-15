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

[FragmentPacket(MessageType.Data, OpCodes.HomeLandUpdateUserCnt2)]
public class HomeLandUpdateUserCnt2Request : BaseRequest
{
    private readonly FragmentContext _database;

    public HomeLandUpdateUserCnt2Request(FragmentContext database)
    {
        _database = database;
    }

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        /*Request_200F //OPCODE_200F, Not yet seen; update server info for homeland
        {
	        uint Unk;
	        byte RegisteredPlayerCount;
	        byte MaxPlayerCount;
        }*/

        uint unk = System.BitConverter.ToUInt32(request.Data.Span.Slice(0, 4));
        byte registeredPlayerCount = request.Data.Span[4];
        byte maxPlayerCount = request.Data.Span[5];

        session.HomeLand!.RegisteredPlayerCnt = (sbyte)registeredPlayerCount;
        session.HomeLand!.MaxPlayerCnt = (sbyte)maxPlayerCount;
        _database.SaveChanges();
        return SingleMessage(new HomeLandUpdateUserCnt2Response().Build());
    }
}
