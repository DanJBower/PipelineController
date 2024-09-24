using System.Text;

namespace ServerInfo;

public static class ServerDataConverter
{
    public const byte StringAlias = 0;
    public const byte IntAlias = 1;
    public const byte FloatAlias = 2;
    public const byte BoolAlias = 3;

    public static readonly Dictionary<byte, Func<byte[], dynamic>> ByteToType = new()
    {
        {StringAlias, data => Encoding.UTF8.GetString(data)},
        {IntAlias, data => BitConverter.ToInt32(data)},
        {FloatAlias, data => BitConverter.ToSingle(data)},
        {BoolAlias, data => BitConverter.ToBoolean(data)},
    };

    public static dynamic ExtractData(byte[] data)
    {
        var dataType = data[0];
        var dataValue = data[1..];

        if (ByteToType.TryGetValue(dataType, out var converter))
        {
            return converter(dataValue);
        }

        throw new ArgumentException("Could not infer type of data");
    }

    public static readonly Dictionary<Type, Func<dynamic, byte[]>> TypeToByte = new()
    {
        {typeof(string), data => TagData(StringAlias,Encoding.UTF8.GetBytes(data))},
        {typeof(int), data => TagData(IntAlias, BitConverter.GetBytes(data))},
        {typeof(float), data => TagData(FloatAlias, BitConverter.GetBytes(data))},
        {typeof(bool), data => TagData(BoolAlias, BitConverter.GetBytes(data))},
    };

    private static byte[] TagData(byte typeAlias, byte[] data)
    {
        var newValues = new byte[data.Length + 1];
        newValues[0] = typeAlias;
        Array.Copy(data, 0, newValues, 1, data.Length);
        return newValues;
    }

    public static byte[] ExtractBytes(dynamic data)
    {
        if (TypeToByte.TryGetValue(data.GetType(), out Func<dynamic, byte[]> converter))
        {
            return converter(data);
        }

        throw new ArgumentException($"No type conversion registered for {data.GetType()}");
    }
}
