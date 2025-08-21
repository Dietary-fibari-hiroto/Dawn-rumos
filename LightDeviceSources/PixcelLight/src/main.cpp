#include <Arduino.h>

// put function declarations here:
#include <Adafruit_NeoPixel.h>

#define LED_PIN     18    // A3に接続
#define NUM_LEDS    22    // LEDの個数
#define BRIGHTNESS  25    // 明るさ（0〜255）

//実際に使うLEDの数
int active_leds  = 22;

Adafruit_NeoPixel strip(NUM_LEDS, LED_PIN, NEO_GRB + NEO_KHZ800);

void setup() {
  Serial.begin(115200);
  strip.begin();

  strip.setBrightness(BRIGHTNESS);
  strip.clear();
  strip.show();
  delay(50);


  // 全LEDを暖色（オレンジ寄り赤）で点灯
  for (int i = 0; i < NUM_LEDS; i++) {
    
    strip.setPixelColor(i, strip.Color(255, 100, 0)); // R:255 G:100 B:0
    //strip.setPixelColor(i, strip.Color(255, 255, 255));
  }
  for (int i = active_leds; i < NUM_LEDS;i++){
    strip.setPixelColor(i,strip.Color(0,0,0));
  }
  strip.show();
}
void loop() {
  Serial.println("テストコンソール");
  delay(3000);
}
