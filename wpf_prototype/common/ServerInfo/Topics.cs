namespace ServerInfo;

public static class Topics
{
    public const string StartTopic = "controller/Start";
    public const ushort StartTopicAlias = 1;


    public const string SelectTopic = "controller/Select";
    public const ushort SelectTopicAlias = 2;


    public const string HomeTopic = "controller/Home";
    public const ushort HomeTopicAlias = 3;


    public const string BigHomeTopic = "controller/BigHome";
    public const ushort BigHomeTopicAlias = 4;


    public const string XTopic = "controller/X";
    public const ushort XTopicAlias = 5;


    public const string YTopic = "controller/Y";
    public const ushort YTopicAlias = 6;


    public const string ATopic = "controller/A";
    public const ushort ATopicAlias = 7;


    public const string BTopic = "controller/B";
    public const ushort BTopicAlias = 8;


    public const string UpTopic = "controller/Up";
    public const ushort UpTopicAlias = 9;


    public const string RightTopic = "controller/Right";
    public const ushort RightTopicAlias = 10;


    public const string DownTopic = "controller/Down";
    public const ushort DownTopicAlias = 11;


    public const string LeftTopic = "controller/Left";
    public const ushort LeftTopicAlias = 12;


    public const string LeftStickXTopic = "controller/LeftStickX";
    public const ushort LeftStickXTopicAlias = 13;


    public const string LeftStickYTopic = "controller/LeftStickY";
    public const ushort LeftStickYTopicAlias = 14;


    public const string LeftStickInTopic = "controller/LeftStickIn";
    public const ushort LeftStickInTopicAlias = 15;


    public const string RightStickXTopic = "controller/RightStickX";
    public const ushort RightStickXTopicAlias = 16;


    public const string RightStickYTopic = "controller/RightStickY";
    public const ushort RightStickYTopicAlias = 17;


    public const string RightStickInTopic = "controller/RightStickIn";
    public const ushort RightStickInTopicAlias = 18;


    public const string LeftBumperTopic = "controller/LeftBumper";
    public const ushort LeftBumperTopicAlias = 19;


    public const string LeftTriggerTopic = "controller/LeftTrigger";
    public const ushort LeftTriggerTopicAlias = 20;


    public const string RightBumperTopic = "controller/RightBumper";
    public const ushort RightBumperTopicAlias = 21;


    public const string RightTriggerTopic = "controller/RightTrigger";
    public const ushort RightTriggerTopicAlias = 22;

    public static readonly Dictionary<ushort, string> AliasedTopics = [];

    static Topics()
    {
        AliasedTopics.Add(StartTopicAlias, StartTopic);
        AliasedTopics.Add(SelectTopicAlias, SelectTopic);
        AliasedTopics.Add(HomeTopicAlias, HomeTopic);
        AliasedTopics.Add(BigHomeTopicAlias, BigHomeTopic);
        AliasedTopics.Add(XTopicAlias, XTopic);
        AliasedTopics.Add(YTopicAlias, YTopic);
        AliasedTopics.Add(ATopicAlias, ATopic);
        AliasedTopics.Add(BTopicAlias, BTopic);
        AliasedTopics.Add(UpTopicAlias, UpTopic);
        AliasedTopics.Add(RightTopicAlias, RightTopic);
        AliasedTopics.Add(DownTopicAlias, DownTopic);
        AliasedTopics.Add(LeftTopicAlias, LeftTopic);
        AliasedTopics.Add(LeftStickXTopicAlias, LeftStickXTopic);
        AliasedTopics.Add(LeftStickYTopicAlias, LeftStickYTopic);
        AliasedTopics.Add(LeftStickInTopicAlias, LeftStickInTopic);
        AliasedTopics.Add(RightStickXTopicAlias, RightStickXTopic);
        AliasedTopics.Add(RightStickYTopicAlias, RightStickYTopic);
        AliasedTopics.Add(RightStickInTopicAlias, RightStickInTopic);
        AliasedTopics.Add(LeftBumperTopicAlias, LeftBumperTopic);
        AliasedTopics.Add(LeftTriggerTopicAlias, LeftTriggerTopic);
        AliasedTopics.Add(RightBumperTopicAlias, RightBumperTopic);
        AliasedTopics.Add(RightTriggerTopicAlias, RightTriggerTopic);
    }
}
