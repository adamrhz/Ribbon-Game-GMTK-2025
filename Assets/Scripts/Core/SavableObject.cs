
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using Unity.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ribbon
{

    [AttributeUsage(AttributeTargets.Field)]
    public class SavableAttribute : Attribute
    {
        public object DefaultValue { get; }

        public SavableAttribute() { }

        public SavableAttribute(bool defaultValue) => DefaultValue = defaultValue;
        public SavableAttribute(int defaultValue) => DefaultValue = defaultValue;
        public SavableAttribute(float defaultValue) => DefaultValue = defaultValue;
        public SavableAttribute(string defaultValue) => DefaultValue = defaultValue;

        public SavableAttribute(bool[] defaultValues) => DefaultValue = defaultValues;
        public SavableAttribute(int[] defaultValues) => DefaultValue = defaultValues;
        public SavableAttribute(string[] defaultValues) => DefaultValue = defaultValues;
    }

    public class SavableObject : ScriptableObject
    {

        [NaughtyAttributes.ReadOnly][Savable] public string UID;
        private void OnValidate()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(UID))
            {
                UID = GUID.Generate().ToString();
                EditorUtility.SetDirty(this);
            }
#endif
        }

        public Dictionary<string, object> GetSavableFields()
        {
            var savableFields = new Dictionary<string, object>();

            Type type = GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (Attribute.IsDefined(field, typeof(SavableAttribute)))
                {
                    object value = field.GetValue(this);
                    savableFields.Add(field.Name, value);
                }
            }

            return savableFields;
        }


        public void SetSavableFields(Dictionary<string, object> savedFields)
        {
            if (savedFields == null) return;

            Type type = GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                if (Attribute.IsDefined(field, typeof(SavableAttribute)) && savedFields.TryGetValue(field.Name, out object savedValue))
                {
                    try
                    {
                        if (savedValue == null && (!field.FieldType.IsValueType || Nullable.GetUnderlyingType(field.FieldType) != null))
                        {
                            field.SetValue(this, null);
                        }
                        else if (field.FieldType.IsArray && savedValue is IEnumerable<object> objArray)
                        {
                            Type elementType = field.FieldType.GetElementType();
                            var list = new List<object>(objArray);
                            Array typedArray = Array.CreateInstance(elementType, list.Count);

                            for (int i = 0; i < list.Count; i++)
                            {
                                object item = Convert.ChangeType(list[i], elementType, CultureInfo.InvariantCulture);
                                typedArray.SetValue(item, i);
                            }

                            field.SetValue(this, typedArray);
                        }
                        else
                        {
                            object convertedValue = Convert.ChangeType(savedValue, field.FieldType, CultureInfo.InvariantCulture);
                            field.SetValue(this, convertedValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to set field '{field.Name}': {ex.Message}");
                    }
                }
            }
        }

        public virtual void ResetSavable()
        {
            // Override this method in derived classes to reset specific fields
            // Example: unlocked = false
        }

    }


}