#include "LedService.h"
#include <ArduinoJson.h>
#include <Adafruit_NeoPixel.h>
#include "DeviceMetaData.h"


const int NUM_LEDS = CONST_NUM_LEDS;
const int LED_PIN = CONST_LED_PIN;
extern Adafruit_NeoPixel strip; // mainで定義したものを参照
    
DeserializationError LedService::error;
JsonDocument LedService::doc;
int LedService::r;
int LedService::g;
int LedService::b;
int LedService::brightness;

//JSON解析
void LedService::getLedJson(String msg){
    error = deserializeJson(doc, msg);
}

//LEDをセット
void LedService::setLedColor(){
    r = doc["R"] | 0;
    g = doc["G"] | 0;
    b = doc["B"] | 0;
    brightness = doc["Brightness"] | 255;
}


//個別コントロール
void LedService::handleIControl(String msg) {
    Serial.print("[I]");
}
//個別コントロール
void LedService::handleLed(String msg) {
    getLedJson(msg);
    if (error) {
        Serial.print("deserializeJson() failed: ");
        Serial.println(error.f_str());
        return;
    }

    setLedColor();

    Serial.printf("Set Color: R=%d, G=%d, B=%d\n", r, g, b);
    strip.setBrightness(brightness);
    for (int i = 0; i < NUM_LEDS; i++) {
        strip.setPixelColor(i, strip.Color(r, g, b));
    }
    delay(20);
    strip.show();
    delay(20);
}

//フェードインサービス
void LedService::ledFadein(String msg) {
    getLedJson(msg);
    if (error) {
        Serial.print("deserializeJson() failed: ");
        Serial.println(error.f_str());
        return;
    }

    setLedColor();

    for (int i = 0; i <= brightness; i++) {
        strip.setBrightness(i);
        for (int j = 0; j < NUM_LEDS; j++) {
            strip.setPixelColor(j, strip.Color(r, g, b));
        }
        strip.show();
        delay(20);
    }
}