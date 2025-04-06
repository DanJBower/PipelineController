package com.danjbower.pipelinecontrollerviewer.data

import java.nio.ByteBuffer
import java.nio.ByteOrder

object ControllerStateSerialiser
{
    private const val FLOAT_SIZE = 4
    private const val BOOL_SIZE = 1

    // 6 floats + 16 bools = 24 + 16 = 40 bytes total
    private const val BYTES_REQUIRED = (6 * FLOAT_SIZE) + (16 * BOOL_SIZE)

    // Same positional constants as in C#:
    private const val START_POSITION = 0
    private const val SELECT_POSITION = START_POSITION + BOOL_SIZE
    private const val HOME_POSITION = SELECT_POSITION + BOOL_SIZE
    private const val BIG_HOME_POSITION = HOME_POSITION + BOOL_SIZE
    private const val X_POSITION = BIG_HOME_POSITION + BOOL_SIZE
    private const val Y_POSITION = X_POSITION + BOOL_SIZE
    private const val A_POSITION = Y_POSITION + BOOL_SIZE
    private const val B_POSITION = A_POSITION + BOOL_SIZE
    private const val UP_POSITION = B_POSITION + BOOL_SIZE
    private const val RIGHT_POSITION = UP_POSITION + BOOL_SIZE
    private const val DOWN_POSITION = RIGHT_POSITION + BOOL_SIZE
    private const val LEFT_POSITION = DOWN_POSITION + BOOL_SIZE
    private const val LEFT_STICK_X_POSITION = LEFT_POSITION + BOOL_SIZE
    private const val LEFT_STICK_Y_POSITION = LEFT_STICK_X_POSITION + FLOAT_SIZE
    private const val LEFT_STICK_IN_POSITION = LEFT_STICK_Y_POSITION + FLOAT_SIZE
    private const val RIGHT_STICK_X_POSITION = LEFT_STICK_IN_POSITION + BOOL_SIZE
    private const val RIGHT_STICK_Y_POSITION = RIGHT_STICK_X_POSITION + FLOAT_SIZE
    private const val RIGHT_STICK_IN_POSITION = RIGHT_STICK_Y_POSITION + FLOAT_SIZE
    private const val LEFT_BUMPER_POSITION = RIGHT_STICK_IN_POSITION + BOOL_SIZE
    private const val LEFT_TRIGGER_POSITION = LEFT_BUMPER_POSITION + BOOL_SIZE
    private const val RIGHT_BUMPER_POSITION = LEFT_TRIGGER_POSITION + FLOAT_SIZE
    private const val RIGHT_TRIGGER_POSITION = RIGHT_BUMPER_POSITION + BOOL_SIZE

    /**
     * Deserializes a [ByteArray] into a [ControllerState].
     * This directly mirrors your C# logic and field layout.
     */
    fun deserialiseControllerState(data: ByteArray): ControllerState {
        require(data.size >= BYTES_REQUIRED) {
            "Not enough bytes to deserialize ControllerState (need $BYTES_REQUIRED, got ${data.size})"
        }

        val start = readBoolean(data, START_POSITION)
        val select = readBoolean(data, SELECT_POSITION)
        val home = readBoolean(data, HOME_POSITION)
        val bigHome = readBoolean(data, BIG_HOME_POSITION)
        val x = readBoolean(data, X_POSITION)
        val y = readBoolean(data, Y_POSITION)
        val a = readBoolean(data, A_POSITION)
        val b = readBoolean(data, B_POSITION)
        val up = readBoolean(data, UP_POSITION)
        val right = readBoolean(data, RIGHT_POSITION)
        val down = readBoolean(data, DOWN_POSITION)
        val left = readBoolean(data, LEFT_POSITION)
        val leftStickX = readFloat(data, LEFT_STICK_X_POSITION)
        val leftStickY = readFloat(data, LEFT_STICK_Y_POSITION)
        val leftStickIn = readBoolean(data, LEFT_STICK_IN_POSITION)
        val rightStickX = readFloat(data, RIGHT_STICK_X_POSITION)
        val rightStickY = readFloat(data, RIGHT_STICK_Y_POSITION)
        val rightStickIn = readBoolean(data, RIGHT_STICK_IN_POSITION)
        val leftBumper = readBoolean(data, LEFT_BUMPER_POSITION)
        val leftTrigger = readFloat(data, LEFT_TRIGGER_POSITION)
        val rightBumper = readBoolean(data, RIGHT_BUMPER_POSITION)
        val rightTrigger = readFloat(data, RIGHT_TRIGGER_POSITION)

        return ControllerState(
            start = start,
            select = select,
            home = home,
            bigHome = bigHome,
            x = x,
            y = y,
            a = a,
            b = b,
            up = up,
            right = right,
            down = down,
            left = left,
            leftStickX = leftStickX,
            leftStickY = leftStickY,
            leftStickIn = leftStickIn,
            rightStickX = rightStickX,
            rightStickY = rightStickY,
            rightStickIn = rightStickIn,
            leftBumper = leftBumper,
            leftTrigger = leftTrigger,
            rightBumper = rightBumper,
            rightTrigger = rightTrigger
        )
    }

