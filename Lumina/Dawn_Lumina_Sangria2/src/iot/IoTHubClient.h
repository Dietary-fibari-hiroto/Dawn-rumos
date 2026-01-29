#pragma once

#include <Arduino.h>
#include <WiFiClientSecure.h>
#include <PubSubClient.h>
#include <az_core.h>
#include <az_iot.h>
#include "IoTHubConfig.h"

// コールバック型定義
typedef void (*C2DMessageCallback)(const char* payload, size_t length);

class IoTHubClient {
public:
    IoTHubClient(const IoTHubConfig& config);
    
    bool begin();
    void loop();
    bool isConnected();
    
    // C2Dメッセージ受信コールバック登録
    void onC2DMessage(C2DMessageCallback callback);
    
    // D2Cメッセージ送信（テレメトリ）
    bool sendTelemetry(const char* payload);
    
    // D2Cメッセージ送信（プロパティ付き）
    bool sendTelemetry(const char* payload, const char* properties);

private:
    IoTHubConfig _config;
    WiFiClientSecure _wifiClient;
    PubSubClient _mqttClient;
    
    az_iot_hub_client _azClient;
    char _azClientId[128];
    char _azUsername[256];
    char _azC2DTopic[128];
    char _azTelemetryTopic[128];
    char _sasToken[512];
    
    C2DMessageCallback _c2dCallback = nullptr;
    
    unsigned long _sasTokenExpireTime = 0;
    unsigned long _lastReconnectAttempt = 0;
    
    bool initAzureClient();
    bool generateSasToken();
    bool connectMqtt();
    void subscribeToC2D();
    
    static void mqttCallback(char* topic, byte* payload, unsigned int length);
    static IoTHubClient* _instance; // コールバック用
};