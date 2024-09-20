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
            return input[0] switch
            {
                var c when Char.IsDigit(c) => DecodeString(input),
                'i' => DecodeInt(input),
                'l' => DecodeList(input),
                'd' => DecodeDictionary(input),
                _ => throw new InvalidOperationException($"Could not decode {input}")
            };
        }

        public static string Encode(object input)
        {
            return input switch
            {
                long n => $"i{n}e",

                string s => $"{s.Length}:{s}",

                object[] arr => $"l{string.Join("", arr.Select(Encode))}e",

                object obj => EncodeNonPrimitiveType(input),

                _ => throw new Exception($"Unknown type: {input.GetType().FullName}")
            };
        }

        private static string EncodeNonPrimitiveType(object input) 
        {
            if (input is object[] inputArray)
            {
                return $"l{string.Join("", inputArray.Select(x => Encode(x)))}e";
            }
            else if (input is Dictionary<string, object> inputDictionary)
            {
                return $"d{string.Join("", inputDictionary.Values.Select(x => Encode(x)))}e";
            }
            else
            {
                throw new Exception($"Unknown type: {input.GetType().FullName}");
            }
        }

        private static string DecodeString(string input)
        {
            int colonIndex = input.IndexOf(':');

            if (colonIndex != -1)
            {
                var strlen = int.Parse(input[..colonIndex]);

                return input.Substring(colonIndex + 1, strlen);
            }
            else
            {
                throw new InvalidOperationException("Invalid encoded value: " + input);
            }
        }

        private static long DecodeInt(string input) => long.Parse(input[1..input.IndexOf('e')]); // EX i52e

        private static object[] DecodeList(string input) // EX: l5:helloi52ee
        {
            //trim 'l'
            input = input[1..];

            var result = new List<object>();

            //iterate while trimming input. ends when we hit 'e' which should be end...
            while (input.Length > 0 && input[0] != 'e')
            {
                var element = Decode(input);

                result.Add(element);

                input = input[Encode(element).Length..];
            }
            
            return [.. result];
        }

        private static Dictionary<string, object> DecodeDictionary(string input) // EX d<key1><value1>...<keyN><valueN>e
        {
            // key is always string, value could be anything...
            //trim d
            input = input[1..];

            var result = new Dictionary<string, object>();

            while(input.Length > 0 && input[0] != 'e')
            {
                var key = DecodeString(input);

                input = input[Encode(key).Length..];

                var value = Decode(input);

                result.Add(key, value);

                input = input[Encode(key).Length..];
            }

            return result;
        }
    }
}
