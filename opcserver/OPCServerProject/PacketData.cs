using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPCServerProject
{
    public class PacketData
    {
        public Dictionary<string, object> packetDataMap = new Dictionary<string, object>();
        public string moduleID;
        public string sendTime;
        public bool isAlert;
        public int alertPos;
        public int alertValue;

        public string resolvePos(int pos)
        {
            if (pos == 1)
                return "AI1/AC1";
            else if (pos == 2)
                return "AI2/AC2";
            else if (pos == 3)
                return "AI3/AC3";
            else if (pos == 4)
                return "AI4/AC4";
            else if (pos == 5)
                return "AI5/AC5";
            else if (pos == 6)
                return "AI6/AC6";
            else if (pos == 7)
                return "DI1";
            else if (pos == 8)
                return "DI2";
            else if (pos == 9)
                return "DI3";
            else if (pos == 10)
                return "DI4";
            else if (pos == 11)
                return "DI5";
            else if (pos == 12)
                return "DI6";
            else if (pos == 13)
                return "DO1";
            else if (pos == 14)
                return "DO2";
            else if (pos == 15)
                return "DO3";
            else if (pos == 16)
                return "DO4";
            else if (pos == 17)
                return "DO5";
            else 
                return "DO6";
        }

        public object transformValue(int value, int pos)
        {
            if (pos >= 1 && pos <= 6)
            {
                return value;
            }
            else
            {
                if (value == 0)
                    return false;
                else
                    return true;
            }
        }

        public PacketData()
        {
            packetDataMap.Add("GPRS-Level", null);
            packetDataMap.Add("AI1/AC1", null);
            packetDataMap.Add("AI2/AC2", null);
            packetDataMap.Add("AI3/AC3", null);
            packetDataMap.Add("AI4/AC4", null);
            packetDataMap.Add("AI5/AC5", null);
            packetDataMap.Add("AI6/AC6", null);
            packetDataMap.Add("DI1", null);
            packetDataMap.Add("DI2", null);
            packetDataMap.Add("DI3", null);
            packetDataMap.Add("DI4", null);
            packetDataMap.Add("DI5", null);
            packetDataMap.Add("DI6", null);
            packetDataMap.Add("DO1", null);
            packetDataMap.Add("DO2", null);
            packetDataMap.Add("DO3", null);
            packetDataMap.Add("DO4", null);
            packetDataMap.Add("DO5", null);
            packetDataMap.Add("DO6", null);
        }

        public static int bytesToInt(byte[] src, int offset)
        {
            int value = src[offset] << 8 | src[offset + 1];
            return value;
        }  

        public static PacketData resolveData1(byte[] data1)
        {
            PacketData packet = new PacketData();
            packet.isAlert = false;
            byte[] moduleID = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                moduleID[i] = data1[i];
            }
            packet.moduleID = Encoding.ASCII.GetString(moduleID);
            int year = Convert.ToInt16(data1[12]);
            int month = Convert.ToInt16(data1[13]);
            int day = Convert.ToInt16(data1[14]);
            int hour = Convert.ToInt16(data1[15]);
            int minute = Convert.ToInt16(data1[16]);
            int second = Convert.ToInt16(data1[17]);
            packet.sendTime = "20"+year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second;

            packet.packetDataMap["GPRS-Level"] = Convert.ToInt16(data1[18]);

            packet.packetDataMap["AI1/AC1"] = bytesToInt(data1, 19); 
            packet.packetDataMap["AI2/AC2"] = bytesToInt(data1, 21);
            packet.packetDataMap["AI3/AC3"] = bytesToInt(data1, 23);
            packet.packetDataMap["AI4/AC4"] = bytesToInt(data1, 25);
            packet.packetDataMap["AI5/AC5"] = bytesToInt(data1, 27);
            packet.packetDataMap["AI6/AC6"] = bytesToInt(data1, 29);

            string value = Convert.ToString(data1[31],2);
            value = transferToEightDigit(value);
            packet.packetDataMap["DI1"] = transferToBoolean(value[7]);
            packet.packetDataMap["DI2"] = transferToBoolean(value[6]);
            packet.packetDataMap["DI3"] = transferToBoolean(value[5]);
            packet.packetDataMap["DI4"] = transferToBoolean(value[4]);
            packet.packetDataMap["DI5"] = transferToBoolean(value[3]);
            packet.packetDataMap["DI6"] = transferToBoolean(value[2]);

            value = Convert.ToString(data1[32], 2);
            value = transferToEightDigit(value);
            packet.packetDataMap["DO1"] = transferToBoolean(value[7]);
            packet.packetDataMap["DO2"] = transferToBoolean(value[6]);
            packet.packetDataMap["DO3"] = transferToBoolean(value[5]);
            packet.packetDataMap["DO4"] = transferToBoolean(value[4]);
            packet.packetDataMap["DO5"] = transferToBoolean(value[3]);
            packet.packetDataMap["DO6"] = transferToBoolean(value[2]);
            return packet;
        }

        private static bool transferToBoolean(char str)
        {
            if (str == '0')
                return false;
            else
                return true;
        }

        private static string transferToEightDigit(string str)
        {
            int zero = 8 - str.Length;
            string result = "";
            for (int i = 0; i < zero; i++)
            {
                result += "0";
            }
            result += str;
            return result;
        }

        public static PacketData resolveData4(byte[] data4)
        {
            PacketData packet = new PacketData();
            packet.isAlert = true;

            byte[] moduleID = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                moduleID[i] = data4[i];
            }
            packet.moduleID = Encoding.ASCII.GetString(moduleID);

            packet.alertPos = Convert.ToInt32(data4[12]);
            packet.alertValue = bytesToInt(data4, 13);

            return packet;
        }
    }
}
