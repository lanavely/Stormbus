using System;
using Stormbus.UI.Configuration;
using Stormbus.UI.Enums;

namespace Stormbus.UI.Converters
{
    public static class ModbusDataTypesConverter
    {
        private static void NormalizeByteString(byte[] byteString)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(byteString);
        }

        #region ConvertFromRegisters

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
            if (registersEndian == EndianType.BigEndian)
                Array.Reverse(input);

            var bytes = new byte[input.Length * 2];
            var j = 0;
            foreach (var item in input)
            {
                var buffer = ParseUshortToBytes(item, bytesEndian);
                bytes[j] = buffer[0];
                bytes[j + 1] = buffer[1];
                j += 2;
            }

            NormalizeByteString(bytes);
            return bytes;
        }

        private static byte[] ParseUshortToBytes(ushort buffer, EndianType endian)
        {
            var bytes = BitConverter.GetBytes(buffer);
            NormalizeByteString(bytes);

            if (endian == EndianType.BigEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        #endregion

        #region ConvertBackToRegisters

        public static ushort[] ConvertToRegisters(object value, ConfigurationSettingsModel settings)
        {
            var byteString = value switch
            {
                ushort ushort32Bytes => BitConverter.GetBytes(ushort32Bytes),
                short short32Bytes => BitConverter.GetBytes(short32Bytes),
                uint uint32Bytes => BitConverter.GetBytes(uint32Bytes),
                int int32Bytes => BitConverter.GetBytes(int32Bytes),
                long int64Bytes => BitConverter.GetBytes(int64Bytes),
                float floatBytes => BitConverter.GetBytes(floatBytes),
                double doubleBytes => BitConverter.GetBytes(doubleBytes),
                _ => throw new ArgumentException(@"Unsupported argument")
            };
            NormalizeByteString(byteString);

            if (settings.BytesEndian == EndianType.LittleEndian)
                Array.Reverse(byteString);

            var registers = ConvertByteStringToUshortString(byteString);

            if (settings.RegistersEndian == EndianType.LittleEndian)
                Array.Reverse(registers);

            return registers;
        }


        public static ushort[] ConvertByteStringToUshortString(byte[] byteString)
        {
            var result = new ushort[byteString.Length / 2];
            var k = 0;
            for (var i = 0; i < byteString.Length / 2; i += 2)
            {
                result[k] = Convert.ToUInt16((Convert.ToUInt16(byteString[i]) << 8) + byteString[i + 1]);
                k++;
            }

            return result;
        }

        #endregion
    }
}