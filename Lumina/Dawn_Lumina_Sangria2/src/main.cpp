#include <WiFi.h>
#include <Adafruit_NeoPixel.h>
#include <ArduinoJson.h>
#include <time.h>
#include "secrets.h"
#include "DeviceMetaData.h"
#include "iot/IoTHubClient.h"
#include "services/LedService.h"

// ==== NeoPixel ====
const int NUM_LEDS = CONST_NUM_LEDS;
const int LED_PIN = CONST_LED_PIN;
Adafruit_NeoPixel strip(NUM_LEDS, LED_PIN, NEO_GRB + NEO_KHZ800);

// ==== IoT Hub Client ====
IoTHubConfig iotConfig = {
    .hostName = IOT_HUB_HOSTNAME,
    .deviceId = IOT_DEVICE_ID,
    .deviceKey = IOT_DEVICE_KEY,
    .sasTokenExpirySecs = SAS_TOKEN_EXPIRY_SECONDS
};
IoTHubClient iotClient(iotConfig);

//WiFi接続
void connectWiFi() {
    Serial.print("Connecting to WiFi");
    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
    
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
    }
    
    Serial.println("\nWiFi connected!");
    Serial.printf("IP: %s\n", WiFi.localIP().toString().c_str());
}

void syncTime() {
    Serial.print("Syncing time");
    configTime(0, 0, "pool.ntp.org", "time.nist.gov");
    
    time_t now = time(nullptr);
    while (now < 1609459200) { 
        delay(500);
        Serial.print(".");
        now = time(nullptr);
    }
    
    Serial.printf("\nTime synced: %s", ctime(&now));
}

// ==== C2Dメッセージ受信コールバック ====
void onC2DMessage(const char* payload, size_t length) {
    String msg;
    for (size_t i = 0; i < length; i++) {
        msg += payload[i];
    }
    
    Serial.printf("[C2D] Received: %s\n", msg.c_str());
    
    // LedServiceで処理
    LedService::setJsonUtil(msg);
    LedService::modeBranch(msg);
}

// ==== 状態レポート送信 ====
void sendStateReport() {
    JsonDocument doc;
    doc["deviceId"] = DEVICE_NAME;
    doc["series"] = DEVICE_SERIES;
    doc["status"] = "online";
    doc["rssi"] = WiFi.RSSI();
    
    char buffer[256];
    serializeJson(doc, buffer, sizeof(buffer));
    
    iotClient.sendTelemetry(buffer);
}

void setup() {
    Serial.begin(115200);
    Serial.println("\n=== Rumos IoT Hub Device ===");
    
    // NeoPixel初期化
    strip.begin();
    strip.show();
    
    // WiFi接続
    connectWiFi();
    
    // NTP時刻同期
    syncTime();
    
    // IoT Hub接続
    iotClient.onC2DMessage(onC2DMessage);
    if (iotClient.begin()) {
        Serial.println("IoT Hub ready!");
        sendStateReport();
    } else {
        Serial.println("IoT Hub init failed!");
    }
}

void loop() {
    // WiFi再接続
    if (WiFi.status() != WL_CONNECTED) {
        Serial.println("WiFi disconnected, reconnecting...");
        connectWiFi();
    }
    
    // IoT Hub処理
    iotClient.loop();
    
    // 定期的に状態レポート（5分ごと）
    static unsigned long lastReport = 0;
    if (millis() - lastReport > 300000) {
        lastReport = millis();
        sendStateReport();
    }
}