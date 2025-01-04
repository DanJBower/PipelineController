#ifndef __CLIENT_TYPES_H
#define __CLIENT_TYPES_H
#pragma once

#include "Arduino.h"

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
