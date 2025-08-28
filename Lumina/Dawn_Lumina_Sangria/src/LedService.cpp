#include "LedService.h"
#include <ArduinoJson.h>
#include <Adafruit_NeoPixel.h>
#include "DeviceMetaData.h"


const int NUM_LEDS = CONST_NUM_LEDS;
const int LED_PIN = CONST_LED_PIN;
extern Adafruit_NeoPixel strip; // mainで定義したものを参照
namespace LedService{
    void handleAll(String msg){
        Serial.print("device");
    }

    void handleIControl(String msg){
        Serial.print("[I]");
    }
    void handleLed(String msg){
        StaticJsonDocument<200> doc;
        DeserializationError error = deserializeJson(doc,msg);
        if(error){
            Serial.print("deserializeJson() failed: ");
            Serial.println(error.f_str());
            return;
        }

        int r = doc["R"] | 0;
        int g = doc["G"] | 0;
        int b = doc["B"] | 0;
        int brightness = doc["Brightness"] | 100;

        Serial.printf("Set Color: R=%d, G=%d, B=%d\n", r, g, b);
strip.setBrightness(brightness);
            for (int i = 0; i < NUM_LEDS; i++) {
      strip.setPixelColor(i, strip.Color(r, g, b));
    }
    delay(20); 
    strip.show();
    delay(20); 
    }
}