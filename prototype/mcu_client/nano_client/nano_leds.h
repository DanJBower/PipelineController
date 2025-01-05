#ifndef __NANO_LEDS_H
#define __NANO_LEDS_H
#pragma once

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

void SetupLedPins();
void SetBrightness(int brightness);
void SetSimpleColor(SimpleColor color);
void DriveLedColorWheel();
void SetColor(int r, int g, int b);

#endif
