#include <Arduino.h>

// put function declarations here:
#include <Adafruit_NeoPixel.h>

#define LED_PIN     18    // A3に接続
#define NUM_LEDS    5     // LEDの個数
#define BRIGHTNESS  50    // 明るさ（0〜255）

Adafruit_NeoPixel strip(NUM_LEDS, LED_PIN, NEO_GRB + NEO_KHZ800);

void setup() {
  strip.begin();
  strip.setBrightness(BRIGHTNESS);

  // 全LEDを暖色（オレンジ寄り赤）で点灯
  for (int i = 0; i < NUM_LEDS; i++) {
    strip.setPixelColor(i, strip.Color(255, 255, 255)); // R:255 G:100 B:0
  }
  strip.show();
}
void loop() {

}
