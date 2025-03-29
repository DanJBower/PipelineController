package com.danjbower.pipelinecontrollerviewer.data

import java.nio.ByteBuffer
import java.nio.ByteOrder
import java.nio.charset.StandardCharsets
import java.time.Instant

// Example placeholder for "Half" in .NET. You may need a real implementation:
class Half(val value: Short) {
    override fun toString(): String = "Half($value)"
}

object ServerDataConverter
{
    // Byte "aliases" (equivalent to C# public const byte):
    const val NONE_ALIAS: Byte = 0

    const val CHAR_ALIAS: Byte = 1
    const val STRING_ALIAS: Byte = 2

    const val BOOL_ALIAS: Byte = 3

    const val BYTE_ALIAS: Byte = 4
    const val USHORT_ALIAS: Byte = 5
    const val UINT_ALIAS: Byte = 6
    const val ULONG_ALIAS: Byte = 7

    const val SHORT_ALIAS: Byte = 8
    const val INT_ALIAS: Byte = 9
    const val LONG_ALIAS: Byte = 10

    const val HALF_ALIAS: Byte = 11
    const val FLOAT_ALIAS: Byte = 12
    const val DOUBLE_ALIAS: Byte = 13

    const val CONTROLLER_STATE_ALIAS: Byte = 14
    const val JOYSTICK_STATE_ALIAS: Byte = 15

    /**
     * A map from a type-tag byte to a lambda that takes the raw byte array
     * and converts it to a Kotlin object (Any).
     */
    private val byteToType: Map<Byte, (ByteArray) -> Any> = mapOf(
        CHAR_ALIAS to { data -> toChar(data) },
        STRING_ALIAS to { data -> toString(data) },
        BOOL_ALIAS to { data -> toBoolean(data) },
        BYTE_ALIAS to { data -> data[0] },
        USHORT_ALIAS to { data -> toUShort(data) },
        UINT_ALIAS to { data -> toUInt(data) },
        ULONG_ALIAS to { data -> toULong(data) },
        SHORT_ALIAS to { data -> toShort(data) },
        INT_ALIAS to { data -> toInt(data) },
        LONG_ALIAS to { data -> toLong(data) },
        HALF_ALIAS to { data -> toHalf(data) }, // see custom half conversion
        FLOAT_ALIAS to { data -> toFloat(data) },
        DOUBLE_ALIAS to { data -> toDouble(data) },
        CONTROLLER_STATE_ALIAS to { data -> ControllerStateSerialiser.deserialiseControllerState(data) },
        JOYSTICK_STATE_ALIAS to { data -> JoystickSerialiser.deserialiseJoystickState(data) }
    )

    /**
     * Extracts (timestamp, typedValueOrNull) from the raw data.
     * C# returned (DateTime, dynamic?), here we return Pair<Long, Any?>
     * storing .NET ticks as a raw Long. Convert to an Instant, etc., if you prefer.
     */
    fun extractData(data: ByteArray): Pair<Instant, Any?> {
        // data[0] is the "alias" of the type
        val dataType = data[0]
        // Next 8 bytes are .NET Ticks (little-endian):
        val ticks = toLong(data.sliceArray(1..8))

        // TODO Debug why this isn't returning the correct instant. It's returning like 1990
        val instant = Instant.ofEpochSecond(0, ticks)

        // If dataType > 0, there's a payload after the first 9 bytes:
        return if (dataType > 0) {
            val dataValue = data.copyOfRange(9, data.size)
            val converter = byteToType[dataType]
                ?: throw IllegalArgumentException("Could not infer type of data ($dataType)")
            instant to converter.invoke(dataValue)
        } else {
            // dataType == 0 => no value
            instant to null
        }
    }

    /**
     * Opposite: we have an object (Any?) and want a byte array tagged with
     * the "alias" plus the current timestamp.
     */
    fun extractBytes(data: Any?): ByteArray {
        // If null, produce a 9-byte array with alias=0 + 8-byte current ticks
        if (data == null) {
            val result = ByteArray(9)
            result[0] = NONE_ALIAS
            val nowTicks = currentDotNetTicks()
            copyLongToByteArray(nowTicks, result, 1)
            return result
        }

        // Otherwise convert to bytes using the type -> lambda approach:
        val converter = typeToByte[data::class]
            ?: throw IllegalArgumentException("No type conversion registered for ${data::class}")
        return converter(data)
    }

    /**
     * Map from Kotlin type (via KClass) to a function that tags the data with an alias
     * and includes the 8-byte timestamp in front.
     */
    private val typeToByte: Map<kotlin.reflect.KClass<*>, (Any) -> ByteArray> = mapOf(
        Char::class to { value ->
            tagData(CHAR_ALIAS, fromChar(value as Char))
        },
        String::class to { value ->
            tagData(STRING_ALIAS, (value as String).toByteArray(StandardCharsets.UTF_8))
        },
        Boolean::class to { value ->
            tagData(BOOL_ALIAS, fromBoolean(value as Boolean))
        },
        Byte::class to { value ->
            tagData(BYTE_ALIAS, byteArrayOf(value as Byte))
        },
        UShort::class to { value ->
            tagData(USHORT_ALIAS, fromUShort(value as UShort))
        },
        UInt::class to { value ->
            tagData(UINT_ALIAS, fromUInt(value as UInt))
        },
        ULong::class to { value ->
            tagData(ULONG_ALIAS, fromULong(value as ULong))
        },
        Short::class to { value ->
            tagData(SHORT_ALIAS, fromShort(value as Short))
        },
        Int::class to { value ->
            tagData(INT_ALIAS, fromInt(value as Int))
        },
        Long::class to { value ->
            tagData(LONG_ALIAS, fromLong(value as Long))
        },
        Half::class to { value ->
            // Convert your custom Half to two bytes, for instance:
            tagData(HALF_ALIAS, fromHalf(value as Half))
        },
        Float::class to { value ->
            tagData(FLOAT_ALIAS, fromFloat(value as Float))
        },
        Double::class to { value ->
            tagData(DOUBLE_ALIAS, fromDouble(value as Double))
        },
        ControllerState::class to { value ->
            tagData(CONTROLLER_STATE_ALIAS, ControllerStateSerialiser.serialiseControllerState(value as ControllerState))
        },
        // If you're using Pair<Float, Float> for Joystick, match with JOYSTICK_STATE_ALIAS:
        Pair::class to { value ->
            @Suppress("UNCHECKED_CAST")
            tagData(JOYSTICK_STATE_ALIAS, JoystickSerialiser.serialiseJoystickState(value as Pair<Float, Float>))
        }
    )

