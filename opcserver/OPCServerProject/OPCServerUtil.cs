using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPCServerProject
{
    class OPCServerUtil
    {
        public void updateToOPCLabel(PacketData data, Dictionary<string,Dictionary<string,uint>> handles)
        {
            if (handles.ContainsKey(data.moduleID))
            {
                Dictionary<string, uint> moduleHandle = handles[data.moduleID];
                foreach (KeyValuePair<string, uint> handle in moduleHandle)
                {
                    if (data.packetDataMap.ContainsKey(handle.Key))
                    {
                        //System.Runtime.InteropServices.FILETIME f = new System.Runtime.InteropServices.FILETIME();
                        deactive();
                        OPClib.UpdateTag(handle.Value, data.packetDataMap[handle.Key], 192);//, System.Runtime.InteropServices.FILETIME);
                        
                       // LogUtil.writeLog(LogUtil.getFileName(), "[" + DateTime.Now.ToString() + "]: OPC标签更新");
                    }
                }
            }
        }

        public void updateAlertToOPCLabel(PacketData data, Dictionary<string, Dictionary<string, uint>> handles)
        {
            if (handles.ContainsKey(data.moduleID))
            {
                Dictionary<string, uint> moduleHandle = handles[data.moduleID];
                int pos = data.alertPos;
                int value = data.alertValue;
                if (moduleHandle.ContainsKey(data.resolvePos(pos)))
                {
                    deactive();
                    OPClib.UpdateTag(moduleHandle[data.resolvePos(pos)], data.transformValue(value,pos), 192);
                }
            }
        }

        private object getDefaultValue(string type)
        {
            if (type.Equals("int"))
                return 0;
            else if (type.Equals("string"))
                return "";
            else if (type.Equals("bool"))
                return false;
            else 
                return null;
        }

        public Dictionary<string,Dictionary<string,uint>> addOPCLabels(LabelStructure label)
        {
            Dictionary<string, Dictionary<string, uint>> labelHandles = new Dictionary<string, Dictionary<string, uint>>();
            foreach (string equip in label.equipmentNames)
            {
                Dictionary<string, LabelItem> items = label.labelItemNamems[equip];
                Dictionary<string, uint> handles = new Dictionary<string, uint>();
                
                foreach (KeyValuePair<string, LabelItem> pair in items)
                {
                    deactive();
                    uint handle = OPClib.CreateTag(equip + "." + pair.Key, getDefaultValue(pair.Value.labelType), 192, true);
                    handles.Add(pair.Key, handle);
                }

                labelHandles.Add(equip, handles);
            }
         
            return labelHandles;
        }

        public void deactive()
        {
            OPClib.Deactivate30MinTimer("JVRPS53R5V64226N62H4");
        }

        public void registerOPCServer()
        {
            string exepath;
            exepath = System.Windows.Forms.Application.StartupPath + "\\OPCServerProject.exe";
            deactive();
            OPClib.WriteNotificationDelegate WriteCallback = new OPClib.WriteNotificationDelegate(MyWriteCallback);
            OPClib.UpdateRegistry("{4FEFF1A0-5B75-4EF3-BC6D-37145069E552}",
                                        "My OPC Server",
                                        "My OPC Server",
                                        exepath);
            OPClib.SetVendorInfo("OPCServer.Com");
            OPClib.InitWTOPCsvr("{4FEFF1A0-5B75-4EF3-BC6D-37145069E552}", 1000);

            OPClib.EnableWriteNotification(WriteCallback, true);
        }

        public void unRegisterOPCServer()
        {
            deactive();
            OPClib.UnregisterServer("{4FEFF1A0-5B75-4EF3-BC6D-37145069E552}", "My OPC Server");
        }

        public void MyWriteCallback(UInt32 hItem, ref Object Value, ref UInt32 ResultCode)
        {
            ResultCode = 0;
        }
    }
}
