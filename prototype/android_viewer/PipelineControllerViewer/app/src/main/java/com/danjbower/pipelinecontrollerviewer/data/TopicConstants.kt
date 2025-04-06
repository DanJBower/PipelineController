package com.danjbower.pipelinecontrollerviewer.data

class TopicConstants
{
    companion object {
        const val StartTopic = "controller/Start"
        const val StartTopicAlias = 1

        const val SelectTopic = "controller/Select"
        const val SelectTopicAlias = 2

        const val HomeTopic = "controller/Home"
        const val HomeTopicAlias = 3

        const val BigHomeTopic = "controller/BigHome"
        const val BigHomeTopicAlias = 4

        const val XTopic = "controller/X"
        const val XTopicAlias = 5

        const val YTopic = "controller/Y"
        const val YTopicAlias = 6

        const val ATopic = "controller/A"
        const val ATopicAlias = 7

        const val BTopic = "controller/B"
        const val BTopicAlias = 8

        const val UpTopic = "controller/Up"
        const val UpTopicAlias = 9

        const val RightTopic = "controller/Right"
        const val RightTopicAlias = 10

        const val DownTopic = "controller/Down"
        const val DownTopicAlias = 11

        const val LeftTopic = "controller/Left"
        const val LeftTopicAlias = 12

        const val LeftStickXTopic = "controller/LeftStickX"
        const val LeftStickXTopicAlias = 13

        const val LeftStickYTopic = "controller/LeftStickY"
        const val LeftStickYTopicAlias = 14

        const val LeftStickInTopic = "controller/LeftStickIn"
        const val LeftStickInTopicAlias = 15

        const val RightStickXTopic = "controller/RightStickX"
        const val RightStickXTopicAlias = 16

        const val RightStickYTopic = "controller/RightStickY"
        const val RightStickYTopicAlias = 17

        const val RightStickInTopic = "controller/RightStickIn"
        const val RightStickInTopicAlias = 18

        const val LeftBumperTopic = "controller/LeftBumper"
        const val LeftBumperTopicAlias = 19

        const val LeftTriggerTopic = "controller/LeftTrigger"
        const val LeftTriggerTopicAlias = 20

        const val RightBumperTopic = "controller/RightBumper"
        const val RightBumperTopicAlias = 21

        const val RightTriggerTopic = "controller/RightTrigger"
        const val RightTriggerTopicAlias = 22

        const val FullTopic = "controller/Full"
        const val FullTopicAlias = 23

        const val LeftStickTopic = "controller/LeftStick"
        const val LeftStickTopicAlias = 24

        const val RightStickTopic = "controller/RightStick"
        const val RightStickTopicAlias = 25

        const val DebugLightTopic = "debug_light"
        const val DebugLightTopicAlias = 26

        val AliasedTopics = mapOf(
            StartTopicAlias to StartTopic,
            /*SelectTopicAlias to SelectTopic,
            HomeTopicAlias to HomeTopic,
            BigHomeTopicAlias to BigHomeTopic,
            XTopicAlias to XTopic,
            YTopicAlias to YTopic,
            ATopicAlias to ATopic,
            BTopicAlias to BTopic,
            UpTopicAlias to UpTopic,
            RightTopicAlias to RightTopic,
            DownTopicAlias to DownTopic,
            LeftTopicAlias to LeftTopic,
            LeftStickXTopicAlias to LeftStickXTopic,
            LeftStickYTopicAlias to LeftStickYTopic,
            LeftStickInTopicAlias to LeftStickInTopic,
            RightStickXTopicAlias to RightStickXTopic,
            RightStickYTopicAlias to RightStickYTopic,
            RightStickInTopicAlias to RightStickInTopic,
            LeftBumperTopicAlias to LeftBumperTopic,
            LeftTriggerTopicAlias to LeftTriggerTopic,
            RightBumperTopicAlias to RightBumperTopic,
            RightTriggerTopicAlias to RightTriggerTopic,
            FullTopicAlias to FullTopic,
            LeftStickTopicAlias to LeftStickTopic,
            RightStickTopicAlias to RightStickTopic,*/
            DebugLightTopicAlias to DebugLightTopic,
        )
    }
}