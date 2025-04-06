package com.danjbower.pipelinecontrollerviewer.data

import java.nio.ByteBuffer
import java.nio.ByteOrder

object JoystickSerialiser
{
    private const val FLOAT_SIZE = 4
    private const val BYTES_REQUIRED = 2 * FLOAT_SIZE

    private const val X_POSITION = 0
    private const val Y_POSITION = X_POSITION + FLOAT_SIZE

    fun deserialiseJoystickState(data: ByteArray): Pair<Float, Float> {
        require(data.size >= BYTES_REQUIRED) {
            "Not enough bytes to deserialize joystick state (need $BYTES_REQUIRED, got ${data.size})"
        }
        val x = ByteBuffer.wrap(data, X_POSITION, FLOAT_SIZE).order(ByteOrder.LITTLE_ENDIAN).float
        val y = ByteBuffer.wrap(data, Y_POSITION, FLOAT_SIZE).order(ByteOrder.LITTLE_ENDIAN).float
        return x to y
    }

    fun serialiseJoystickState(joystickPosition: Pair<Float, Float>): ByteArray {
        val (x, y) = joystickPosition
        val result = ByteArray(BYTES_REQUIRED)
        writeFloat(result, X_POSITION, x)
        writeFloat(result, Y_POSITION, y)
        return result
    }

    private fun writeFloat(dest: ByteArray, offset: Int, value: Float) {
        val buf = ByteBuffer.allocate(FLOAT_SIZE).order(ByteOrder.LITTLE_ENDIAN)
        buf.putFloat(value)
        System.arraycopy(buf.array(), 0, dest, offset, FLOAT_SIZE)
    }
}
