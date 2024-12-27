#r "nuget: ppy.SDL3-CS, 2024.1128.0"

using SDL;

{
    SDL3.SDL_SetHint(SDL3.SDL_HINT_JOYSTICK_HIDAPI_PS5, "1");
    if (!SDL3.SDL_Init(SDL_InitFlags.SDL_INIT_JOYSTICK | SDL_InitFlags.SDL_INIT_GAMEPAD))
    {
        throw new Exception("Couldn't initialise SDL");
    }

    var gamepadId = GetFirstValidControllerId();

    WriteLine("Press any key to stop");

    unsafe
    {
        var gamepad = SDL3.SDL_OpenGamepad(gamepadId);

        var name = SDL3.SDL_GetGamepadName(gamepad);

        SDL_Event sdlEvent;
        var running = true;

        while (running)
        {
            if (KeyAvailable)
            {
                running = false;
            }

            while (SDL3.SDL_PollEvent(&sdlEvent))
            {
                if (sdlEvent.Type == SDL_EventType.SDL_EVENT_QUIT)
                {
                    running = false;
                    break;
                }

                if (sdlEvent.Type is SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN
                    or SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP)
                {
                    bool isDown = sdlEvent.button.down;
                    WriteLine($"{sdlEvent.Type}: {sdlEvent.gbutton.Button} {isDown}");
                }
                else if (sdlEvent.Type is SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION)
                {
                    WriteLine($"{sdlEvent.Type}: {sdlEvent.gaxis.Axis} {sdlEvent.gaxis.value}");
                }
            }
        }

        SDL3.SDL_CloseGamepad(gamepad);
    }

    SDL3.SDL_Quit();
}

private static SDL_JoystickID GetFirstValidControllerId()
{
    var joystickIds = SDL3.SDL_GetJoysticks();

    if (joystickIds is null || joystickIds.Count == 0)
    {
        throw new Exception("Couldn't find any joysticks");
    }

    for (var i = 0; i < joystickIds.Count; i++)
    {
        var joystickId = joystickIds[i];

        if (!SDL3.SDL_IsGamepad(joystickId))
        {
            continue;
        }

        bool isPs5Joystick;

        unsafe
        {
            var gamepad = SDL3.SDL_OpenGamepad(joystickId);
            var gamepadType = SDL3.SDL_GetGamepadType(gamepad);
            isPs5Joystick = gamepadType is SDL_GamepadType.SDL_GAMEPAD_TYPE_PS5;
            SDL3.SDL_CloseGamepad(gamepad);
        }

        if (isPs5Joystick)
        {
            return joystickId;
        }
    }

    throw new Exception("Couldn't find any gamepads from joystick list");
}
