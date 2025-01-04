#include "client_secrets.h"
#include "client_config.h"
#include "client_types.h"

#include <WiFi.h>

// TODO Look into:
// * Periodic Timer with callback - https://docs.espressif.com/projects/esp-idf/en/latest/esp32s3/api-reference/system/esp_timer.html
// * MQTT - https://docs.espressif.com/projects/esp-idf/en/latest/esp32s3/api-reference/protocols/mqtt.html

#define DELAY_BETWEEN_COLORS 10
enum class SimpleColor
{
    Black,
    White,
    Red,
    Green,
    Blue,
    Yellow,
    Purple,
    Cyan
};

void setup()
{
    SetupLedPins();
    Serial.begin(115200);

    while(!Serial); // Wait for serial to connect - May need to be removed when running on actual console, but useful for debugging on PC

    SetSimpleColor(SimpleColor::Red);
    Serial.println();
    Serial.println();
    Serial.println();
    Serial.println("Starting");

    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

    while (WiFi.status() != WL_CONNECTED) {
        Serial.println("Attempting to connect to Wifi");
        delay(100);
    }
    
    Serial.println("Wifi connected");

    SetSimpleColor(SimpleColor::Blue);
}

void loop()
{
    if (WiFi.status() == WL_CONNECTED)
    {
        Serial.println("Running: Wifi connected");
    }
    else
    {
        Serial.println("Running: Wifi not connected");
    }

    delay(1000);
}

void SetupLedPins()
{
    pinMode(LED_BUILTIN, OUTPUT); // This is top left orange LED. Can only change it's brightness.
    pinMode(LED_RED, OUTPUT);     // These 3 pins make up the RGB LED but the pins have to be
    pinMode(LED_GREEN, OUTPUT);   // controlled separately.
    pinMode(LED_BLUE, OUTPUT);    // The pinout says it shouldn't support analogWrite (as no ~ next to pins)
                                  // but it does.
                                  // https://docs.arduino.cc/resources/pinouts/ABX00083-full-pinout.pdf
}

void SetBrightness(int brightness)
{
    analogWrite(LED_BUILTIN, brightness);
}

void SetSimpleColor(SimpleColor color)
{
    if (color == SimpleColor::White)
    {
        digitalWrite(LED_RED, LOW);
        digitalWrite(LED_GREEN, LOW);
        digitalWrite(LED_BLUE, LOW);
    }
    else if (color == SimpleColor::Red)
    {
        digitalWrite(LED_RED, LOW);
        digitalWrite(LED_GREEN, HIGH);
        digitalWrite(LED_BLUE, HIGH);
    }
    else if (color == SimpleColor::Green)
    {
        digitalWrite(LED_RED, HIGH);
        digitalWrite(LED_GREEN, LOW);
        digitalWrite(LED_BLUE, HIGH);
    }
    else if (color == SimpleColor::Blue)
    {
        digitalWrite(LED_RED, HIGH);
        digitalWrite(LED_GREEN, HIGH);
        digitalWrite(LED_BLUE, LOW);
    }
    else if (color == SimpleColor::Yellow)
    {
        digitalWrite(LED_RED, LOW);
        digitalWrite(LED_GREEN, LOW);
        digitalWrite(LED_BLUE, HIGH);
    }
    else if (color == SimpleColor::Purple)
    {
        digitalWrite(LED_RED, LOW);
        digitalWrite(LED_GREEN, HIGH);
        digitalWrite(LED_BLUE, LOW);
    }
    else if (color == SimpleColor::Cyan)
    {
        digitalWrite(LED_RED, HIGH);
        digitalWrite(LED_GREEN, LOW);
        digitalWrite(LED_BLUE, LOW);
    }
    else
    {
        digitalWrite(LED_RED, HIGH);
        digitalWrite(LED_GREEN, HIGH);
        digitalWrite(LED_BLUE, HIGH);
    }
}

void DriveLedColorWheel()
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
