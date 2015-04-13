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

            string sql = string.Format("insert into [{0}]" +
           "(rtu,[time],dbm,ch1,ch2,ch3,ch4,ch5,ch6,ch7,ch8,ch9,ch10,ch11,ch12,ch13,ch14,ch15,ch16,ch17,ch18) values" +
           "('{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21})", tableName,
           data.moduleID, data.sendTime, data.packetDataMap["GPRS-Level"], data.packetDataMap["AI1/AC1"], data.packetDataMap["AI2/AC2"]
           , data.packetDataMap["AI3/AC3"], data.packetDataMap["AI4/AC4"], data.packetDataMap["AI5/AC5"], data.packetDataMap["AI6/AC6"]
           , data.packetDataMap["DI1"], data.packetDataMap["DI2"], data.packetDataMap["DI3"], data.packetDataMap["DI4"]
           , data.packetDataMap["DI5"], data.packetDataMap["DI6"], data.packetDataMap["DO1"], data.packetDataMap["DO2"]
           , data.packetDataMap["DO3"], data.packetDataMap["DO4"], data.packetDataMap["DO5"], data.packetDataMap["DO6"]);

            DBUtil db = new DBUtil();
            db.executeNonQuerySQL(sql);
        }

        public static void saveAlertPacketContentToDb(PacketData data)
        {
            string rtu = data.moduleID;
            string tableName = MonitorOPCServer.getInstance().alertTable;

            string sql = "";/* string.Format("insert into [{0}]" +
             "(rtu,[time],dbm,ch1,ch2,ch3,ch4,ch5,ch6,ch7,ch8,ch9,ch10,ch11,ch12,ch13,ch14,ch15,ch16,ch17,ch18) values" +
             "('{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21})", tableName,
             data.moduleID, data.sendTime, data.packetDataMap["GPRS-Level"], data.packetDataMap["AI1/AC1"], data.packetDataMap["AI2/AC2"]
             , data.packetDataMap["AI3/AC3"], data.packetDataMap["AI4/AC4"], data.packetDataMap["AI5/AC5"], data.packetDataMap["AI6/AC6"]
             , data.packetDataMap["DI1"], data.packetDataMap["DI2"], data.packetDataMap["DI3"], data.packetDataMap["DI4"]
             , data.packetDataMap["DI5"], data.packetDataMap["DI6"], data.packetDataMap["DO1"], data.packetDataMap["DO2"]
             , data.packetDataMap["DO3"], data.packetDataMap["DO4"], data.packetDataMap["DO5"], data.packetDataMap["DO6"]);*/

            DBUtil db = new DBUtil();
            db.executeNonQuerySQL(sql);
        } 
    }
}
