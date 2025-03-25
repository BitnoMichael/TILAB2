using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace TI2
{
    internal class KeyGenerator
    {
        byte[] bits;
        public int KeyLength { get => bits.Length; }
        public static KeyGenerator ParseKeyGenerator(string str)
        {
            if (str == null)
                return null;
            var bits = new List<byte>();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '0')
                    bits.Add(0);
                else if (str[i] == '1')
                    bits.Add(1);
            }
            return new KeyGenerator(bits.ToArray());
        }

        static private Dictionary<int, HashSet<int>> _xorBitsForKeyLengths = new Dictionary<int, HashSet<int>>
        {
            { 24, new HashSet<int>{0, 2, 3, 23 } }
        };
        public KeyGenerator(byte[] keyBits)
        {
            if (!_xorBitsForKeyLengths.ContainsKey(keyBits.Length))
                throw new ArgumentException("Can not handle key with this length");
            this.bits = keyBits;
            for (int i = 0; i < bits.Length; i++)
            {
                bits[i] = (byte)(bits[i] & 0b00000001);
            }
        }
        byte shiftKeyBits()
        {
            var result = bits[bits.Length - 1];
            for (int i = bits.Length - 1; i > 0; i--)
            {
                bits[i] = bits[i - 1];
            }
            bits[0] = 0;
            return result;
        }
        public byte GetNextKeyBit()
        {
            HashSet<int> bitsToXor;
            if (!_xorBitsForKeyLengths.TryGetValue(bits.Length, out bitsToXor))
                throw new ArgumentException("Can not handle key with this length");

            byte newFirstBit = 0;
            foreach (int index in bitsToXor)
                newFirstBit ^= bits[index];

            var result = shiftKeyBits();
            bits[0] = newFirstBit;
            return result;
        }
        public byte GetNextKeyByte()
        {
            byte result = 0b00000000;
            for (int i = 0; i < 7; i ++)
            {
                result ^= GetNextKeyBit();
                result = (byte)(result << 1);
            }
            result ^= GetNextKeyBit();
            return result;
        }

    }
}
