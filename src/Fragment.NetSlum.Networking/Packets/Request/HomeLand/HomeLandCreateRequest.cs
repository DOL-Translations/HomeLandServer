using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Attributes;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using Fragment.NetSlum.Networking.Packets.Response.HomeLand;
using Fragment.NetSlum.Networking.Sessions;
using Fragment.NetSlum.Persistence;
using Fragment.NetSlum.Persistence.Entities;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.HomeLandCreate)]
public class HomeLandCreateRequest : BaseRequest
{
    private readonly FragmentContext _database;
    
    public HomeLandCreateRequest(FragmentContext database)
    {
        _database = database;
    }

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        /*
        Request_HomelandInfo //OPCODE_HOMELAND_INFO
        {
	        byte Unk1;
	        uint LocalIP;
	        char name[]; //null terminated string
	        ushort Location;
	        byte Time;
	        char Password[]; //null terminated string
	        char Comment[]; //null terminated string
	        byte RegisteredPlayerCount;
	        uint32_t ClearCount; //? This looks like what the list says
	        byte Unk2; //
	        byte Unk2; //Either 6 or 2, depending on some variable.
        }
        */
        var counter = 0;

        byte unk1 = request.Data.Span[counter++];
        var localIp = System.BitConverter.ToUInt32(request.Data.Span.Slice(counter, 4));
        counter += 4;
        var name = request.Data.Span.Slice(counter).ToShiftJisString();
        counter += name.Length + 1;
        var location = System.BitConverter.ToUInt16(request.Data.Span.Slice(counter, 2));
        counter += 2;
        byte time = request.Data.Span[counter++];
        var password = request.Data.Span.Slice(counter).ToShiftJisString();
        counter += password.Length + 1;
        var comment = request.Data.Span.Slice(counter).ToShiftJisString();
        counter += comment.Length + 1;
        byte registeredPlayerCount = request.Data.Span[counter++];
        var clearCount = System.BitConverter.ToUInt32(request.Data.Span.Slice(counter, 4));
        counter += 4;
        byte unk2 = request.Data.Span[counter++];
        byte unk3 = request.Data.Span[counter++];

        //build a new homeland in the database

        //search for existing homelands with this name
        var homeland = _database.HomeLands.FirstOrDefault(h => h.HomeLandName == name);
        if (homeland != null)
        {
            //homeland.Name = name;
            homeland.Location = location;
            homeland.Time = time;
            homeland.Password = password;
            homeland.Comment = comment;
            homeland.RegisteredPlayerCnt = (sbyte)registeredPlayerCount;
            homeland.ClearCnt = clearCount;
        }
        else
        {
            homeland = new HomeLandEntity
            {
                LocalIp = localIp,
                HomeLandName = name,
                Location = location,
                Time = time,
                Password = password,
                Comment = comment,
                RegisteredPlayerCnt = (sbyte)registeredPlayerCount,
                ClearCnt = clearCount,
            };
            _database.HomeLands.Add(homeland);
        }

        session.HomeLand = homeland;

        var responses = new List<FragmentMessage>
        {
            new HomeLandCreateResponse().Build(),
        };

        return new ValueTask<ICollection<FragmentMessage>>(responses);
    }
}
