using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Ribbon
{

    public class SaveFileManager : MonoBehaviour
    {
        public static Dictionary<string, SavableObject> Saves = new Dictionary<string, SavableObject>();

        public static bool SaveLoaded = false;
        public static string path;


        [SerializeField] List<SavableObject> AllFileToSave = new List<SavableObject>();



        public static SaveFileManager Instance;

        private void Awake()
        {
            Instance = this;
            InitializeSavePath();

        }

        public void LoadSave()
        {

            ClearSave();
            InitializeDictionary(ref Saves);
        }

        public static void SetSaveFilePath(string newpath)
        {
            path = newpath;

            string extension = ".sav";
            if (!Path.HasExtension(path))
            {
                path += extension;
            }
        }
        public static void InitializeSavePath()
        {
            path = $"{Application.persistentDataPath}/RibbonData/Saves/Save_.sav";
        }

        public static bool FileHasBeenLoaded()
        {
            if (!SaveLoaded)
            {
                InitializeSavePath();
                Debug.Log("Save wasnt loaded. Not saving over it.");
                Debug.Log("Loading file instead");
                LoadToFile();
            }


            return SaveLoaded;
        }

        public void ClearSave()
        {
            SaveLoaded = false;
            Saves.Clear();
        }
        public void InitializeStaticDictionnary()
        {
            if (SaveLoaded)
            {
                return;
            }
            InitializeDictionary(ref Saves);
            if (!File.Exists(path))
            {
                ResetSavable();
                SaveToFile(Saves);
            }
            LoadToFile();
            SaveLoaded = true;
        }
        public void InitializeDictionary(ref Dictionary<string, SavableObject> dict)
        {
            Saves?.Clear();
            foreach (SavableObject item in AllFileToSave)
            {
                dict.Add(item.UID, item);
            }
        }


        public static void SaveCompleteFile()
        {
            if (!SaveLoaded)
            {
                InitializeSavePath();
                Debug.Log("Save wasnt loaded. Not saving over it.");
                Debug.Log("Loading file instead");

                LoadToFile();
                return;
            }

            SaveToFile(Saves);
        }
        private static void SaveToFile(Dictionary<string, SavableObject> dict)
        {
            Dictionary<string, Dictionary<string, object>> JSONifiedDict = new Dictionary<string, Dictionary<string, object>>();

            foreach (KeyValuePair<string, SavableObject> saveFile in dict)
            {

                string id = saveFile.Key;
                Dictionary<string, object> SavableFields = saveFile.Value.GetSavableFields();

                string debugOutput = $"ID: {id}\n";
                foreach (var kvp in SavableFields)
                {
                    debugOutput += $"  {kvp.Key}: {kvp.Value}\n";
                }

                JSONifiedDict.Add(id, SavableFields);
            }
            SavableUtility.SaveAll(JSONifiedDict, path);

        }
        public static void LoadToFile()
        {
            ResetSavable();
            string json = RibbonFileWriter.ReadFile(path);
            var saveData = new Dictionary<string, Dictionary<string, object>>();
            try
            {
                saveData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);

            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading save file: {e.Message}");
                Debug.LogError($"Save File Corrupted. Resetting to default values.");
                return;
            }
            foreach (SavableObject save in Saves.Values)
            {
                ResetFieldsToDefault(save);
            }

            foreach (KeyValuePair<string, Dictionary<string, object>> kvp in saveData)
            {
                string uid = kvp.Key;
                Dictionary<string, object> data = kvp.Value;
                string debugOutput = $"ID: {uid}\n";
                foreach (var kvp2 in data)
                {
                    debugOutput += $"  {kvp2.Key}: {kvp2.Value}\n";
                }
                Saves[uid]?.SetSavableFields(data);
            }

        }

        public static void ResetFieldsToDefault(SavableObject obj)
        {
            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                var attr = (SavableAttribute)Attribute.GetCustomAttribute(field, typeof(SavableAttribute));
                if (attr != null && attr.DefaultValue != null)
                {
                    Type defaultType = attr.DefaultValue.GetType();
                    Type fieldType = field.FieldType;

                    bool canAssign = false;

                    if (fieldType.IsAssignableFrom(defaultType))
                    {
                        canAssign = true;
                    }
                    else if (fieldType.IsArray && defaultType.IsArray)
                    {
                        Type fieldElementType = fieldType.GetElementType();
                        Type defaultElementType = defaultType.GetElementType();

                        if (fieldElementType != null && defaultElementType != null &&
                            fieldElementType.IsAssignableFrom(defaultElementType))
                        {
                            canAssign = true;
                        }
                    }

                    if (canAssign)
                    {
                        field.SetValue(obj, attr.DefaultValue);
                    }
                    else
                    {
                        Debug.LogWarning($"DefaultValue {attr.DefaultValue} type {defaultType} does not match field type {fieldType} for field {field.Name}");
                    }
                }
            }
        }

        public static void ResetSavable()
        {
            foreach (var savableObject in Saves.Values)
            {
                if (savableObject != null)
                {
                    savableObject.ResetSavable();
                }
            }
        }
    }


    public class SavableUtility : MonoBehaviour
    {
        public static void SaveAll(Dictionary<string, Dictionary<string, object>> savableObjects, string path)
        {
            string json = JsonConvert.SerializeObject(savableObjects, Formatting.Indented);
            RibbonFileWriter.WriteFile(path, json);
        }

    }
}