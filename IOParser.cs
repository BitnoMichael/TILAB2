using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI2
{
    internal class IOParser
    {
        public static string BytesToShortenedString(byte[] data, int registerLength)
        {
            var numOfBytesInRegister = (int) Math.Ceiling((double)registerLength / 8);
            var numOfBytesToShow =  numOfBytesInRegister * 4;
            var needSeparator = numOfBytesToShow < data.Length;
            StringBuilder sb = new StringBuilder((data.Length * 9 + 4) % (registerLength * 4 + 4));
            int i = 0;
            while (i < data.Length)
            {
                sb
                    .Append(Convert.ToString(data[i], 2).PadLeft(8, '0'))
                    .Append(" ");
                if (needSeparator && i == numOfBytesInRegister * 2 - 1)
                {
                    sb.Append("... ");
                    i = data.Length - 2 * numOfBytesInRegister;
                }
                else
                    i++;
            }
            return sb.ToString();
        }
        public static byte[] ParseByteArray(string str)
        {
            // Удаляем все символы, кроме '0' и '1'
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '1' || str[i] == '0')
                    sb.Append(str[i]);
            }
            string fixedStr = sb.ToString();

            // Если строка пустая, возвращаем пустой массив
            if (fixedStr.Length == 0)
                return Array.Empty<byte>();

            // Вычисляем количество байтов (округляем вверх)
            int byteCount = (int)Math.Ceiling(fixedStr.Length / 8.0);
            byte[] result = new byte[byteCount];

            // Заполняем массив байтов
            for (int i = 0; i < fixedStr.Length; i++)
            {
                int byteIndex = i / 8;
                int bitPosition = 7 - (i % 8); // Старший бит слева
                if (fixedStr[i] == '1')
                {
                    result[byteIndex] |= (byte)(1 << bitPosition);
                }
            }

            return result;
        }
    }
}
