using KSP.UI.Screens;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace KerboKatz
{
    using ReflectionWrapper;

    public static partial class SmallExtensions
    {
        public static bool IsTypeOf(this Type type, Type typeOf)
        {
            if (type == typeOf)
                return true;
            return type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeOf);
        }

        public static bool HasData(this IScienceDataContainer container, ScienceData data)
        {
            return new ScienceDataContainerWrapper(container).HasData(data);
        }

        public static bool HasData(this ScienceDataContainerWrapper container, ScienceData data)
        {
            var dataStore = container.GetData(); ;
            var scienceCount = dataStore.Length;
            for (var i = 0; i < scienceCount; i++)
            {
                if (dataStore[i].subjectID == data.subjectID)
                {
                    return true;
                }
            }
            return false;
        }

        public static void StoreData(this IScienceDataContainer container, IScienceDataContainer experiment, bool dumpDuplicates, ILogHandler logHandler)
        {
            if (logHandler == null)
            {
                logHandler = new ScreenMessageLogHandler();
            }
            if (container == experiment)
            {
                logHandler.Log("Container is the same as experiment!");
                return;
            }

            var containerWrapper = new ScienceDataContainerWrapper(container);
            var experimentWrapper = new ScienceDataContainerWrapper(experiment);
            var containerData = containerWrapper.GetData();
            var experimentData = experimentWrapper.GetData();
            if (experimentData.Length == 0)
            {
                logHandler.Log($"GetData returned no scienceData");
            }
            for (var i = 0; i < experimentData.Length; i++)
            {
                var currentData = experimentData[i];
                logHandler.Log($"Processing {i + 1}/{experimentData.Length}: {currentData.subjectID} ({currentData.title})");
                if (!containerWrapper.HasData(currentData))
                {
                    containerWrapper.ReturnData(currentData);
                    if (containerWrapper.HasData(currentData))
                    {
                        experimentWrapper.DumpData(currentData);
                        logHandler.Log($"{currentData.dataAmount} data stored");
                    }
                    else
                        logHandler.Log($"Container didn't store the experiment");
                }
                else if (dumpDuplicates)
                {
                    logHandler.Log($"{currentData.subjectID} - {currentData.title}: already contains -> dump");
                    experimentWrapper.DumpData(currentData);
                }
            }
        }

        public static void StoreData(this IScienceDataContainer container, IScienceDataContainer experiment, bool dumpDuplicates)
        {
            StoreData(container, experiment, dumpDuplicates, null);
        }

        public static string NormalizePath(this DirectoryInfo path)
        {
            return path.FullName.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        public static void setToZero(this RectOffset offset)
        {
            offset.left = 0;
            offset.right = 0;
            offset.bottom = 0;
            offset.top = 0;
        }

        public static void Clear(this StringBuilder sb)
        {
            sb.Length = 0;
            //sb.Remove(0, sb.Length);
        }

        /**
         * since .net 3.5 doesn't have has flag and i needed something like that i implemented this
         */

        public static bool HasFlag(this ApplicationLauncher.AppScenes source, ApplicationLauncher.AppScenes flag)
        {
            return (source & flag) == flag;
        }

        public static Rect moveToCursor(this Rect rect, float offsetX = 10, float offsetY = 10)
        {
            rect.x = Input.mousePosition.x + offsetX;
            rect.y = Screen.height - Input.mousePosition.y + offsetY;
            return rect;
        }

        public static Vector3 clampTo(this Vector3 current, Vector3 min, Vector3 max)
        {
            if (current.x > max.x)
            {
                current.x = max.x;
            }
            else if (current.x < min.x)
            {
                current.x = min.x;
            }

            if (current.y > max.y)
            {
                current.y = max.y;
            }
            else if (current.y < min.y)
            {
                current.y = min.y;
            }

            if (current.z > max.z)
            {
                current.z = max.z;
            }
            else if (current.z < min.z)
            {
                current.z = min.z;
            }
            return current;
        }

        /*
        private static Dictionary<KeyBinding, KeyBindingStorage> KeyBindingStorage = new Dictionary<KeyBinding, KeyBindingStorage>();

        public static void saveDefault(this KeyBinding KeyBinding)
        {
          if (!KeyBindingStorage.ContainsKey(KeyBinding))
            KeyBindingStorage.Add(KeyBinding, new KeyBindingStorage(KeyBinding.primary, KeyBinding.secondary));
        }

        public static void setNone(this KeyBinding KeyBinding)
        {
          KeyBinding.primary = KeyCode.None;
          KeyBinding.secondary = KeyCode.None;
        }

        public static void reset(this KeyBinding KeyBinding)
        {
          if (KeyBindingStorage.ContainsKey(KeyBinding))
          {
            KeyBinding.primary = KeyBindingStorage[KeyBinding].primary;
            KeyBinding.secondary = KeyBindingStorage[KeyBinding].secondary;
          }
        }

        public static KeyCode getDefaultPrimary(this KeyBinding KeyBinding)
        {
          if (KeyBindingStorage.ContainsKey(KeyBinding))
          {
            return KeyBindingStorage[KeyBinding].primary;
          }
          return KeyCode.None;
        }

        private static Dictionary<AxisBinding, AxisBindingStorage> AxisBindingStorage = new Dictionary<AxisBinding, AxisBindingStorage>();

        public static void saveDefault(this AxisBinding AxisBinding)
        {
          if (!AxisBindingStorage.ContainsKey(AxisBinding))
            AxisBindingStorage.Add(AxisBinding, new AxisBindingStorage(AxisBinding.primary.scale, AxisBinding.secondary.scale));
        }

        public static void setZero(this AxisBinding AxisBinding)
        {
          AxisBinding.primary.scale = 0;
          AxisBinding.secondary.scale = 0;
        }

        public static void reset(this AxisBinding AxisBinding)
        {
          if (AxisBindingStorage.ContainsKey(AxisBinding))
          {
            AxisBinding.primary.scale = AxisBindingStorage[AxisBinding].primaryScale;
            AxisBinding.secondary.scale = AxisBindingStorage[AxisBinding].secondaryScale;
          }
        }*/
    }
}