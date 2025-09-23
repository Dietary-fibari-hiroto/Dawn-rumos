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
String LedService::mode;

void LedService::setJsonUtil(String msg){
    error = deserializeJson(doc, msg);
    if (error) {
        Serial.print("deserializeJson() failed: ");
        Serial.println(error.f_str());
        return;
    }
    r = doc["R"] | 0;
    g = doc["G"] | 0;
    b = doc["B"] | 0;
    brightness = doc["Brightness"] | 2;
    mode = doc["Mode"] | "normal";
}


//mode分岐
void LedService::modeBranch(String msg){
    if(mode == "normal") handleLed();
    else if(mode=="fadein") ledFadein();
    else if(mode=="whitegradient") WhiteGradient();
}
//個別コントロール
void LedService::handleLed() {
    strip.setBrightness(brightness);
    for (int i = 0; i < NUM_LEDS; i++) {
        strip.setPixelColor(i, strip.Color(r, g, b));
    }
    delay(20);
    strip.show();
    delay(20);
}

//フェードインサービス
void LedService::ledFadein() {

    for (int i = 0; i <= brightness; i++) {
        strip.setBrightness(i);
        for (int j = 0; j < NUM_LEDS; j++) {
            strip.setPixelColor(j, strip.Color(r, g, b));
        }
        strip.show();
        delay(20);
    }
}

// 色の補間（線形補間）
uint32_t lerpColor(uint32_t c1, uint32_t c2, float t) {
  uint8_t r1 = (uint8_t)(c1 >> 16);
  uint8_t g1 = (uint8_t)(c1 >>  8);
  uint8_t b1 = (uint8_t)(c1 >>  0);

  uint8_t r2 = (uint8_t)(c2 >> 16);
  uint8_t g2 = (uint8_t)(c2 >>  8);
  uint8_t b2 = (uint8_t)(c2 >>  0);

  uint8_t r = r1 + (r2 - r1) * t;
  uint8_t g = g1 + (g2 - g1) * t;
  uint8_t b = b1 + (b2 - b1) * t;

  return strip.Color(r, g, b);
}

void LedService::WhiteGradient(){
    int mostHightValue = max(r,max(g,b));

    uint32_t startColor = strip.Color(mostHightValue,mostHightValue,mostHightValue);
    uint32_t endColor   = strip.Color(r,g,b); // 白 (末尾)

    int fixedHead = 3;
    int fixedTail = 10;

    strip.setBrightness(brightness);

    for(int i = 0;i < NUM_LEDS;i++){
        if(i < fixedHead){
            strip.setPixelColor(i, startColor);
        }else if(i >= NUM_LEDS - fixedTail){
            strip.setPixelColor(i, endColor);
        }else{
            float t = (float)(i - fixedHead) / (float)(NUM_LEDS - fixedHead - fixedTail - 1);
            strip.setPixelColor(i, lerpColor(startColor,endColor,t));
        }
    }

    strip.show();
    delay(100);
}