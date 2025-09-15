#pragma once
#include <Arduino.h>
#include <Adafruit_NeoPixel.h>
#include <ArduinoJson.h>



class LedService{
    private:
    static DeserializationError error;
    static JsonDocument doc;
    static int r;
    static int g;
    static int b;
    static int brightness;

    public:
    static void getLedJson(String msg);
    static void setLedColor();
    static void handleIControl(String msg);
    static void handleLed(String msg);
    static void ledFadein(String msg);
};