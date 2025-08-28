#include <WiFi.h>
#include <PubSubClient.h>
#include <Adafruit_NeoPixel.h>
#include <ArduinoJson.h>
#include "secrets.h"
#include "DeviceMetaData.h"
#include "TopicRegistry.h"
#include "MessageController.h"

// ==== WiFi設定 ====
const char* ssid = WIFI_SSID;
const char* password = WIFI_PASSWORD;

// ==== MQTT設定 ====
const char* device_name = DEVICE_NAME;
const char* device_series = DEVICE_SERIES;
const char* mqtt_server = MQTT_SERVER;  // ←PCのLAN IP
const int mqtt_port = MQTT_PORT;

// ==== NeoPixel設定 ====
const int NUM_LEDS = CONST_NUM_LEDS;
const int LED_PIN = CONST_LED_PIN;
Adafruit_NeoPixel strip(NUM_LEDS, LED_PIN, NEO_GRB + NEO_KHZ800);

WiFiClient espClient;
PubSubClient client(espClient);

// ==== MQTTコールバック ====
void callback(char* topic, byte* payload, unsigned int length) {
  String msg;
  for (unsigned int i = 0; i < length; i++) {
    msg += (char)payload[i];
  }
  MessageController::handleMessage(topic, msg.c_str());
}

void reconnect() {
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    if (client.connect(DEVICE_NAME)) {
      Serial.println("connected");
      // TopicRegistry に任せて subscribe する
      TopicRegistry::registerAll(DEVICE_NAME);
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
  
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("WiFi connected");
  Serial.println(WiFi.localIP());
  
  // NeoPixel初期化
  strip.begin();
  strip.show();

  // MQTT設定
  client.setServer(mqtt_server, mqtt_port);
  client.setCallback(callback);

  TopicRegistry::registerAll(device_name);
}

void loop() {
  if (!client.connected()) {
    reconnect();
  }
  client.loop();
}
