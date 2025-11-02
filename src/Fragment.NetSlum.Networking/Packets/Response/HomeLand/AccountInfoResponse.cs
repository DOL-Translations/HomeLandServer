using System;
using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class AccountInfoResponse : BaseResponse
    {
        private int _accountId = 1; //placeholder
				
				private const byte RESULT_OK = 0x00;
				private const byte RESULT_FAIL = 0x01;
				private const byte RESULT_ACCOUNT_FOUND = 0x17;
				
        public AccountInfoResponse SetAccountId(int id)
        {
            _accountId = id;
            return this;
        }
				
				private byte _result;
				
				public AccountInfoResponse SetResult(byte result)
				{
						_result = result;
						return this;
				}
				
				private string? _plaintextPassword;
				
				public AccountInfoResponse SetPlaintextPassword(string password)
				{
						_plaintextPassword = password;
						return this;
				}

        public override FragmentMessage Build()
        {
            byte result = 0x00; // 0x00 OK
            
            //byte[] unk1 = new byte[16]; //Unknown
            int test1 = 0x00;
            int test2 = 0x00;
            int test3 = 0x00;
            int test4 = 0x00;

            byte heartbeatMode = 0x00; // 0x00 00020002 keep alive
            byte msgType = 0x02;
						
						
						string defaultMessage = "Welcome to the HomeLand matching server beta test!\nRemember to report bugs :)";
						string msg1 = "";
						string msg2 = "";
						
						switch(_result)
						{
								// msg2 is ignored
								case RESULT_OK:
									result = RESULT_OK;
									
									msg1 = defaultMessage;
									if(_plaintextPassword != null)
									{
										msg1 = $"Welcome! New account registered.\nID: {_accountId} | Password: {_plaintextPassword}";
									}
									
									msg2 = "";
									break;
								
								// reverse message order
								case RESULT_ACCOUNT_FOUND:
									result = RESULT_ACCOUNT_FOUND;
									
									msg1 = "Login successful.";
									msg2 = defaultMessage;
									break;
								
								default:
									result = RESULT_FAIL; // error code 21049
									
									msg1 = $"Unknown result code: 0x{result:X2}. Please forward this information.";
									msg2 = "*";
									break;
						}
						
						
						byte[] msg1Bytes = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(msg1 + "\0");
						byte[] msg2Bytes = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(msg2 + "\0");
            var writer = new MemoryWriter(40 + msg1Bytes.Length + msg2Bytes.Length);
            //var writer = new MemoryWriter(40);

            writer.Write(result);
            writer.Write(_accountId);
            //writer.Write(unk1.AsMemory());
            writer.Write(test1);
            writer.Write(test2);
            writer.Write(test3);
            writer.Write(test4);
            writer.Write(heartbeatMode);
            writer.Write(msgType);
						
						writer.Write(msg1Bytes.Length);
            writer.Write(msg1Bytes.AsSpan());
						
						writer.Write(msg2Bytes.Length);
            writer.Write(msg2Bytes.AsSpan());


            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.AccountInfo,
                Data = writer.Buffer,
            };
        }
    }
}
