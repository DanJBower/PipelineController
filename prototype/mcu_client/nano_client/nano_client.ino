#define DELAY_BETWEEN_COLORS 10

void setup()
{
    pinMode(LED_BUILTIN, OUTPUT);
    pinMode(LED_RED, OUTPUT);
    pinMode(LED_GREEN, OUTPUT);
    pinMode(LED_BLUE, OUTPUT);
}

void loop()
{
    for (int i = 0; i <= 255; i++) {
        SetColor(255, i, 0);
        analogWrite(LED_BUILTIN, i);
        delay(DELAY_BETWEEN_COLORS);
    }

    for (int i = 255; i >= 0; i--) {
        SetColor(i, 255, 0);
        analogWrite(LED_BUILTIN, i);
        delay(DELAY_BETWEEN_COLORS);
    }

    for (int i = 0; i <= 255; i++) {
        SetColor(0, 255, i);
        analogWrite(LED_BUILTIN, i);
        delay(DELAY_BETWEEN_COLORS);
    }

    for (int i = 255; i >= 0; i--) {
        SetColor(0, i, 255);
        analogWrite(LED_BUILTIN, i);
        delay(DELAY_BETWEEN_COLORS);
    }

    for (int i = 0; i <= 255; i++) {
        SetColor(i, 0, 255);
        analogWrite(LED_BUILTIN, i);
        delay(DELAY_BETWEEN_COLORS);
    }

    for (int i = 255; i >= 0; i--) {
        SetColor(255, 0, i);
        analogWrite(LED_BUILTIN, i);
        delay(DELAY_BETWEEN_COLORS);
    }
}

void SetColor(int r, int g, int b)
{
    r = 255 - r;
    g = 255 - g;
    b = 255 - b;
    analogWrite(LED_RED, r);
    analogWrite(LED_GREEN, g);
    analogWrite(LED_BLUE, b);
}
