using System;
using System.Collections.Generic;
using System.IO;

namespace PUBG_Profiler
{
    class SavedProperty
    {
        private static readonly string PropName = Environment.CurrentDirectory + @"\SavedProperty.prop";
        private static readonly string[] PropertyKeys = { "LastUse" };
        private static readonly int[] IndexKeys = new int[1];

        public static void SaveProperty(ref List<string> SaveList)
        {
            FileStream ThisPropStream = File.Open(PropName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader ThisPropReader = new(ThisPropStream);
            StreamWriter TmpStreamWriter = new(PropName + ".tmp");

            int nowLine = 0, PassedSave = 0;
            string lineThisProp;
            while((lineThisProp = ThisPropReader.ReadLine()) != null)
            {
                bool useFromOrigin = true;
                for(int i = 0; i < PropertyKeys.Length; i++)
                {
                    if (nowLine == IndexKeys[i])
                    {
                        useFromOrigin = false;
                        TmpStreamWriter.WriteLine(PropertyKeys[i] + "=" + SaveList[i]);
                        PassedSave++;
                        break;
                    }
                }
                if(useFromOrigin) TmpStreamWriter.WriteLine(lineThisProp);
                nowLine++;
            }
            if(PassedSave < PropertyKeys.Length)
                for(int i = 0; i< PropertyKeys.Length; i++)
                    TmpStreamWriter.WriteLine(PropertyKeys[i] + "=" + SaveList[i]);

            ThisPropStream.Close();
            ThisPropReader.Close();
            TmpStreamWriter.Close();
            File.Delete(PropName);
            File.Move(PropName + ".tmp", PropName);
        }

        public static List<string> LoadProperty()
        {
            List<string> ToReturn = new(new string[PropertyKeys.Length]);
            if (File.Exists(PropName))
            {
                StreamReader reader = new(PropName);
                string ReadedLine;
                int CurrentLine = 0;
                while((ReadedLine = reader.ReadLine()) != null)
                {
                    for(int i = 0; i < PropertyKeys.Length; i++)
                    {
                        if (ReadedLine.Contains(PropertyKeys[i]))
                        {
                            IndexKeys[i] = CurrentLine;
                            ToReturn[i] = ReadedLine.Substring(ReadedLine.IndexOf('=') + 1);
                        }
                    }
                    CurrentLine++;
                }
                reader.Close();
            }
            return ToReturn;
        }
    }
}
