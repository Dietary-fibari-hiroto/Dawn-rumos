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
    static String mode;

    public:
    static void setJsonUtil(String msg);
    static void modeBranch(String msg);
    static void getLedJson(String msg);
    static void setLedColor();
    static void handleIControl(String msg);
    static void handleLed();
    static void ledFadein();
    static void WhiteGradient();
    
    static void returnState();
};