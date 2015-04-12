using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPCServerProject
{
    class LabelStructure
    {
        public List<string> equipmentNames = new List<string>();
        public Dictionary<string, Dictionary<string, LabelItem>> labelItemNamems = new Dictionary<string, Dictionary<string, LabelItem>>();

        public void load(string[] labels)
        {
            if (labels.Length > 0)
            {
                for (int i = 0; i < labels.Length; i++)
                {
                    if (labels[i].Equals(""))
                        continue;

                    string labelLine = labels[i];
                    string[] prop = labelLine.Split('=');
                    string equipment = prop[0];
                    string[] labelText = prop[1].Split(',');
                    this.equipmentNames.Add(equipment);
                    this.labelItemNamems.Add(equipment, new Dictionary<string, LabelItem>());
                    for (int j = 0; j < labelText.Length; j++)
                    {
                        if (!labelText[j].Equals(""))
                        {
                            string[] labelSubElement = labelText[j].Split('|');
                            string labelName = labelSubElement[0];
                            string labelType = labelSubElement[1];
                            string inputOutput = labelSubElement[2];
                            LabelItem item = new LabelItem();
                            item.labelName = labelName;
                            item.labelType = labelType;
                            item.inputOutput = inputOutput;
                            labelItemNamems[equipment].Add(labelName, item);
                        }
                    }
                }
            }
        }
    }

    class LabelItem
    {
        public string labelName;
        public string labelType;
        public string inputOutput;
    }
}
