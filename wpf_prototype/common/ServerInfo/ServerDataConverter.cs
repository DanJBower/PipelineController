using Controller;
using System.Text;

namespace ServerInfo;

public static class ServerDataConverter
{
    public const byte NoneAlias = 0;

    public const byte CharAlias = 1;
    public const byte StringAlias = 2;

    public const byte BoolAlias = 3;

    public const byte ByteAlias = 4;
    public const byte UShortAlias = 5;
    public const byte UIntAlias = 6;
    public const byte ULongAlias = 7;

    public const byte ShortAlias = 8;
    public const byte IntAlias = 9;
    public const byte LongAlias = 10;

    public const byte HalfAlias = 11;
    public const byte FloatAlias = 12;
    public const byte DoubleAlias = 13;

    public const byte ControllerStateAlias = 14;

    private static readonly object None = new();

    private static readonly Dictionary<byte, Func<byte[], dynamic>> ByteToType = new()
    {
        {CharAlias, data => BitConverter.ToChar(data)},
        {StringAlias, data => Encoding.UTF8.GetString(data)},

        {BoolAlias, data => BitConverter.ToBoolean(data)},

        {ByteAlias, data => data[0]},
        {UShortAlias, data => BitConverter.ToUInt16(data)},
        {UIntAlias, data => BitConverter.ToUInt32(data)},
        {ULongAlias, data => BitConverter.ToUInt64(data)},

        {ShortAlias, data => BitConverter.ToInt16(data)},
        {IntAlias, data => BitConverter.ToInt32(data)},
        {LongAlias, data => BitConverter.ToInt64(data)},

        {HalfAlias, data => BitConverter.ToHalf(data)},
        {FloatAlias, data => BitConverter.ToSingle(data)},
        {DoubleAlias, data => BitConverter.ToDouble(data)},

        {ControllerStateAlias, data => ControllerStateSerialiser.DeserialiseControllerState(data)},
    };

    public static (DateTime, dynamic) ExtractData(byte[] data)
    {
        var dataType = data[0];
        var timestamp = new DateTime(BitConverter.ToInt64(data.AsSpan()[1..9]));

        if (dataType > 0)
        {
            var dataValue = data[9..];

            if (ByteToType.TryGetValue(dataType, out var converter))
            {
                return (timestamp, converter(dataValue));
            }

            throw new ArgumentException("Could not infer type of data");
        }

        return (timestamp, None);
    }

    private static readonly Dictionary<Type, Func<dynamic, byte[]>> TypeToByte = new()
    {
        {typeof(char), data => TagData(CharAlias, BitConverter.GetBytes(data))},
        {typeof(string), data => TagData(StringAlias, Encoding.UTF8.GetBytes(data))},

        {typeof(bool), data => TagData(BoolAlias, BitConverter.GetBytes(data))},

        {typeof(byte), data => TagData(ByteAlias, [data])},
        {typeof(ushort), data => TagData(UShortAlias, BitConverter.GetBytes(data))},
        {typeof(uint), data => TagData(UIntAlias, BitConverter.GetBytes(data))},
        {typeof(ulong), data => TagData(ULongAlias, BitConverter.GetBytes(data))},

        {typeof(short), data => TagData(ShortAlias, BitConverter.GetBytes(data))},
        {typeof(int), data => TagData(IntAlias, BitConverter.GetBytes(data))},
        {typeof(long), data => TagData(LongAlias, BitConverter.GetBytes(data))},

        {typeof(Half), data => TagData(HalfAlias, BitConverter.GetBytes(data))},
        {typeof(float), data => TagData(FloatAlias, BitConverter.GetBytes(data))},
        {typeof(double), data => TagData(DoubleAlias, BitConverter.GetBytes(data))},

        {typeof(ControllerState), data => TagData(ControllerStateAlias, ControllerStateSerialiser.SerialiseControllerState(data))},
    };

    private static byte[] TagData(byte typeAlias, byte[] data)
    {
        var newValues = new byte[data.Length + 9];
        newValues[0] = typeAlias;

        var timestamp = BitConverter.GetBytes(DateTime.Now.Ticks);
        Array.Copy(timestamp, 0, newValues, 1, timestamp.Length);

        Array.Copy(data, 0, newValues, 9, data.Length);
        return newValues;
    }

    public static byte[] ExtractBytes(dynamic? data)
    {
        if (data is null)
        {
            var newValues = new byte[9];
            newValues[0] = NoneAlias;
            var timestamp = BitConverter.GetBytes(DateTime.Now.Ticks);
            Array.Copy(timestamp, 0, newValues, 1, timestamp.Length);
            return newValues;
        }

        if (TypeToByte.TryGetValue(data.GetType(), out Func<dynamic, byte[]> converter))
        {
            return converter(data);
        }

        throw new ArgumentException($"No type conversion registered for {data.GetType()}");
    }
}
