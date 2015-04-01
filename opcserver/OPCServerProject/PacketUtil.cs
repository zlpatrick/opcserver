using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPCServerProject
{
    public class PacketUtil
    {

        /// <summary>
        /// 校验数据,若是一个多包数据，需要写成几个包，分别处理
        /// </summary>
        public static List<byte[]> CheckLong(int length, byte[] bytes)
        {
            List<byte[]> list = new List<byte[]>();
            int L = bytes[1] * 256 + bytes[2];
            if (length == L + 5)
            {
                list.Add(bytes);
            }
            else if (length > L + 5)
            {
                while (length >= L + 5)
                {
                    byte[] bytes1 = new byte[L + 5];
                    for (int i = 0; i < L + 5; i++)
                    {
                        bytes1[i] = bytes[i];
                    }
                    list.Add(bytes1);
                    for (int j = 0; j < length - L - 5; j++)
                    {
                        bytes[j] = bytes[L + 5 + j];
                    }
                    length = length - L - 5;
                    L = bytes[1] * 256 + bytes[2];
                }
            }
            return list;
        }
        /// <summary>
        /// 校验单个帧数据
        /// </summary>
        public static bool CheckData(int length, byte[] bytes)
        {
            bool b = false;
            int L = bytes[1] * 256 + bytes[2];
            if (length - 5 == L)
            {
                int cs = 0;
                for (int i = 0; i < length - 5; i++)
                {
                    cs += bytes[i + 3];
                }
                cs = cs % 256;
                if (cs == bytes[length - 2])
                {
                    b = true;
                }
            }
            return b;
        }
        /// <summary>
        /// 字符转16进制按十进制发送
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ConvertAry(string value)
        {
            byte[] sendMessage = null;
            if (value.Length % 2 == 0)
            {
                sendMessage = new byte[value.Length / 2];
                for (int i = 0; i < value.Length / 2; i++)
                {
                    string w = value.Substring(2 * i, 2);
                    sendMessage[i] = Convert.ToByte(w, 16);
                }
            }
            return sendMessage;
        }

        public static void savePacketContentToDb(PacketData data)
        {
            string rtu = data.moduleID;
            string tableName = MonitorOPCServer.getInstance().dbNameMapping[rtu];

            string sql = "insert into ["+tableName+"]";
        }
    }
}
