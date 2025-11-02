using System;
using System.Linq;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fragment.NetSlum.Core.Buffers;
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
	        byte Unk2; // -> probably 0x01 as it is a game server
	        byte Unk2; //Either 6 or 2, depending on some variable. -> probably 
        }
        */

        var reader = new SpanReader(request.Data.Span);

        byte unk1										= reader.ReadByte();
        uint localIp								= reader.ReadUInt32();
        string name									= reader.ReadString(out _).ToShiftJisString();
        ushort location							= reader.ReadUInt16();
        byte time										= reader.ReadByte();
        string password							= reader.ReadString(out _).ToShiftJisString();
        string comment							= reader.ReadString(out _).ToShiftJisString();
        byte registeredPlayerCount	= reader.ReadByte();
        uint clearCount							= reader.ReadUInt32();
        byte unk2										= reader.ReadByte();
        byte unk3										= reader.ReadByte();
				
				Console.WriteLine($"IP_CREATE_REQUEST  : {localIp}");



        if (session.IsOverseas && location != (ushort)5000 && location <= (ushort)8)
        {
            //Before: Africa, Antarctica, Asia, Europe, Middle East, North America, Oceania, South America, Other
            //After: Asia, Africa, Oceania, North America, Middle East, Antarctica, South America, Europe, Other
            ushort[] locationMapping = new ushort[] { 1, 5, 0, 7, 4, 3, 2, 6, 8 };
            location = (ushort)(locationMapping[location] + 2933);
        }

        //search for existing homelands with this id
        var homeland = _database.HomeLands.FirstOrDefault(h => h.PlayerAccountId == session.PlayerAccountId);
        if (homeland != null)
        {
            //homeland.Name = name;
						//homeland.PlayerAccountId = session.PlayerAccountId;
            homeland.LocalIp = localIp;
            homeland.HomeLandName = name;
            homeland.Location = location;
            homeland.Time = time;
            homeland.Password = password;
            homeland.Comment = comment;
            homeland.RegisteredPlayerCnt = registeredPlayerCount;
            homeland.ClearCnt = clearCount;
            homeland.Unk2_1 = unk2;
            homeland.Unk3_1 = unk3;
        }
        else
        {
            homeland = new HomeLandEntity
            {
                HomeLandId = 0, //use account id as homeland id
								PlayerAccountId = session.PlayerAccountId,
                LocalIp = localIp,
                HomeLandName = name,
                Location = location,
                Time = time,
                Password = password,
                Comment = comment,
                RegisteredPlayerCnt = registeredPlayerCount,
                ClearCnt = clearCount,
								Unk2_1 = unk2,
								Unk3_1 = unk3,
            };
            _database.HomeLands.Add(homeland);
        }
        _database.SaveChanges();
        
        session.HomeLand = homeland;

        var responses = new List<FragmentMessage>
        {
            new HomeLandCreateResponse().Build(),
        };

        return new ValueTask<ICollection<FragmentMessage>>(responses);
    }
}
