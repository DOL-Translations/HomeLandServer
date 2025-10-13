using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fragment.NetSlum.Networking.Attributes;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using Fragment.NetSlum.Networking.Packets.Response.HomeLand;
using Fragment.NetSlum.Networking.Sessions;
using Fragment.NetSlum.Persistence;
using Fragment.NetSlum.Persistence.Entities;
using Fragment.NetSlum.Core.CommandBus;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.AccountInfo)]
public class AccountInfoRequest : BaseRequest
{
    private readonly FragmentContext _database;
    private readonly ICommandBus _commandBus;

    public AccountInfoRequest(FragmentContext database, ICommandBus commandBus)
    {
        _database = database;
        _commandBus = commandBus;
    }

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        /*Request_AccountInfo //OPCODE_ACCOUNT_INFO
        {
	        uint AccountID;
	        byte UnknownBytes[16];
	        ClientType GameMode; //Maybe?
	        byte Unk2; //Either 6 or 2, depending on some variable.
        }*/

        var saveId = System.BitConverter.ToUInt32(request.Data.Span.Slice(0, 4));

        var accountId = saveId; //_database.PlayerAccounts.FirstOrDefault(p => p.SaveId == saveId)?.Id;

        //todo: actually register new acct for id 0

        if (accountId == 0)
        {
            accountId = 1;
        }

        /*if (accountId == null)
        {
            accountId = 0;  //await _commandBus.Execute(new RegisterPlayerAccountCommand(saveId));
        }*/

        session.PlayerAccountId = (int)accountId;//.Value;

        var responses = new List<FragmentMessage>
        {
            new AccountInfoResponse().SetAccountId((int)accountId/*.Value*/).Build(),
            new EchoResponse().Build(),
            new UrgentAnnouncementResponse().Build(),
        };

        return new ValueTask<ICollection<FragmentMessage>>(responses);
    }
}
