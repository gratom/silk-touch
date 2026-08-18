using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Universal module for saving and loading any text information
    /// </summary>
    public static class SaverLoaderModule
    {
        #region for window saves in application folder

        //uncomment line below
        //public static string StandartSavesPath => Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/Standart_Saves";

        #endregion for window saves in application folder

        #region for universal saves

        public static string StandardSavesPath => Application.persistentDataPath + "/Standart_Saves";

        #endregion for universal saves

        #region public functions

        /// <summary>
        /// Saves data to a file with the specified name. Remember that this function saves the file in a standard folder.
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <param name="stringData">Your string data</param>
        public static void SaveMyDataToFile(string fileName, string stringData)
        {
            try
            {
                fileName = Normalizer(fileName);
                OverGeneratePath(StandardSavesPath + fileName);
                FileStream fileStream = new FileStream(StandardSavesPath + fileName, FileMode.Create);
                StreamWriter myFile = new StreamWriter(fileStream);
                myFile.Write(stringData);
                myFile.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Load string data from file. Remember that this function loads the file from the standard folder.
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <returns>String data or "", if file don`t exist</returns>
        public static string LoadMyDataFromFile(string fileName)
        {
            try
            {
                fileName = Normalizer(fileName);
                if (File.Exists(StandardSavesPath + fileName))
                {
                    StreamReader myFile = new StreamReader(StandardSavesPath + fileName);
                    string DString = myFile.ReadToEnd();
                    myFile.Close();
                    return DString;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return "";
        }

        /// <summary>
        /// Saves data to a file with the specified name. Remember that in this function you must specify the full path to the file.
        /// </summary>
        /// <param name="fullPath">Full path to file</param>
        /// <param name="stringData">Your string data</param>
        public static void SaveMyDataTo(string fullPath, string stringData)
        {
            try
            {
                OverGeneratePath(fullPath);
                FileStream fileStream = new FileStream(fullPath, FileMode.Create);
                StreamWriter MyFile = new StreamWriter(fileStream);
                MyFile.Write(stringData);
                MyFile.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Load string data from file. Remember that in this function you must specify the full path to the file.
        /// </summary>
        /// <param name="fullPath">Full path to file</param>
        /// <returns>String data or "", if file don`t exist</returns>
        public static string LoadMyDataFrom(string fullPath)
        {
            try
            {
                if (File.Exists(fullPath))
                {
                    StreamReader MyFile = new StreamReader(fullPath);
                    string DString = MyFile.ReadToEnd();
                    MyFile.Close();
                    return DString;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return "";
        }

        public static string OverGeneratePath(string fileName)
        {
            List<string> finalListOfPathParts = NormalizePath(fileName);

            string returnedPath = "";
            for (int i = 0; i < finalListOfPathParts.Count - 1; i++)
            {
                returnedPath += finalListOfPathParts[i] + "/";
                if (!Directory.Exists(returnedPath))
                {
                    try
                    {
                        Directory.CreateDirectory(returnedPath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }

            returnedPath += finalListOfPathParts[finalListOfPathParts.Count - 1];
            return returnedPath;
        }

        private static List<string> NormalizePath(string fileName)
        {
            string[] tempArray = fileName.Split('/');
            return tempArray.Where(t => t != "").ToList();
        }

        #endregion public functions

        #region private functions

        private static string Normalizer(string fileName)
        {
            if (fileName.IndexOf('/') == 0)
            {
                return fileName;
            }
            else
            {
                return "/" + fileName;
            }
        }

        #endregion private functions
    }

}