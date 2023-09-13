using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PUBG_Profiler.Content;
using System.IO;
using System.Diagnostics;

namespace PUBG_Profiler
{
    class PresetUtensils
    {
        public static readonly string[] SettingKeys = { "Normal", @"SensitiveName=""Targeting""", "Scoping"
        ,"Scope2X", "Scope3X", "Scope4X", "Scope6X", "Scope8X", "Scope15X", "ScopingMagnified"};
        public static readonly string[] IDKeys = new string[SettingKeys.Length + 1];

        public static void LoadIDKeys()
        {
            for(int i = 0; i < SettingKeys.Length; i++)
                IDKeys[i] = "0x0" + Convert.ToString(i);
            IDKeys[IDKeys.Length - 1] = "0x0v";
        }

        private static string GetFilePath(string FileName = "")
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(MainContent.GameDir + MainContent.BaseNamePreset);
            if (FileName != "")
                stringBuilder.Append(MainContent.KeyName + FileName);            
            stringBuilder.Append(MainContent.INI);
            return stringBuilder.ToString();
        }

        public static List<string> GetPresetSensitivity(string FileName = "")
        {
            List<string> ToReturn = new(new string[SettingKeys.Length]);
            string ToReadPath = GetFilePath(FileName);
            

            if (File.Exists(ToReadPath))
            {
                StreamReader reader = new(ToReadPath);
                string readedLine = "";
                while((readedLine = reader.ReadLine()) != null)
                {
                    if (readedLine.Contains("LastConvertedSensitivity"))
                    {
                        string S_LastConverted = "LastConvertedSensitivity";
                        int Index_NameKey = 0;
                        int Index_LastConverted = 0;
                        StringBuilder GstringBuilder = new();
                        for (int i = 0; i < SettingKeys.Length; i++)
                        {
                            //Debug.Write(SettingKeys[i]);
                            GstringBuilder.Clear();
                            if (SettingKeys[i].Contains("Scope"))
                                GstringBuilder.Append(SettingKeys[i]);
                            GstringBuilder.Append(IDKeys[i]);
                            Index_NameKey = readedLine.IndexOf(SettingKeys[i]);
                            //if (Index_NameKey < 0) Index_NameKey = readedLine.IndexOf(SettingKeys[i]); // Re-search
                            if (Index_NameKey > 0)
                            {
                                Index_LastConverted = readedLine.IndexOf(S_LastConverted, Index_NameKey);
                                GstringBuilder.Append(readedLine.Substring(Index_LastConverted - 10, 9));
                                
                                //Debug.WriteLine(readedLine.Substring(Index_LastConverted - 10, 9));
                            }
                            else GstringBuilder.Append("50.0");
                            ToReturn[i] = GstringBuilder.ToString();
                        }
                        
                        string MouseVerticalSensitivityMultiplierAdjusted = "MouseVerticalSensitivityMultiplierAdjusted";
                        GstringBuilder.Clear();
                        GstringBuilder.Append(IDKeys[IDKeys.Length - 1]);
                        if (readedLine.Contains(MouseVerticalSensitivityMultiplierAdjusted))
                        {
                            string VerticalSen = readedLine.Substring(readedLine.IndexOf(MouseVerticalSensitivityMultiplierAdjusted)
                                + MouseVerticalSensitivityMultiplierAdjusted.Length + 1, 8);                            
                            GstringBuilder.Append(VerticalSen);
                        }
                        else GstringBuilder.Append("1");
                        ToReturn.Add(GstringBuilder.ToString());
                        GstringBuilder.Clear();

                        string bIsUsingPerScopeMouseSensitivity = "bIsUsingPerScopeMouseSensitivity=";
                        int bIsUsingPerScopeMouseSensitivitySize = readedLine.IndexOf(bIsUsingPerScopeMouseSensitivity)
                            + bIsUsingPerScopeMouseSensitivity.Length - readedLine.IndexOf(',', readedLine.IndexOf(bIsUsingPerScopeMouseSensitivity));
                        bIsUsingPerScopeMouseSensitivitySize = Math.Abs(bIsUsingPerScopeMouseSensitivitySize);
                        GstringBuilder.Append(bIsUsingPerScopeMouseSensitivity);
                        if (readedLine.Contains(bIsUsingPerScopeMouseSensitivity))
                        {
                            string s = readedLine.Substring(readedLine.IndexOf(bIsUsingPerScopeMouseSensitivity) 
                                + bIsUsingPerScopeMouseSensitivity.Length, bIsUsingPerScopeMouseSensitivitySize);
                            GstringBuilder.Append(s);
                            ToReturn.Add(GstringBuilder.ToString());
                        }
                        GstringBuilder.Clear();

                        string InvertMouse = "InvertMouse=";
                        int InvertMouseSize = readedLine.IndexOf(InvertMouse) + InvertMouse.Length
                            - readedLine.IndexOf(',', readedLine.IndexOf(InvertMouse));
                        InvertMouseSize = Math.Abs(InvertMouseSize);
                        GstringBuilder.Append(InvertMouse);
                        if (readedLine.Contains(InvertMouse))
                        {
                            string s = readedLine.Substring(readedLine.IndexOf(InvertMouse)
                                + InvertMouse.Length, InvertMouseSize);
                            GstringBuilder.Append(s);
                            ToReturn.Add(GstringBuilder.ToString());
                        }
                        break;
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            return ToReturn;
        }

        public static void SaveProperty(string FileName, List<string> ToSave)
        {
            string FilePath = GetFilePath(FileName);
            if (File.Exists(FilePath))
            {
                int IScopeSaving = 3;                
                StreamReader streamReader = new(FilePath);
                StreamWriter streamWriter = new(FilePath + ".tmp");
                string readedLine;
                while((readedLine = streamReader.ReadLine()) != null)
                {
                    if (readedLine.Contains("LastConvertedSensitivity"))
                    {
                        int IName = 0;
                        foreach (string ToSaveString in ToSave)
                        {
                            if (ToSaveString.Contains("Scope"))
                            {
                                int IScope = readedLine.IndexOf(SettingKeys[IScopeSaving]);
                                if(IScope > -1)
                                {
                                    int ILastConverted = readedLine.IndexOf("LastConvertedSensitivity", IScope) - 10;
                                    readedLine = readedLine.Remove(ILastConverted, 9);
                                    readedLine = readedLine.Insert(ILastConverted,
                                        string.Format("{0:0.000000}", Convert.ToInt32(ToSaveString.Substring("Scope".Length))));
                                    //Debug.WriteLine(string.Format("{0:0.000000}", Convert.ToInt32(ToSaveString.Substring("Scope".Length))));
                                }
                                IScopeSaving++;
                            }
                            else if (ToSaveString.Contains("0x0v"))
                            {
                                string MouseVerticalSensitivityMultiplierAdjusted = "MouseVerticalSensitivityMultiplierAdjusted";
                                int IVertical = readedLine.IndexOf(MouseVerticalSensitivityMultiplierAdjusted);
                                if (IVertical > -1)
                                {
                                    IVertical += MouseVerticalSensitivityMultiplierAdjusted.Length + 1;
                                    readedLine = readedLine.Remove(IVertical, 8);
                                    readedLine = readedLine.Insert(IVertical, ToSaveString.Substring(4));
                                }
                            }
                            else
                            {
                                Debug.WriteLine(ToSaveString);
                                IName = readedLine.IndexOf(SettingKeys[Array.IndexOf(IDKeys, ToSaveString.Substring(0, 4))], IName);
                                if(IName < 0) IName = readedLine.IndexOf(SettingKeys[Array.IndexOf(IDKeys, ToSaveString.Substring(0, 4))]);
                                if (IName > -1)
                                {
                                    int ILastConverted = readedLine.IndexOf(",LastConvertedSensitivity", IName) - 9;
                                    Debug.WriteLine(SettingKeys[Array.IndexOf(IDKeys, ToSaveString.Substring(0, 4))]);
                                    Debug.WriteLine(Array.IndexOf(IDKeys, ToSaveString.Substring(0, 4)));
                                    Debug.WriteLine("IName: " + IName);
                                    Debug.WriteLine("ILast: " + ILastConverted);
                                    Debug.WriteLine(readedLine.Substring(ILastConverted, 9));
                                    readedLine = readedLine.Remove(ILastConverted, 9);
                                    readedLine = readedLine.Insert(ILastConverted, string.Format("{0:0.000000}", Convert.ToInt32(ToSaveString.Substring(4))));

                                    Debug.WriteLine(readedLine.Substring(ILastConverted, 9));
                                    continue;
                                }
                            }
                        }
                    }
                    streamWriter.WriteLine(readedLine);
                }
                Debug.WriteLine("######################");
                streamReader.Close();
                streamWriter.Close();
                streamReader.Dispose();
                streamWriter.Dispose();
                File.Delete(FilePath);
                File.Move(FilePath + ".tmp", FilePath);
            }           
        }
    }
}
