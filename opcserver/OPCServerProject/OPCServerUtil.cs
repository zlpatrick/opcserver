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
                        OPClib.UpdateTag(handle.Value, data.packetDataMap[handle.Key], 192);
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
                    OPClib.UpdateTag(moduleHandle[data.resolvePos(pos)], value, 192);

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
                    uint handle = OPClib.CreateTag(equip + "." + pair.Key, getDefaultValue(pair.Value.labelType), 192, true);
                    handles.Add(pair.Key, handle);
                }

                labelHandles.Add(equip, handles);
            }
         
            return labelHandles;
        }

        public void registerOPCServer()
        {
            string exepath;
            exepath = System.Windows.Forms.Application.StartupPath + "\\OPCServerProject.exe";
            
            OPClib.WriteNotificationDelegate WriteCallback = new OPClib.WriteNotificationDelegate(MyWriteCallback);
            OPClib.UpdateRegistry("{57E9743C-0678-419c-B28B-7508417DAC8C}",
                                        "My OPC Server",
                                        "My OPC Server",
                                        exepath);
            OPClib.SetVendorInfo("OPCServer.Com");
            OPClib.InitWTOPCsvr("{57E9743C-0678-419c-B28B-7508417DAC8C}", 1000);

            OPClib.EnableWriteNotification(WriteCallback, true);
        }

        public void MyWriteCallback(UInt32 hItem, ref Object Value, ref UInt32 ResultCode)
        {
            ResultCode = 0;
        }
    }
}
