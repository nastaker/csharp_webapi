/*
  作者：李大熊（602365214）
*/
using System;
using System.Text;

namespace ilabHelper
{
    public class XJWT
    {
        private static byte DOT = (byte)'.';
        private static int SIG_LENGTH = 32;
        private static int BASE64_SIG_LENGTH = 44;
        private static int AES_KEY_LENGTH = 44;//ase密码的长度
        private static int EXPIRES_OFFSET = 0;
        private static int TYPE_OFFSET = EXPIRES_OFFSET + 8;//header 中的类型
        private static int ISSUEID_OFFSET = TYPE_OFFSET + 1;
        private static int HEADER_LENGTH = ISSUEID_OFFSET + 8;

        private byte[] secret_key;//自定义
        private byte[] aesKey;
        private byte[] keySpec;
        private long issueId;
        private Random rand;
        private byte[] sig = new byte[SIG_LENGTH];

        private ByteBuffer header = ByteBuffer.Allocate(HEADER_LENGTH);
        private ByteBuffer encrypted = ByteBuffer.Allocate(1024 * 64);

        public enum Type : byte
        {
            RESERVED = 0,
            JSON = 1,
            SYS = 2,
        }

        public static bool isValidType(byte t)
        {
            switch ((Type)t)//此处修改过
            {
                case Type.RESERVED:
                case Type.JSON:
                case Type.SYS:
                    return true;
                default:
                    return false;
            }
        }

        //secret、aesKey和issueId由ilab-x.com提供
        public XJWT(string secret, string aesKey, long issueId) : this(secret, aesKey, DateTime.Now.Second, issueId) { }
        public XJWT(string secret, string aesKey, long seed, long issueId)
        {
            try
            {
                if (aesKey.Length != AES_KEY_LENGTH) { throw new Exception("Aes key length must be 44"); }
                secret_key = Encoding.UTF8.GetBytes(secret);
                this.aesKey = Utils.Base64_Decode(Encoding.UTF8, aesKey);
                byte[] iv = new byte[16];
                for (int i = 0; i < iv.Length; i++) { iv[i] = this.aesKey[i]; }
                keySpec = this.aesKey;

                rand = new Random((int)seed);
                this.issueId = issueId;
            }
            catch (Exception e) { throw e; }
        }

        private int absolutePosition(ByteBuffer bb)
        {
            return (int)bb.Position + 0;//java中 bytebuffer 中的bb.arrayOffset()，是相对于最开始的长度，这里都是从0开始
        }

        public ByteBuffer sign(byte type, ByteBuffer payload, ByteBuffer outbyte, long expires)
        {
            outbyte.Clear();
            int pos = absolutePosition(outbyte);
            //设置头部信息
            header.Clear();
            byte[] expires_byte = BitConverter.GetBytes(expires);
            Array.Reverse(expires_byte);
            //【注意：】C# 和java的byte顺序相反，ilab-x使用java开发的，Java中的byte[]｛1，2，3，4，5，6，7，8｝，C#中对应的为｛8，7，6，5，4，3，2，1｝
            header.Put(expires_byte);

            header.Put(type);

            byte[] issueId_byte = BitConverter.GetBytes(issueId);
            Array.Reverse(issueId_byte);//C# 和java的byte顺序相反，原因同上
            header.Put(issueId_byte);

            outbyte.Put(Utils.Base64_Encode_Java(header.ToArray()));

            outbyte.Put(DOT);
            //设置payload信息
            outbyte.Put(Utils.Base64_Encode(Encoding.UTF8, payload.ToArray()));

            string tempsig = new string(Encoding.UTF8.GetChars(outbyte.ToArray()));
            outbyte.Put(DOT);
            //设置signature信息
            byte[] sig256;
            try { sig256 = Utils.SHA256_Hmac(tempsig, secret_key); }
            catch (Exception e) { throw e; }

            outbyte.Put(Utils.Base64_Encode_Java(sig256));
            outbyte.Flip();
            return outbyte;
        }

