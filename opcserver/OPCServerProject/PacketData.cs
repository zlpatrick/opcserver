using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPCServerProject
{
    class PacketData
    {
        public string moduleID;
        public string sendTime;
        public int GPRSLevel;
        public int AI1AC1;
        public int AI2AC2;
        public int AI3AC3;
        public int AI4AC4;
        public int AI5AC5;
        public int AI6AC6;
        public bool DI1;
        public bool DI2;
        public bool DI3;
        public bool DI4;
        public bool DI5;
        public bool DI6;
        public bool DO1;
        public bool DO2;
        public bool DO3;
        public bool DO4;
        public bool DO5;
        public bool DO6;

        public static PacketData resolveData1(byte[] data1)
        {
            PacketData packet = new PacketData();
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

            packet.GPRSLevel = Convert.ToInt16(data1[18]);
            packet.AI1AC1 = BitConverter.ToInt16(data1, 19);
            packet.AI2AC2 = BitConverter.ToInt16(data1, 21);
            packet.AI3AC3 = BitConverter.ToInt16(data1, 23);
            packet.AI4AC4 = BitConverter.ToInt16(data1, 25);
            packet.AI5AC5 = BitConverter.ToInt16(data1, 27);
            packet.AI6AC6 = BitConverter.ToInt16(data1, 29);

            string value = Convert.ToString(data1[31], 2);
            packet.DI1 = Convert.ToBoolean(value[0]);
            packet.DI2 = Convert.ToBoolean(value[1]);
            packet.DI3 = Convert.ToBoolean(value[2]);
            packet.DI4 = Convert.ToBoolean(value[3]);
            packet.DI5 = Convert.ToBoolean(value[4]);
            packet.DI6 = Convert.ToBoolean(value[5]);

            value = Convert.ToString(data1[32], 2);
            packet.DO1 = Convert.ToBoolean(value[0]);
            packet.DO2 = Convert.ToBoolean(value[1]);
            packet.DO3 = Convert.ToBoolean(value[2]);
            packet.DO4 = Convert.ToBoolean(value[3]);
            packet.DO5 = Convert.ToBoolean(value[4]);
            packet.DO6 = Convert.ToBoolean(value[5]);
            return packet;
        }
    }
}
