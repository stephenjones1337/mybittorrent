using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codecrafters_bittorrent.src
{
    public class Bencode
    {
        public static object Decode(string input)
        {
            var index = 0;
            return DecodeBencode(input, ref index);
        }

        private static object DecodeBencode(string input, ref int index)
        {
            return input[index] switch
            {
                var c when Char.IsDigit(c) => DecodeString(input, ref index),
                'i' => DecodeInt(input, ref index),
                'l' => DecodeList(input, ref index),
                'd' => DecodeDictionary(input, ref index),
                _ => throw new InvalidOperationException($"Could not decode {input} - char: {input[index]}")
            };
        }

        private static string DecodeString(string input, ref int index) // EX: 5:hello
        {
            int colonIndex = input.IndexOf(':', index);

            if (colonIndex != -1)
            {
                var strlen = int.Parse(input[index..colonIndex]);

                index = colonIndex + 1;

                var strValue = input.Substring(index, strlen);

                index += strlen;

                return strValue;
            }
            else
            {
                throw new InvalidOperationException("Invalid encoded value: " + input);
            }
        }

        private static long DecodeInt(string input, ref int index) // EX i52e
        {
            index++; //skipping i
            int endIndex = input.IndexOf("e", index);
            string numStr = input.Substring(index, endIndex - index);
            index = endIndex + 1;
            return long.Parse(numStr); 
        }

        private static object[] DecodeList(string input, ref int index) // EX: l5:helloi52ee
        {
            var result = new List<object>();
            
            index++; //skipping l


            //iterate while trimming input. ends when we hit 'e' which should be end...
            while (input[index] != 'e')
            {
                result.Add(DecodeBencode(input, ref index));
            }

            index++; //skipping e
            
            return [.. result];
        }

        private static Dictionary<string, object> DecodeDictionary(string input, ref int index) // EX d<key1><value1>...<keyN><valueN>e
        {
            // key is always string, value could be anything...
            var result = new Dictionary<string, object>();

            index++; //skipping d


            while(input[index] != 'e')
            {
                var key = DecodeString(input, ref index);

                var value = DecodeBencode(input, ref index);

                result.Add(key, value);

            }

            return result;
        }
    }
}
