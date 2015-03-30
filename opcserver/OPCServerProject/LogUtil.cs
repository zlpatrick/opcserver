using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;

namespace OPCServerProject
{
    public class LogUtil
    {
        /// <summary>
        /// 得到日志文件名
        /// </summary>
        /// <returns></returns>
        public static string getFileName()
        {
            string fileName = "LogFiles/" + DateTime.Today.Year + "-" + string.Format("{0:D2}", DateTime.Today.Month) + "-" + string.Format("{0:D2}", DateTime.Today.Day) + ".log";
            return fileName;
        }
        /// <summary>
        /// 写日志文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="log"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]//多线程同步
        public static void writeLog(string file, string log)
        {
            File.AppendAllText(file, log + "\r\n");
        }
        /// <summary>
        /// 读文件
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]//多线程同步
        public static string[] ReadFile(string file)
        {
            string[] list = File.ReadAllLines(file);
            return list;
        }
        /// <summary>
        /// 写文件
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]//多线程同步
        public static void WriteFile(string file, string content)
        {
            File.AppendAllText(file, content + "\r\n");
        }
        /// <summary>
        /// 文件清空
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]//多线程同步
        public static void ClearFile(string file)
        {
            File.WriteAllText(file, "");
        }
        /// <summary>
        /// 读取文件portMapping.txt
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]//多线程同步
        public static Dictionary<string, string> ReadPortMappingFile()
        {
            Dictionary<string, string> PortMapping = new Dictionary<string, string>();//所有端口名(RtuID,COM10)
            string[] PortMapping_ = ReadFile("portMapping.txt");
            if (PortMapping_.Length != 0)
            {
                if (PortMapping_[0] != "")
                {
                    for (int i = 0; i < PortMapping_.Length; i++)
                    {
                        PortMapping.Add(PortMapping_[i].Substring(0, 12), PortMapping_[i].Substring(12, PortMapping_[i].Length - 12));
                    }
                }
            }
            return PortMapping;
        }
    }
}
