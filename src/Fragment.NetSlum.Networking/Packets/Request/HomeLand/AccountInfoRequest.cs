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
using Result = Fragment.NetSlum.Networking.Constants.Result;
using BCrypt.Net;

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
        byte[] accountKeyBytes  = reader.Read(16).ToArray();
        byte clientType        = reader.ReadByte();
        byte gameVersion       = reader.ReadByte();

        session.IsTestDisc = (gameVersion == 2 || gameVersion == 3);
        session.IsOverseas = (gameVersion == 3 || gameVersion == 7);

        string accountKey = BitConverter.ToString(accountKeyBytes).Replace("-", "");

        Result result = Result.Ok;

        //compatibility - remove from release!
        //swap endianness
        /*if (accountIdSupplied > 10 || accountIdSupplied < 2)
        {
            accountIdSupplied = BinaryPrimitives.ReverseEndianness(accountIdSupplied);
        }*/

        PlayerAccount account = null;
        if (accountIdSupplied >= 2)
        {
            account = _database.PlayerAccounts.FirstOrDefault(p => p.Id == accountIdSupplied); /*&& p.Unk1_16 == accountKey);*/
        }

        //todo: if accountKey is 00 generate a new one

        if (account != null)
        {
            result = Result.AcctInfoSuccess;
            account.LastLogin  = DateTime.UtcNow;
            account.ClientType = clientType;
            try { _database.SaveChanges(); } catch { result = Result.Fail; }

            session.PlayerAccountId = account.Id;
            
            var response_found = new List<FragmentMessage>
            {
                new AccountInfoResponse()
                    .SetAccountId(account.Id)
                    .SetAccountKey(accountKeyBytes)
                    .SetGameVersion(gameVersion)
                    .SetResult(result)
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
            Id = accountIdSupplied >= 2 ? (int)accountIdSupplied : 0,
            SaveId = tempSaveId.ToString(),
            CreatedAt = DateTime.UtcNow,
            Unk1_16 = accountKey,
            ClientType = clientType,
            GameVersion = gameVersion,
            PasswordHash = hashedPassword,
        };
        _database.PlayerAccounts.Add(newAccount);
        try { _database.SaveChanges(); } catch { result = Result.Fail; }

        session.PlayerAccountId = newAccount.Id;
        
        var response_created = new List<FragmentMessage>
        {
            new AccountInfoResponse()
                .SetAccountId(newAccount.Id)
                .SetAccountKey(accountKeyBytes)
                .SetGameVersion(gameVersion)
                .SetResult(result)
                .SetPlaintextPassword(password)
                .Build(),

            new EchoResponse().Build(),
        };

        return new ValueTask<ICollection<FragmentMessage>>(response_created);
    }
}
