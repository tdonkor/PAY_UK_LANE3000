/*******************************************************************************
NAME             :  KPOSLibrary.IniFile
DESCRIPTION      :  Read Write ini file
                    All function is similary with TiniFile class from delphi
AUTHOR           :  DSO
HISTORY          :
 1.0 - first version
 * 
Usage example
 
    private string GetIniFullFilename()
    {
        string res = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "KDOR.net.ini");
        return res;
    }
 
    int refInt;
 
    //save value to ini file
    IniFile inif = new IniFile(GetIniFullFilename()); 
    inif.WriteInteger("PERSISTENCE", "LastRefint", refInt);

    //read value from ini file
    IniFile inif = new IniFile(GetIniFullFilename());
    refInt = inif.ReadInteger("PERSISTENCE", "LastRefint", 1);
    
 
*******************************************************************************/
using Acrelec.Mockingbird.Interfaces.Peripherals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Acrelec.Mockingbird.Payment_UK_IPP350
{
    public static class IniFilesSimple
    {
        public static string ReadString(string key, string defString, string fileName, ILogger logger = null)
        {
            try
            {
                List<string> iniFileLines = File.ReadAllLines(fileName).ToList();

                foreach (string line in iniFileLines)
                {
                    string trimedLine = line.Trim();

                    //Skip commented lines
                    if (trimedLine.StartsWith(";") || trimedLine.StartsWith("#"))
                        continue;

                    //Get delimiter position

                    int delimiterPosition = -1;
                    delimiterPosition = trimedLine.IndexOf('=');

                    //Skip lines with no delimiter
                    if (delimiterPosition == -1)
                        continue;

                    string parameter = trimedLine.Substring(0, delimiterPosition).Trim();

                    //Skip parameters with spaces in their names
                    if (parameter.Contains(' '))
                        continue;

                    if(parameter == key)
                        return trimedLine.Substring(delimiterPosition + 1, trimedLine.Length - delimiterPosition - 1);
                }
            }
            catch (Exception ex) 
            {
                if (logger != null)
                {
                    logger.Error("Payment_UK_IPP350", string.Format("ReadString : Failed to read payment setting.\r\n{0}", ex.Message));
                }
            }

            return defString;
        }

        public static bool WriteValue(string key, string value, string fileName, ref string exceptionMessage)
        {
            try
            {
                List<string> iniFileLines = File.ReadAllLines(fileName).ToList();

                for (int i = 0; i < iniFileLines.Count; i++)
                {
                    string trimedLine = iniFileLines[i].Trim();

                    //Skip commented lines
                    if (trimedLine.StartsWith(";") || trimedLine.StartsWith("#"))
                        continue;

                    //Get delimiter position

                    int delimiterPosition = -1;
                    delimiterPosition = trimedLine.IndexOf('=');

                    //Skip lines with no delimiter
                    if (delimiterPosition == -1)
                        continue;

                    string parameter = trimedLine.Substring(0, delimiterPosition).Trim();

                    //Skip parameters with spaces in their names
                    if (parameter.Contains(' '))
                        continue;

                    if (parameter == key)
                    {
                        iniFileLines[i] = string.Format("{0}={1}", parameter, value);

                        File.WriteAllLines(fileName, iniFileLines);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionMessage = $"{ex.ToString()} ({fileName})";
                return false;
            }

            exceptionMessage = $"key was not found ({fileName})";

            return false;
        }
    }
}