    /**
     * Serializes a [ControllerState] into a [ByteArray].
     * Matches the exact layout expected by [deserialiseControllerState].
     */
    fun serialiseControllerState(controllerState: ControllerState): ByteArray {
        val result = ByteArray(BYTES_REQUIRED)
        writeBoolean(result, START_POSITION, controllerState.start)
        writeBoolean(result, SELECT_POSITION, controllerState.select)
        writeBoolean(result, HOME_POSITION, controllerState.home)
        writeBoolean(result, BIG_HOME_POSITION, controllerState.bigHome)
        writeBoolean(result, X_POSITION, controllerState.x)
        writeBoolean(result, Y_POSITION, controllerState.y)
        writeBoolean(result, A_POSITION, controllerState.a)
        writeBoolean(result, B_POSITION, controllerState.b)
        writeBoolean(result, UP_POSITION, controllerState.up)
        writeBoolean(result, RIGHT_POSITION, controllerState.right)
        writeBoolean(result, DOWN_POSITION, controllerState.down)
        writeBoolean(result, LEFT_POSITION, controllerState.left)
        writeFloat(result, LEFT_STICK_X_POSITION, controllerState.leftStickX)
        writeFloat(result, LEFT_STICK_Y_POSITION, controllerState.leftStickY)
        writeBoolean(result, LEFT_STICK_IN_POSITION, controllerState.leftStickIn)
        writeFloat(result, RIGHT_STICK_X_POSITION, controllerState.rightStickX)
        writeFloat(result, RIGHT_STICK_Y_POSITION, controllerState.rightStickY)
        writeBoolean(result, RIGHT_STICK_IN_POSITION, controllerState.rightStickIn)
        writeBoolean(result, LEFT_BUMPER_POSITION, controllerState.leftBumper)
        writeFloat(result, LEFT_TRIGGER_POSITION, controllerState.leftTrigger)
        writeBoolean(result, RIGHT_BUMPER_POSITION, controllerState.rightBumper)
        writeFloat(result, RIGHT_TRIGGER_POSITION, controllerState.rightTrigger)
        return result
    }

    // Simple read/write helpers to mimic BitConverter in C#.
    private fun readBoolean(data: ByteArray, offset: Int): Boolean =
        data[offset].toInt() != 0

    private fun writeBoolean(dest: ByteArray, offset: Int, value: Boolean) {
        dest[offset] = if (value) 1 else 0
    }

    private fun readFloat(data: ByteArray, offset: Int): Float =
        ByteBuffer.wrap(data, offset, FLOAT_SIZE).order(ByteOrder.LITTLE_ENDIAN).float

    private fun writeFloat(dest: ByteArray, offset: Int, value: Float) {
        val buf = ByteBuffer.allocate(FLOAT_SIZE).order(ByteOrder.LITTLE_ENDIAN)
        buf.putFloat(value)
        System.arraycopy(buf.array(), 0, dest, offset, FLOAT_SIZE)
    }
}