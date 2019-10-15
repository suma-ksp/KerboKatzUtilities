using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerboKatz
{
    using System.Reflection;

    public static class TypeExtensions
    {
        public static FieldInfo GetField(this Type type, string fieldName, BindingFlags flags, out string error)
        {
            var result = type.GetField(fieldName, flags);
            if (result != null)
            {
                error = null;
                return result;
            }
            var sb = new StringBuilder();
            sb.AppendLine($"Field {fieldName} not found on type {type.FullName}, availableFields:");
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                sb.AppendLine($"public-instance: {field.Name}");
            }
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                sb.AppendLine($"nonpublic-instance: {field.Name}");
            }
            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                sb.AppendLine($"public-static: {field.Name}");
            }
            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.NonPublic))
            {
                sb.AppendLine($"nonpublic-static: {field.Name}");
            }
            error = sb.ToString();
            return null;
        }
        public static PropertyInfo GetProperty(this Type type, string fieldName, BindingFlags flags, out string error)
        {
            var result = type.GetProperty(fieldName, flags);
            if (result != null)
            {
                error = null;
                return result;
            }
            var sb = new StringBuilder();
            sb.AppendLine($"Field {fieldName} not found on type {type.FullName}, availableFields:");
            foreach (var field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                sb.AppendLine($"public-instance: {field.Name}");
            }
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                sb.AppendLine($"nonpublic-instance: {field.Name}");
            }
            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                sb.AppendLine($"public-static: {field.Name}");
            }
            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.NonPublic))
            {
                sb.AppendLine($"nonpublic-static: {field.Name}");
            }
            error = sb.ToString();
            return null;
        }
    }
}
