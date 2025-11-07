using System;
using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class LoginReadyResponse : BaseResponse
    {
        private byte _result = 0;

        public LoginReadyResponse SetResult(byte result)
        {
            _result = result;
            return this;
        }

        private int _accountId = 0;

        public LoginReadyResponse SetAccountId(int id)
        {
            _accountId = id;
            return this;
        }

        private uint _homelandId = 0;

        public LoginReadyResponse SetHomelandId(uint homelandId)
        {
            _homelandId = homelandId;
            return this;
        }
        
        public override FragmentMessage Build()
        {
            /* Request_2011 //OPCODE_2011
            {
              uint Value0; -> account id?
              byte Value1; -> character slot?
              uint Value2; -> timestamp?
              byte Value3; -> client type 00 server, 01 client ?
              byte Value4; -> version 02 test disc, 06 retail ?
            }*/
            
            /* uint value0 = _homelandId;
            byte value1 = _result;
            uint value2 = (uint) DateTimeOffset.UtcNow.ToUnixTimeSeconds();;
            byte value3 = 0x00;
            byte value4 = 0x06; */
            
            uint value0 = (uint) _accountId;
            byte value1 = 0x00;
            uint value2 = _homelandId;
            byte value3 = 0x00;
            byte value4 = 0x06;
            
            var writer = new MemoryWriter(1);
            writer.Write((byte) 0x01);
            /* writer.Write(value0);
            writer.Write(value1);
            writer.Write(value2);
            writer.Write(value3);
            writer.Write(value4); */
            
            return new FragmentMessage
            {
                MessageType = MessageType.Data,
                DataPacketType = OpCodes.LoginReady,
                Data = writer.Buffer,
            };
        }
    }
}