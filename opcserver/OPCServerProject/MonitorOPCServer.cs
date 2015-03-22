using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace OPCServerProject
{
    class MonitorOPCServer
    {
        private static MonitorOPCServer server;
        Dictionary<string, SerialPort> PortMapping = new Dictionary<string, SerialPort>();

        private MonitorOPCServer()
        {
        }

        public static MonitorOPCServer getInstance()
        {
            if (server == null)
                server = new MonitorOPCServer();
            return server;
        }

        public void startMonitor()
        {
            LoadPort();
            PortMapping["RTU1"].DataReceived += new SerialDataReceivedEventHandler(port_DataReceive);//在此将调用返回监控数据函数
            PortMapping["RTU1"].ReceivedBytesThreshold = 1;
            PortMapping["RTU1"].RtsEnable = true;
        }

        public void shoutdownMonitor()
        {
        }

        private void port_DataReceive(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort serialPort = (SerialPort)sender;
                byte[] buffer_ = new byte[serialPort.ReadBufferSize];
                int length = serialPort.Read(buffer_, 0, buffer_.Length);
                //按规定长度存储，转化成十六进制，便于日志显示
                string result = "";
                byte[] buffer = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    buffer[i] = buffer_[i];
                    result += string.Format("{0:X2}", buffer_[i]);
                }

                PacketData packetData = parseProtocol(result);
                saveToDatabase(packetData);
            }
            catch (Exception ex)
            {
            }
        }

        private PacketData parseProtocol(string dataString)
        {
            PacketData packetData = null;
            return packetData;
        }

        private void saveToDatabase(PacketData packetData)
        {
            OPCServerUtil.updateToOPCLabel(packetData);
        }

        public void LoadPort()
        {
            if (PortMapping != null)
            {
                PortMapping.Add("RTU1", new SerialPort("COM3", 9600, Parity.Even, 8, StopBits.One));
                PortMapping["RTU1"].Open();
            }
        }
    }
}
