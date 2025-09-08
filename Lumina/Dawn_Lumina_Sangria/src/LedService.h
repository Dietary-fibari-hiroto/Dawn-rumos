#pragma once
#include <Arduino.h>
#include <Adafruit_NeoPixel.h>




namespace LedService{
    void handleAll(String msg);
    void handleIControl(String msg);
    void handleLed(String msg);
    void ledFadein(String msg);
}