using System;
using System.Linq;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Networking.Attributes;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using Fragment.NetSlum.Networking.Packets.Response.HomeLand;
using Fragment.NetSlum.Networking.Sessions;
using Fragment.NetSlum.Persistence;
using Fragment.NetSlum.Persistence.Entities;
using Fragment.NetSlum.Core.CommandBus;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;
using BCrypt.Net;

namespace Fragment.NetSlum.Networking.Packets.Request.HomeLand;

[FragmentPacket(MessageType.Data, OpCodes.AccountInfo)]
public class AccountInfoRequest : BaseRequest
{
    private const byte RESULT_OK = 0x00;
    private const byte ACCOUNT_INFO_SUCCESS = 0x17;
    
    private readonly FragmentContext _database;
    private readonly ICommandBus _commandBus;

    public AccountInfoRequest(FragmentContext database, ICommandBus commandBus)
    {
        _database = database;
        _commandBus = commandBus;
    }
    
    private static string GeneratePassword(int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
    }

    public override ValueTask<ICollection<FragmentMessage>> GetResponse(FragmentTcpSession session, FragmentMessage request)
    {
        /*Request_AccountInfo //OPCODE_ACCOUNT_INFO
        {
          uint AccountID;
          byte UnknownBytes[16];
          byte ClientType;
          byte GameVersion;
        }*/

        var reader = new SpanReader(request.Data.Span);

        uint accountIdSupplied = reader.ReadUInt32();
        byte[] unknown16Bytes  = reader.Read(16).ToArray();
        byte clientType        = reader.ReadByte();
        byte gameVersion       = reader.ReadByte();

        string unknown16 = BitConverter.ToString(unknown16Bytes).Replace("-", "");

        var account = _database.PlayerAccounts.FirstOrDefault(p => p.Id == accountIdSupplied);

        if(account != null)
        {
            account.LastLogin  = DateTime.UtcNow;
            account.ClientType = clientType;
            _database.SaveChanges();

            session.PlayerAccountId = account.Id;
            session.IsTestDisc      = (gameVersion == 2);

            byte result_found = ACCOUNT_INFO_SUCCESS;

            var response_found = new List<FragmentMessage>
            {
                new AccountInfoResponse()
                    .SetAccountId(account.Id)
                    .SetGameVersion(session.IsTestDisc)
                    .SetResult(result_found)
                    .Build(),
                
                new EchoResponse().Build(),
            };

            return new ValueTask<ICollection<FragmentMessage>>(response_found);
        }

        // Add the new account to the database
        var password = GeneratePassword();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        int tempSaveId = 0;

        var newAccount = new PlayerAccount
        {
            Id = 0,
            SaveId = tempSaveId.ToString(),
            CreatedAt = DateTime.UtcNow,
            Unk1_16 = unknown16,
            ClientType = clientType,
            GameVersion = gameVersion,
            PasswordHash = hashedPassword,
        };
        _database.PlayerAccounts.Add(newAccount);
        _database.SaveChanges();

        session.PlayerAccountId = newAccount.Id;
        session.IsTestDisc      = (gameVersion == 2);

        byte result_created = RESULT_OK;

        var response_created = new List<FragmentMessage>
        {
            new AccountInfoResponse()
                .SetAccountId(newAccount.Id)
                .SetGameVersion(session.IsTestDisc)
                .SetResult(result_created)
                .SetPlaintextPassword(password)
                .Build(),
        };

        return new ValueTask<ICollection<FragmentMessage>>(response_created);
    }
}
