using System;
using Fragment.NetSlum.Core.Buffers;
using Fragment.NetSlum.Core.Extensions;
using Fragment.NetSlum.Networking.Constants;
using Fragment.NetSlum.Networking.Objects;
using OpCodes = Fragment.NetSlum.Networking.Constants.OpCodes;
using Result = Fragment.NetSlum.Networking.Constants.Result;

namespace Fragment.NetSlum.Networking.Packets.Response.HomeLand
{
    public class AccountInfoResponse : BaseResponse
    {
        private const byte ACCOUNT_INFO_SUCCESS = 0x17;
        
        private int _accountId = 0;
        
        public AccountInfoResponse SetAccountId(int id)
        {
            _accountId = id;
            return this;
        }

        private byte[]? _accountKey;

        public AccountInfoResponse SetAccountKey(byte[] key)
        {
            if (key.Length != 16) { key = new byte[16]; }
            _accountKey = key;
            return this;
        }

        private bool _isTestDisc;
        private bool _isOverseas;
        
        public AccountInfoResponse SetGameVersion(byte gameVersion)
        {
            _isTestDisc = (gameVersion == 2 || gameVersion == 3);
            _isOverseas = (gameVersion == 3 || gameVersion == 7);
            return this;
        }

        //NOTE: most of the time we use bytes for the Responses, but here it's
        //easier with the big switch later to just pass the enum directly.
        private Result _result;

        public AccountInfoResponse SetResult(Result result)
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
            Result result = Result.Ok;
            
            byte heartbeatMode = 0xFF;
            byte msgType = 0x02;
            
            string defaultMessage = "Welcome to the HomeLand matching server beta test!\nRemember to report bugs :)";
            if (!_isOverseas) { defaultMessage = "ホームランド非公式マッチングサーバー(βテスト版)へようこそ！\nバグ報告にご協力いただけると、システム改善につながります。"; }
            string msg1 = "";
            string msg2 = "";
            
            switch(_result)
            {
                // msg2 is ignored
                case Result.Ok:
                    result = Result.Ok;
                  
                    msg1 = defaultMessage;
                    if(_plaintextPassword != null)
                    {
                        if (_isOverseas) { msg1 = $"Welcome! New account registered.\nID: {_accountId} | Password: {_plaintextPassword}"; }
                        else { msg1 = $"ようこそ！登録完了です。\nＩＤ：{_accountId} | パスワード：{_plaintextPassword}"; }
                    }
                    break;
                
                // reverse message order
                case Result.AcctInfoSuccess:
                    result = (_isTestDisc ? Result.Ok : Result.AcctInfoSuccess);

                    if (_isOverseas) { msg1 = "Login successful."; }
                    else { msg1 = "ログイン成功"; }
                    msg2 = defaultMessage;
                    break;

                case Result.AcctInfoError:
                    result = (_isTestDisc ? Result.Fail : Result.AcctInfoError);

                    if (_isOverseas) { msg1 = "Account information error.\nPlease try again later."; }
                    else { msg1 = "アカウント情報エラーが発生しました。\nしばらくしてから再度お試しください。"; }
                    msg2 = "*";
                    break;

                default:
                    result = _result;
                    break;
            }
            
            byte[] msg1Bytes = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(msg1 + "\0");
            byte[] msg2Bytes = System.Text.Encoding.GetEncoding("shift_jis").GetBytes(msg2 + "\0");
            var writer = new MemoryWriter(40 + msg1Bytes.Length + msg2Bytes.Length);

            writer.Write((byte)result);
            writer.Write(_accountId);
            writer.Write(_accountKey.AsSpan());
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