        public ByteBuffer encryptAndSign(byte type, ByteBuffer payload, ByteBuffer outbyte, long expires)
        {
            if (!isValidType(type)) { throw new Exception("Unknown type:" + type); }
            outbyte.Clear();
            int pos = (int)outbyte.Position;
            outbyte.PutLong(rand.Next());   //前8位随机数
            outbyte.Put(payload);           //填充实际数据
            //补全后面的字节
            byte padding = (byte)((16 - ((8 + payload.ToArray().Length + 1) & 0xF)) & 0xF);
            for (int i = 0; i < padding + 1; ++i) { outbyte.Put(padding); }

            try
            {
                encrypted.Clear();//先清空，然后添加
                encrypted.Put(Utils.AES_Encrypt(outbyte.ToArray(), aesKey));
                encrypted.Flip();

                int len = encrypted.ToArray().Length;
                encrypted.Limit = len;
                outbyte.Position = pos;

                return sign(type, encrypted, outbyte, expires);

            }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// 验证sign和header
        /// </summary>
        /// <param name="data">加密的字符串数据</param>
        /// <param name="outbyte">输出ByteBuffer对象，传一个新的对象就行</param>
        /// <param name="now">当前时间</param>
        /// <returns></returns>
        public byte verify(string data, ByteBuffer outbyte, long now)
        {
            if (data == null) { throw new Exception("Input data is null——输入数据为空"); }

            int i = data.Length - BASE64_SIG_LENGTH - 1;
            if (i < 0 || data[i] != DOT) { throw new Exception("Invalid token——错误的令牌"); }

            byte[] da = Encoding.UTF8.GetBytes(data);

            //进行 signature验证（用Hmac解密）
            string datemp = new string(Encoding.UTF8.GetChars(da), 0, i);
            try
            {
                string tmp = Utils.SHA256_Hmac(datemp, Encoding.UTF8.GetString(secret_key));
                sig = Encoding.UTF8.GetBytes(tmp);
            }
            catch (Exception e) { throw e; }

            //demo中最后一部分的sign部分没有用base64加密，所以此处直接使用，以后需要根据实际情况测试
            string _sig = data.Substring(data.LastIndexOf((char)DOT) + 1);
            for (int j = 0; j < _sig.Length; ++j)
            {
                if (_sig[j] != sig[j]) { throw new Exception("Invalid token——令牌错误"); }
            }

            //进行令牌时间验证
            int s = data.IndexOf((char)DOT);
            byte[] header = Utils.Base64_Decode(Encoding.UTF8, data.Substring(0, s));
            ByteBuffer bb = ByteBuffer.Wrap(header);

            byte[] tmpb = bb.ReadBytes(8);  //前8位是令牌过期时间
            Array.Reverse(tmpb);            //【注意】C# 中和Java中顺序相反，原因参见102行
            long expires = BitConverter.ToInt64(tmpb, 0);
            //验证令牌时间是否过期
            if (expires < now) { throw new Exception("Invalid token, expired——令牌过期"); }

            outbyte.Clear();
            int sl = data.LastIndexOf((char)DOT);
            string paytemp = data.Substring(s + 1, sl - s - 1);
            byte[] payload = Utils.Base64_Decode(Encoding.UTF8, paytemp);
            outbyte.Put(payload);
            outbyte.Flip();

            byte type = header[TYPE_OFFSET];
            if (!isValidType(type)) { throw new Exception("Invalid token, unknown type:" + type); }//2是Json

            return type;
        }

        //验证和解密签名数据
        public byte verifyAndDecrypt(string data, ByteBuffer outbyte, long now)
        {
            byte type = verify(data, encrypted, now);//进行sign和header的验证
            outbyte.Clear();
            try
            {
                //对payload进行aes解码
                byte[] payload = encrypted.ToArray();
                byte[] destr = Utils.AES_Decrypt(payload, keySpec);
                outbyte.Put(destr);
                outbyte.Flip();

                int len;
                len = destr.Length;
                len -= 1 + destr[len - 1];  //去除掉前面的8个字节和最后的随机数节

                if (len < 0 || len > outbyte.Remaining) { throw new Exception("Incorrect AES key or outout buffer too small"); }

                //todo::这个范围需要确定一下
                outbyte.Position = 8;
                outbyte.Limit = destr.Length - 1;//正确，但不知原因（todo）
                return type;
            }
            catch (Exception e) { throw e; }
        }

        /// <summary>
        /// 使用JSON形式，对给定的payload进行加密和sign
        /// </summary>
        /// <param name="payload">需要进行加密和sign的payload</param>
        /// <param name="token">输出的令牌</param>
        /// <param name="expires">以milliseconds计算的过期时间</param>
        /// <returns>返回令牌</returns>
        public ByteBuffer encryptAndSign_json(ByteBuffer payload, ByteBuffer token, long expires)
        {
            return encryptAndSign((byte)Type.JSON, payload, token, expires);
        }

        /// <summary>
        /// 使用SYS形式，对给定的payload进行加密和sign
        /// </summary>
        /// <param name="payload">需要进行加密和sign的payload</param>
        /// <param name="token">输出的令牌</param>
        /// <param name="expires">以milliseconds计算的过期时间</param>
        /// <returns>返回令牌</returns>
        public ByteBuffer encryptAndSign_sys(ByteBuffer payload, ByteBuffer token, long expires)
        {
            return encryptAndSign((byte)Type.SYS, payload, token, expires);
        }

        //验证并返回一个关于payload的字符串
        public string verifyAndDecrypt(string data, long now)
        {
            ByteBuffer outbyte = ByteBuffer.Allocate(1024 * 8);
            verifyAndDecrypt(data, outbyte, now);
            byte[] tempbytes = outbyte.ToArray();
            return Encoding.UTF8.GetString(tempbytes, (int)outbyte.Position, outbyte.Remaining);
        }

        /// <summary>
        /// 创建令牌
        /// </summary>
        /// <param name="result_json">需要创建令牌的内容</param>
        /// <param name="xjwt_type">创建方式XJWT.Type.JSON或XJWT.Type.SYS</param>
        /// <param name="now">创建的时间</param>
        /// <returns></returns>
        public string createToken(string result_json, Type xjwt_type, long now)
        {
            ByteBuffer payload = ByteBuffer.Allocate(result_json.Length);
            payload.Put(Encoding.UTF8.GetBytes(result_json));
            payload.Flip();
            //创建令牌
            ByteBuffer token = ByteBuffer.Allocate(1024);
            if (xjwt_type == Type.JSON)
                encryptAndSign_json(payload, token, now + 10000);
            if (xjwt_type == Type.SYS)
                encryptAndSign_sys(payload, token, now + 10000);

            string xjwt_str = new string(Encoding.UTF8.GetChars(token.ToArray(), 0, token.Remaining));
            return xjwt_str;
        }
    }
}