using System;
using System.Linq;
using System.Buffers.Binary;
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
	        ClientType GameMode; //Maybe?
	        byte Unk2; //Either 6 or 2, depending on some variable.
        }*/

				var span = request.Data.Span;
				
				int accountIdSupplied = BinaryPrimitives.ReadInt32BigEndian(span.Slice(0, 4));
				Console.WriteLine($"Looking for ID: {accountIdSupplied}");
				
				var unknown16span = span.Slice(4, 16);
				string unknown16 = BitConverter.ToString(unknown16span.ToArray()).Replace("-", "");
				
				byte unknown1 = span[20];
				byte unknown2 = span[21];

				//var accountId = _database.PlayerAccounts.FirstOrDefault(p => p.Id == accountIdSupplied)?.Id ?? 0;
				var account = _database.PlayerAccounts.FirstOrDefault(p => p.Id == accountIdSupplied);

				if(account != null)
				{
						account.LastLogin = DateTime.UtcNow;
						_database.SaveChanges();
						
						session.PlayerAccountId = account.Id;
						byte result_found = 0x17;

						var response_found = new List<FragmentMessage>
						{
								new AccountInfoResponse().SetAccountId(account.Id).SetResult(result_found).Build(),
								new EchoResponse().Build(),
								//new UrgentAnnouncementResponse().Build(),
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
						Unk2_1 = unknown1,
						Unk3_1 = unknown2,
						PasswordHash = hashedPassword,
				};
				_database.PlayerAccounts.Add(newAccount);
				_database.SaveChanges();
				
				
        session.PlayerAccountId = newAccount.Id;
				byte result_created = 0x00; // OK

				var response_created = new List<FragmentMessage>
				{
						new AccountInfoResponse().SetAccountId(newAccount.Id).SetResult(result_created).SetPlaintextPassword(password).Build(),
						new EchoResponse().Build(),
						//new UrgentAnnouncementResponse().Build(),
				};

        return new ValueTask<ICollection<FragmentMessage>>(response_created);

				////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //var saveId = System.BitConverter.ToInt32(request.Data.Span.Slice(0, 4));

        //var accountId = saveId; //_database.PlayerAccounts.FirstOrDefault(p => p.SaveId == saveId)?.Id;

        //todo: actually register new acct for id 0

        /*if (accountId == null || accountId < 2)
        {
            accountId = 0;  //await _commandBus.Execute(new RegisterPlayerAccountCommand(saveId));
        }*/

        //generate a new account id if given id is < 2
        //new id should be between 2 and 10, and be unique between _database.PlayerAccounts.Id and _database.HomeLands.HomeLandId
        /*if (accountId < 2)
        {
            var existingAccountIds = _database.PlayerAccounts.Select(p => p.Id).ToHashSet();
            var existingHomeLandIds = _database.HomeLands.Select(h => h.HomeLandId).ToHashSet();
            var existingIds = existingAccountIds.Union(existingHomeLandIds.Select(x => (int)x)).ToHashSet();
            var newId = 2;
            while (existingIds.Contains(newId) && newId <= 10)
            {
                newId++;
            }
            if (newId <= 10)
            {
                accountId = newId;
                // Add the new account to the database
                var newAccount = new PlayerAccount
                {
                    Id = accountId,
                    SaveId = accountId.ToString(),
                    CreatedAt = DateTime.UtcNow
                };
                _database.PlayerAccounts.Add(newAccount);
                _database.SaveChanges();
            }
        } */

        /*session.PlayerAccountId = accountId;//.Value;

        var responses = new List<FragmentMessage>
        {
            new AccountInfoResponse().SetAccountId(accountId/*.Value* /).Build(),
            new EchoResponse().Build(),
            new UrgentAnnouncementResponse().Build(),
        };

        return new ValueTask<ICollection<FragmentMessage>>(responses); */
    }
}
