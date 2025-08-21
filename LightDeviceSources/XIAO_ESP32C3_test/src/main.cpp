#include <WiFi.h>
#include <PubSubClient.h>
#include <Adafruit_NeoPixel.h>
#include <ArduinoJson.h>
#include "secrets.h"  // WiFiとMQTTの情報を定義したヘッダ

// ==== WiFi設定 ====
const char* ssid = WIFI_SSID;
const char* password = WIFI_PASSWORD;

// ==== MQTT設定 ====
const char* mqtt_server = MQTT_SERVER;   // ←PCのLAN IP
const int   mqtt_port   = MQTT_PORT;
const char* mqtt_topic  = "esp32/led";   // ←ASP.NET 側と合わせる

// ==== NeoPixel設定 ====
// XIAO ESP32C3 は D10 (GPIO10) が NeoPixel によく使われるピン
// もし外付けLEDなら配線ピンに合わせて変更する
#define LED_PIN    0   // ← GPIO10 (D10 ピン)
#define NUM_LEDS   5

Adafruit_NeoPixel strip(NUM_LEDS, LED_PIN, NEO_GRB + NEO_KHZ800);

WiFiClient espClient;
PubSubClient client(espClient);

// ==== 受信時の処理 ====
void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Message arrived [");
  Serial.print(topic);
  Serial.print("] ");
  String msg;
  for (unsigned int i = 0; i < length; i++) {
    msg += (char)payload[i];
  }
  Serial.println(msg);

  // JSON解析
  StaticJsonDocument<256> doc;  // ← バッファ少し大きめ
  DeserializationError error = deserializeJson(doc, msg);
  if (error) {
    Serial.print("deserializeJson() failed: ");
    Serial.println(error.f_str());
    return;
  }

  int r = doc["R"] | 0;
  int g = doc["G"] | 0;
  int b = doc["B"] | 0;
  int brightness = doc["Brightness"] | 100;

  Serial.printf("Set Color: R=%d, G=%d, B=%d Brightness=%d\n", r, g, b, brightness);

  // 明るさを先にセット
  strip.setBrightness(brightness);

  // 全部のLEDに反映
  for (int i = 0; i < NUM_LEDS; i++) {
    strip.setPixelColor(i, strip.Color(r, g, b));
  }
  strip.show();
}

// ==== MQTT再接続 ====
void reconnect() {
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    if (client.connect("XIAO_ESP32C3_Client")) {
      Serial.println("connected");
      client.subscribe(mqtt_topic);
      Serial.print("Subscribed to: ");
      Serial.println(mqtt_topic);
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      delay(5000);
    }
  }
}

void setup() {
  Serial.begin(115200);

  // WiFi接続
  WiFi.begin(ssid, password);
  WiFi.setTxPower(WIFI_POWER_8_5dBm);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("WiFi connected");
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());

  // NeoPixel初期化
  strip.begin();
  strip.clear();
  strip.show();

  // MQTT設定
  client.setServer(mqtt_server, mqtt_port);
  client.setCallback(callback);
}

void loop() {
  if (!client.connected()) {
    reconnect();
  }
  client.loop();
}
