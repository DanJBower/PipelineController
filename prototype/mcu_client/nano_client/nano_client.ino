#define DELAY_BETWEEN_COLORS 10

void setup()
{
    pinMode(LED_BUILTIN, OUTPUT); // This is top left orange LED. Can only change it's brightness.
    pinMode(LED_RED, OUTPUT);     // These 3 pins make up the RGB LED but the pins have to be
    pinMode(LED_GREEN, OUTPUT);   // controlled separately.
    pinMode(LED_BLUE, OUTPUT);    // The pinout says it shouldn't support analogWrite (as no ~ next to pins)
                                  // but it does.
                                  // https://docs.arduino.cc/resources/pinouts/ABX00083-full-pinout.pdf
    Serial.begin(9600);
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
    Serial.print("r: ");
    Serial.print(r);
    Serial.print(" g: ");
    Serial.print(g);
    Serial.print(" b: ");
    Serial.println(b);
}
