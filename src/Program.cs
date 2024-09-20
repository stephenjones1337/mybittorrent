using System.Runtime.CompilerServices;
using System.Text.Json;

// Parse arguments
var (command, param) = args.Length switch
{
    0 => throw new InvalidOperationException("Usage: your_bittorrent.sh <command> <param>"),
    1 => throw new InvalidOperationException("Usage: your_bittorrent.sh <command> <param>"),
    _ => (args[0], args[1])
};

// Parse command and act accordingly
if (command == "decode")
{
    // You can use print statements as follows for debugging, they'll be visible when running tests.
    // Console.WriteLine("Logs from your program will appear here!");

    // Uncomment this line to pass the first stage
    var encodedValue = param;
    if (Char.IsDigit(encodedValue[0]))
    {
        // Example: "5:hello" -> "hello"
        Console.WriteLine(DecodeString(encodedValue, out int strLength));
    }
    else if (Char.IsAsciiLetter(encodedValue[0]) && Char.IsAsciiLetter(encodedValue[^1])) // returns false for capital letters as well
    {
        //lists
        if (encodedValue[0] == 'l' && encodedValue[^1] == 'e') //EX: l5:helloi52ee
        {
            // loop until reach e
            //set current index
            var currentIndex = 1;

            while (currentIndex < encodedValue.Length-1)
            {
                // remaking first 'if' logic, come up with reusable way...
                if (Char.IsDigit(encodedValue[currentIndex]))
                {
                    var str = DecodeString(encodedValue, out int strLength ,currentIndex);
                    currentIndex += strLength;
                    Console.WriteLine(str);
                }
                else if (encodedValue[currentIndex] == 'i') // i guess i'm hoping that only ints start with i...
                {
                    var intValue = DecodeInteger(encodedValue, out currentIndex, currentIndex);
                    Console.WriteLine(intValue);
                }
                else
                {
                    //this should be end... we should break anyway to prevent recursion.
                    //just throw error as we should never be here
                    throw new InvalidDataException($"Data malformed, unable to continue processing loop. Current index: {currentIndex} - Current value: {encodedValue[currentIndex]} - Encoded value: " + encodedValue);
                }
            }

            var colonIndex = encodedValue.IndexOf(':');
        }
        else
        {
            // Example: i52e
            Console.WriteLine(JsonSerializer.Serialize(DecodeInteger(encodedValue, out int currentIndex)));
        }
    }
    else
    {
        throw new InvalidOperationException("Unhandled encoded value: " + encodedValue);
    }
}
else
{
    throw new InvalidOperationException($"Invalid command: {command}");
}



static Int128 DecodeInteger(string encodedValue, out int currentIndex,  int startIndex = 1)
{
    int endIndex = startIndex;
    foreach (char c in encodedValue)
    {
        if (Char.IsDigit(c))
            endIndex++;
        else if (Char.IsLetter(c))
            break;
    }
    currentIndex = endIndex;
    if (Int128.TryParse(encodedValue[startIndex..endIndex], out Int128 int128))
    {
        return int128;
    }
    else
    {
        throw new InvalidCastException("Invaid cast: " + encodedValue[startIndex..endIndex]);
    }
}

static string DecodeString(string encodedValue, out int strLength, int startIndex = 0)
{
    var colonIndex = encodedValue.IndexOf(':');
    if (colonIndex != -1)
    {
        //can pass back regular string length to account for the colon 
        strLength = int.Parse(encodedValue[startIndex..colonIndex]);
        return encodedValue.Substring(colonIndex + 1, strLength);
    }
    else
    {
        throw new InvalidOperationException("Invalid encoded value: " + encodedValue);
    }
}