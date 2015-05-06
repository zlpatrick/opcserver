using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace OPCServerProject
{
    class MonitorOPCServer
    {
        private static MonitorOPCServer server;
        public string ip;
        public int port;
        private Socket listener;
        public Thread listeningThread;
        public Thread sendCommandThread;
        public Thread timeoutCheckingThread;
     
        OPCServerUtil opc = new OPCServerUtil();
        public Dictionary<string, string> dbNameMapping = new Dictionary<string, string>();
        public string alertTable;
        public Dictionary<string, LabelStructure> labels = new Dictionary<string, LabelStructure>(); 
        public Dictionary<string, Dictionary<string, uint>> handles = new Dictionary<string, Dictionary<string, uint>>();
        public Dictionary<string, DateTime> lastUpdate = new Dictionary<string, DateTime>();
        public Dictionary<string, PacketData> lastUpdateData = new Dictionary<string, PacketData>();
        public List<string> onlineLabel = new List<string>();
        public Dictionary<string, Dictionary<string, object>> currentOutputValue = new Dictionary<string,Dictionary<string,object>>();
       
        private MonitorOPCServer()
        {
        }

        public static MonitorOPCServer getInstance()
        {
            if (server == null)
                server = new MonitorOPCServer();
            return server;
        }

        private void loadInfo()
        {
            if (File.Exists("alert.properties"))
            {
                alertTable = File.ReadAllText("alert.properties");
            }

            if (File.Exists("dbmapping.properties"))
            {
                string[] mappings = File.ReadAllLines("dbmapping.properties");
                for (int i = 0; i < mappings.Length; i++)
                {
                    string[] values = mappings[i].Split(',');
                    dbNameMapping.Add(values[0], values[1]);
                }
            }
            LabelStructure labelStructure = new LabelStructure();
            if (File.Exists("label.properties"))
            {
                string[] labels = File.ReadAllLines("label.properties");
                labelStructure.load(labels);
            }
            
            opc.registerOPCServer();
            handles = opc.addOPCLabels(labelStructure);
        }

        public void startMonitor(string ip,int port,int timeout,bool enableOutput)
        {
            loadInfo();
            
            try
            {
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse(ip);
                listener.Connect(new IPEndPoint(ipAddress, port));
                byte[] initial = new byte[] { 0x68, 0x00, 0x03, 0x05, 0x01, 0x00, 0x06, 0x16 };
                listener.Send(initial); 
            }
            catch(Exception ex)
            {
                LogUtil.writeLog(LogUtil.getFileName(), "[" + DateTime.Now.ToString() + "]: 启动失败");
                shoutdownMonitor();
            }
            try
            {
                //接收线程
                listeningThread = new Thread(new ParameterizedThreadStart(ReceiveWorkThread));
                listeningThread.Start(listener);

                if (timeout > 0)
                {
                    //超时检测线程
                    timeoutCheckingThread = new Thread(new ParameterizedThreadStart(TimeoutCheckingThreadWork));
                    timeoutCheckingThread.Start(timeout);
                }

                //发送线程
                if (enableOutput)
                {
                    sendCommandThread = new Thread(new ThreadStart(sendCommandWorkThread));
                    sendCommandThread.Start();
                }
            }
            catch (Exception ex)
            {
                LogUtil.writeLog(LogUtil.getFileName(), "[" + DateTime.Now.ToString() + "]: 启动失败");
                shoutdownMonitor();
            }

            LogUtil.writeLog(LogUtil.getFileName(), "[" + DateTime.Now.ToString() + "]: 成功启动监控");
        }

        private void sendCommandWorkThread()
        {
            while(true)
            {
                Thread.Sleep(1000);
                List<string> nowOnline = new List<string>();
                nowOnline.AddRange(onlineLabel);
                foreach (string module in nowOnline)
                {
                    if(handles.ContainsKey(module))
                    {
                        Dictionary<string, uint> handle = handles[module];

                        uint DO1 = handle["DO1"];
                        uint DO2 = handle["DO2"];
                        uint DO3 = handle["DO3"];
                        uint DO4 = handle["DO4"];
                        uint DO5 = handle["DO5"];
                        uint DO6 = handle["DO6"];
                        object value1 = null;
                        object value2 = null;
                        object value3 = null;
                        object value4 = null;
                        object value5 = null;
                        object value6 = null;
                        try
                        {
                            OPClib.ReadTag(DO1, ref value1);
                            OPClib.ReadTag(DO2, ref value2);
                            OPClib.ReadTag(DO3, ref value3);
                            OPClib.ReadTag(DO4, ref value4);
                            OPClib.ReadTag(DO5, ref value5);
                            OPClib.ReadTag(DO6, ref value6);
                        }
                        catch(Exception ex)
                        {
                            continue;
                        }
                        
                        Dictionary<string, object> values = new Dictionary<string, object>();
                        values.Add("DO1", value1);
                        values.Add("DO2", value2);
                        values.Add("DO3", value3);
                        values.Add("DO4", value4);
                        values.Add("DO5", value5);
                        values.Add("DO6", value6);

                        if(!currentOutputValue.ContainsKey(module))
                        {
                            currentOutputValue.Add(module,values);
                        }
                        else
                        {
                            Dictionary<string, object> savedValue = currentOutputValue[module];
                            if (!compareOutputValues(savedValue, values))
                            {
                                try
                                {
                                    listener.Send(constructDataPacket(values,module));
                                    currentOutputValue[module] = values;
                                }
                                catch(Exception ex)
                                {
                                    continue;
                                }
                            }
                        }
                    }

                }
                
            }
        }

        public byte[] constructDataPacket(Dictionary<string,object> data,string module)
        {
            byte[] modulebyte = Encoding.ASCII.GetBytes(module);
          
            byte[] send = new byte[20];
            send[0] = 0x68;
            send[1] = 0x00;
            send[2] = 0x10;
            send[3] = 0x03;
            send[4] = 0x01;
            send[5] = 0x00;
            for(int i = 0 ; i<12;i++)
            {
                send[6 + i] = modulebyte[i];
            }
            send[18] = constructbit(data);
            send[19] = 0x16;
            return send;
        }

        public byte constructbit(Dictionary<string,object> data)
        {
            string result = "";
            if((int)data["DO1"] == 1)
            {
                result += "1";
            }
            else
            {
                result += "0";
            }

            if((int)data["DO2"] == 1)
            {
                result += "1";
            }
            else
            {
                result += "0";
            }

            if((int)data["DO3"] == 1)
            {
                result += "1";
            }
            else
            {
                result += "0";
            }

            if((int)data["DO4"] == 1)
            {
                result += "1";
            }
            else
            {
                result += "0";
            }

            if((int)data["DO5"] == 1)
            {
                result += "1";
            }
            else
            {
                result += "0";
            }

            if((int)data["DO6"] == 1)
            {
                result += "1";
            }
            else
            {
                result += "0";
            }
            result += "00";

            byte b = Convert.ToByte(result,2);
            return b;
        }

        private bool compareOutputValues( Dictionary<string,object> savedValue, Dictionary<string,object> currentValue )
        {
            bool equals = true;
            foreach (KeyValuePair<string, object> current in currentValue)
            {
                if (savedValue.ContainsKey(current.Key) && (!savedValue[current.Key].Equals(current.Value)))
                {
                    equals = false;
                    break;
                }
            }

            return equals;
        }

        public void shoutdownMonitor()
        {
            try
            {
                listener.Close();
            }
            catch (Exception ex)
            {
            }

            try
            {
                listeningThread.Abort();
            }
            catch (Exception ex)
            {
            }

            try
            {
                if (timeoutCheckingThread != null)
                {
                    timeoutCheckingThread.Abort();
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                if (sendCommandThread != null)
                {
                    sendCommandThread.Abort();
                }
            }
            catch (Exception ex)
            {
            }

            dbNameMapping.Clear();
            alertTable = null;
            labels.Clear();
            handles.Clear();
            lastUpdate.Clear();
            lastUpdateData.Clear();
            onlineLabel.Clear();
            currentOutputValue.Clear();
            try
            {
                opc.unRegisterOPCServer();
            }
            catch (Exception ex)
            {
            }

            server = null;
            LogUtil.writeLog(LogUtil.getFileName(), "[" + DateTime.Now.ToString() + "]: 关闭监控");
        }

        private void TimeoutCheckingThreadWork(object obj)
        {
            int timeout = (int)obj;
            while(true)
            {
                Thread.Sleep(1000);
                DateTime now = DateTime.Now ;

                Dictionary<string, DateTime> oneTimeUpdate = new Dictionary<string, DateTime>();
                foreach(KeyValuePair<string,DateTime> updatedItem in lastUpdate)
                {
                    oneTimeUpdate.Add(updatedItem.Key, updatedItem.Value);
                }
                foreach (KeyValuePair<string, DateTime> updatedItem in oneTimeUpdate)
                {
                    DateTime dt = updatedItem.Value;
                    TimeSpan span = now - dt;
                    string moduleID = updatedItem.Key;
                    if( span.Seconds >= timeout * 60 )
                    {
                        if(handles.ContainsKey(moduleID))
                        {
                            Dictionary<string, uint> handle = handles[moduleID];
                            foreach(KeyValuePair<string, uint> singleLabel in handle)
                            {
                                string labelName = singleLabel.Key;
                                uint labelHandle = singleLabel.Value;
                                if (lastUpdateData.ContainsKey(moduleID) &&
                                    lastUpdateData[moduleID].packetDataMap.ContainsKey(labelName))
                                {
                                    OPClib.UpdateTag(labelHandle, lastUpdateData[moduleID].packetDataMap[labelName], 0);
                                    //LogUtil.writeLog(LogUtil.getFileName(), "[" + DateTime.Now.ToString() + "]: 信号差");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ReceiveWorkThread(object obj)
        {
            Thread.CurrentThread.IsBackground = true;
            Socket socket = (Socket)obj;
            while (true)
            {
                try
                {
                    //接收数据
                    byte[] bytes_ = new byte[1024];
                    int length = 0;
                    try
                    {
                        length = socket.Receive(bytes_);
                        if (length <= 0)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                    //按规定长度存储
                    byte[] bytes = new byte[length];
                    for (int i = 0; i < length; i++)
                    {
                        bytes[i] = bytes_[i];
                    }
                    //检查粘包数据
                    List<byte[]> list = PacketUtil.CheckLong(length, bytes);
                    if (list.Count > 1)
                    {
                        string r = "";
                        for (int i = 0; i < length; i++)
                        {
                            r += string.Format("{0:X2}", bytes[i]);
                        }
                        LogUtil.writeLog(LogUtil.getFileName(), "[" + DateTime.Now.ToString() + "]: 粘包数据是：<" + r + ">");
                    }
                    //数据处理
                    for (int kk = 0; kk < list.Count; kk++)
                    {
                        byte[] bytes1 = list[kk];
                        int length1 = bytes1.Length;
                        //先都转化为16进制字符
                        string result = "";
                        for (int i = 0; i < length1; i++)
                        {
                            result += string.Format("{0:X2}", bytes1[i]);
                        }
                        if (PacketUtil.CheckData(length1, bytes1))
                        {
                            if (result.StartsWith("68") && result.Substring(8, 2) == "00")
                            {
                                try
                                {
                                    //RTU连接数据 [68 00 xx 02 00 00 data1 cs 16]
                                    if (result.Substring(6, 2) == "02")
                                    {
                                        try
                                        {
                                            //解析从12开始的40个字节，作为data1的数据
                                            byte[] data1 = new byte[33];
                                            for (int i = 0; i < 33; i++)
                                            {
                                                data1[i] = bytes1[i + 6];
                                            }

                                            PacketData data1Packet = PacketData.resolveData1(data1);
   
                                            opc.updateToOPCLabel(data1Packet,handles);
                                            PacketUtil.savePacketContentToDb(data1Packet);

                                            DateTime now = DateTime.Now;
                                            if (!lastUpdate.ContainsKey(data1Packet.moduleID))
                                            {
                                                lastUpdate.Add(data1Packet.moduleID, now);
                                            }
                                            else
                                            {
                                                lastUpdate[data1Packet.moduleID] = now;
                                            }

                                            if (!lastUpdateData.ContainsKey(data1Packet.moduleID))
                                            {
                                                lastUpdateData.Add(data1Packet.moduleID, data1Packet);
                                            }
                                            else
                                            {
                                                lastUpdateData[data1Packet.moduleID] = data1Packet;
                                            }

                                            if(!onlineLabel.Contains(data1Packet.moduleID))
                                            {
                                                onlineLabel.Add(data1Packet.moduleID);
                                            }

                                           // LogUtil.writeLog(LogUtil.getFileName(), "[" + DateTime.Now.ToString() + "]: 收到data1");
                                        }                                        
                                        catch (Exception ex)
                                        {
                                            continue;
                                        }
                                    }
                                    //报警数据
                                    else if (result.Substring(6, 2) == "06")
                                    {
                                        try
                                        {
                                            //解析从12开始的40个字节，作为data1的数据
                                            byte[] data4 = new byte[14];
                                            for (int i = 0; i < 14; i++)
                                            {
                                                data4[i] = bytes1[i + 6];
                                            }
                                            
                                            PacketData data4Packet = PacketData.resolveData4(data4);
                                            
                                            opc.updateAlertToOPCLabel(data4Packet,handles);
                                            PacketUtil.saveAlertPacketContentToDb(data4Packet);
                                          //  LogUtil.writeLog(LogUtil.getFileName(), "[" + DateTime.Now.ToString() + "]: 收到data4");
                                        }
                                        catch (Exception ex)
                                        {
                                            continue;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            LogUtil.writeLog(LogUtil.getFileName(), "[" + DateTime.Now.ToString() + "]: " + "检验码错误，此时的数据是：<" + result + ">");
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }
    }
}
