#include "client_secrets.h"
#include "client_config.h"
#include "client_types.h"
#include "nano_leds.h"

#include <WiFi.h>

// TODO Look into:
// * Periodic Timer with callback - https://docs.espressif.com/projects/esp-idf/en/latest/esp32s3/api-reference/system/esp_timer.html
// * MQTT - https://docs.espressif.com/projects/esp-idf/en/latest/esp32s3/api-reference/protocols/mqtt.html

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
