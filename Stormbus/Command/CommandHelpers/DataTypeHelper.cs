using System;
using Stormbus.UI.Enums;

namespace Stormbus.UI.Command.CommandHelpers
{
    public static class DataTypeHelper
    {
        public static object GetDefaultValue(DataType dataType)
        {
            object result = null;
            switch (dataType)
            {
                case DataType.Double:
                    result = default(double);
                    break;
                case DataType.Float:
                    result = default(float);
                    break;
                case DataType.Int:
                    result = default(int);
                    break;
                case DataType.Long:
                    result = default(long);
                    break;
                case DataType.Short:
                    result = default(short);
                    break;
                case DataType.UInt:
                    result = default(uint);
                    break;
                case DataType.UShort:
                    result = default(ushort);
                    break;
            }

            return result;
        }

        public static object ConvertToType(Type targetType, string source)
        {
            if (targetType == typeof(double) && double.TryParse(source, out var doubleVal))
                return doubleVal;
            if (targetType == typeof(float) && float.TryParse(source, out var floatVal))
                return floatVal;
            if (targetType == typeof(int) && int.TryParse(source, out var intVal))
                return intVal;
            if (targetType == typeof(long) && long.TryParse(source, out var longVal))
                return longVal;
            if (targetType == typeof(short) && short.TryParse(source, out var shortVal))
                return shortVal;
            if (targetType == typeof(uint) && uint.TryParse(source, out var uintVal))
                return uintVal;
            if (targetType == typeof(ushort) && ushort.TryParse(source, out var ushortVal))
                return ushortVal;
            throw new ArgumentException(@"Unsupported type");
        }
    }
}