    /**
     * Prepends [typeAlias, timestamp, dataBytes].
     * The first byte is the type alias; next 8 are .NET ticks, then data payload.
     */
    private fun tagData(typeAlias: Byte, payload: ByteArray): ByteArray {
        val result = ByteArray(payload.size + 9)
        result[0] = typeAlias

        val nowTicks = currentDotNetTicks()
        copyLongToByteArray(nowTicks, result, 1)

        // Copy payload into index 9 onward
        System.arraycopy(payload, 0, result, 9, payload.size)
        return result
    }

    /**
     * In .NET, DateTime.Now.Ticks is the number of 100-ns ticks since year 1 AD.
     * This is a naive conversion from System.currentTimeMillis() to .NET ticks.
     *
     * If you don’t need faithful .NET alignment, or your system doesn’t need exact
     * synchronization with .NET, you can just store `System.currentTimeMillis()` raw.
     */
    private fun currentDotNetTicks(): Long {
        val instant = Instant.now()
        return (instant.epochSecond * 1_000_000_000L) + instant.nano
    }

    /** Helper to read/write 64-bit values in little-endian from/to a ByteArray. */
    private fun toLong(bytes: ByteArray): Long {
        return ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).long
    }
    private fun copyLongToByteArray(value: Long, destination: ByteArray, offset: Int) {
        val buf = ByteBuffer.allocate(8).order(ByteOrder.LITTLE_ENDIAN)
        buf.putLong(value)
        System.arraycopy(buf.array(), 0, destination, offset, 8)
    }

    /** Helper routines that replicate the .NET BitConverter usages. */
    private fun toChar(bytes: ByteArray): Char =
        ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).char

    private fun toBoolean(bytes: ByteArray): Boolean =
        (bytes[0].toInt() != 0)

    private fun toUShort(bytes: ByteArray): UShort =
        ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).short.toUShort()

    private fun toUInt(bytes: ByteArray): UInt =
        ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).int.toUInt()

    private fun toULong(bytes: ByteArray): ULong =
        ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).long.toULong()

    private fun toShort(bytes: ByteArray): Short =
        ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).short

    private fun toInt(bytes: ByteArray): Int =
        ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).int

    private fun toFloat(bytes: ByteArray): Float =
        ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).float

    private fun toDouble(bytes: ByteArray): Double =
        ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).double

    // .NET Half is 16 bits. We'll store it simply as the raw bits in a custom Half class:
    private fun toHalf(bytes: ByteArray): Half {
        val s = ByteBuffer.wrap(bytes).order(ByteOrder.LITTLE_ENDIAN).short
        return Half(s)
    }

    private fun toString(bytes: ByteArray): String =
        String(bytes, StandardCharsets.UTF_8)

    /** Opposite set of "fromX" conversions */
    private fun fromShort(value: Short): ByteArray =
        ByteBuffer.allocate(2).order(ByteOrder.LITTLE_ENDIAN).putShort(value).array()

    private fun fromInt(value: Int): ByteArray =
        ByteBuffer.allocate(4).order(ByteOrder.LITTLE_ENDIAN).putInt(value).array()

    private fun fromLong(value: Long): ByteArray =
        ByteBuffer.allocate(8).order(ByteOrder.LITTLE_ENDIAN).putLong(value).array()

    private fun fromChar(value: Char): ByteArray =
        ByteBuffer.allocate(2).order(ByteOrder.LITTLE_ENDIAN).putChar(value).array()

    private fun fromFloat(value: Float): ByteArray =
        ByteBuffer.allocate(4).order(ByteOrder.LITTLE_ENDIAN).putFloat(value).array()

    private fun fromDouble(value: Double): ByteArray =
        ByteBuffer.allocate(8).order(ByteOrder.LITTLE_ENDIAN).putDouble(value).array()

    private fun fromBoolean(value: Boolean): ByteArray =
        byteArrayOf(if (value) 1 else 0)

    private fun fromUShort(value: UShort): ByteArray =
        ByteBuffer.allocate(2).order(ByteOrder.LITTLE_ENDIAN).putShort(value.toShort()).array()

    private fun fromUInt(value: UInt): ByteArray =
        ByteBuffer.allocate(4).order(ByteOrder.LITTLE_ENDIAN).putInt(value.toInt()).array()

    private fun fromULong(value: ULong): ByteArray =
        ByteBuffer.allocate(8).order(ByteOrder.LITTLE_ENDIAN).putLong(value.toLong()).array()

    private fun fromHalf(value: Half): ByteArray =
        ByteBuffer.allocate(2).order(ByteOrder.LITTLE_ENDIAN).putShort(value.value).array()
}