using System;
using System.Linq;
using NModbus.Extensions.Functions;
using Stormbus.UI.Containers;

namespace Stormbus.UI.Converters
{
    public static class ModbusDataTypesConverter
    {
        public static float ConvertToFloat(ushort[] input, EndianType registersEndian, EndianType bytesEndian)
        {
            var bytes = ConvertToByteString(input, registersEndian, bytesEndian);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static double ConvertToDouble(ushort[] input, EndianType registersEndian, EndianType bytesEndian)
        {
            var bytes = ConvertToByteString(input, registersEndian, bytesEndian);
            return BitConverter.ToDouble(bytes, 0);
        }

        public static short ConvertToShort(ushort[] input, EndianType registersEndian, EndianType bytesEndian)
        {
            var bytes = ConvertToByteString(input, registersEndian, bytesEndian);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static ushort ConvertToUShort(ushort[] input, EndianType registersEndian, EndianType bytesEndian)
        {
            var bytes = ConvertToByteString(input, registersEndian, bytesEndian);
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static int ConvertToInt(ushort[] input, EndianType registersEndian, EndianType bytesEndian)
        {
            var bytes = ConvertToByteString(input, registersEndian, bytesEndian);
            return BitConverter.ToInt32(bytes, 0);
        }

        public static uint ConvertToUInt(ushort[] input, EndianType registersEndian, EndianType bytesEndian)
        {
            var bytes = ConvertToByteString(input, registersEndian, bytesEndian);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static long ConvertToLong(ushort[] input, EndianType registersEndian, EndianType bytesEndian)
        {
            var bytes = ConvertToByteString(input, registersEndian, bytesEndian);
            return BitConverter.ToInt64(bytes, 0);
        }

        public static ulong ConvertToULong(ushort[] input, EndianType registersEndian, EndianType bytesEndian)
        {
            var bytes = ConvertToByteString(input, registersEndian, bytesEndian);
            return BitConverter.ToUInt64(bytes, 0);
        }

        private static byte[] ConvertToByteString(ushort[] input, EndianType registersEndian, EndianType bytesEndian)
        {
            //if (registersEndian == EndianType.LittleEndian) input = input.Reverse().ToArray();

            var bytes = new byte[input.Length * 2];
            var j = 0;
            for (var i = 0; i < input.Length; i++)
            {
                var buffer = ParseUshortToBytes(input[input.Length - 1 - i], bytesEndian);
                bytes[j] = buffer[0];
                bytes[j + 1] = buffer[1];
                j += 2;
            }

            return bytes;
        }

        private static byte[] ParseUshortToBytes(ushort buffer, EndianType endian)
        {
            var bytes = new byte[2];
            bytes[0] = Convert.ToByte(buffer >> 8);
            bytes[1] = Convert.ToByte(buffer & 0xFF);
            return endian == EndianType.BigEndian ? Endian.BigEndian(bytes) : Endian.LittleEndian(bytes);
        }
    }
}