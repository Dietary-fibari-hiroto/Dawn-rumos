#include "IoTHubClient.h"
#include <base64.h>
#include <mbedtls/md.h>
#include <mbedtls/base64.h>

// DigiCert Global Root G2 (Azure IoT Hub用)
static const char* ROOT_CA = R"EOF(
-----BEGIN CERTIFICATE-----
MIIDjjCCAnagAwIBAgIQAzrx5qcRqaC7KGSxHQn65TANBgkqhkiG9w0BAQsFADBh
MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
d3cuZGlnaWNlcnQuY29tMSAwHgYDVQQDExdEaWdpQ2VydCBHbG9iYWwgUm9vdCBH
MjAeFw0xMzA4MDExMjAwMDBaFw0zODAxMTUxMjAwMDBaMGExCzAJBgNVBAYTAlVT
MRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5j
b20xIDAeBgNVBAMTF0RpZ2lDZXJ0IEdsb2JhbCBSb290IEcyMIIBIjANBgkqhkiG
9w0BAQEFAAOCAQ8AMIIBCgKCAQEAuzfNNNx7a8myaJCtSnX/RrohCgiN9RlUyfuI
2/Ou8jqJkTx65qsGGmvPrC3oXgkkRLpimn7Wo6h+4FR1IAWsULecYxpsMNzaHxmx
1x7e/dfgy5SDN67sH0NO3Xss0r0upS/kqbitOtSZpLYl6ZtrAGCSYP9PIUkY92eQ
q2EGnI/yuum06ZIya7XzV+hdG82MHauVBJVJ8zUtluNJbd134/tJS7SsVQepj5Wz
tCO7TG1F8PapspUwtP1MVYwnSlcUfIKdzXOS0xZKBgyMUNGPHgm+F6HmIcr9g+UQ
vIOlCsRnKPZzFBQ9RnbDhxSJITRNrw9FDKZJobq7nMWxM4MphQIDAQABo0IwQDAP
BgNVHRMBAf8EBTADAQH/MA4GA1UdDwEB/wQEAwIBhjAdBgNVHQ4EFgQUTiJUIBiV
5uNu5g/6+rkS7QYXjzkwDQYJKoZIhvcNAQELBQADggEBAGBnKJRvDkhj6zHd6mcY
1Yl9PMCcit6E7dBE2gtbiCghzsi1zKpLi7Nt8vZ5PzLZ+dCnU7xkKNUO5Yn9TyN6
b9qPFWrcZ4Iq7kFFYbCk5/VFPy+j91SXhIL9oxXxExhFwEt78P/MNfTHbqCLbVlL
5jzAIFjqq5UkZqMX6jPU5OejT8FNtXVreCWMbCCL5q3FyAqgFqdCnC7V6sxLEkfN
HGNOb7xm6iW6kMCiGz4M3+7bPhh9r+H9EZS9ljF0RHDL/XEfjJw/MfgXF1Tyqf/R
OnVqKhtTL/Q6IINJxO4gSJk6y8c6xoLlOL+i6Py4c3Z69Kv+0DChfZ2rRDksqh/B
m3c=
-----END CERTIFICATE-----
)EOF";

// static instance for callback
IoTHubClient* IoTHubClient::_instance = nullptr;

IoTHubClient::IoTHubClient(const IoTHubConfig& config)
    : _config(config), _mqttClient(_wifiClient) {
    _instance = this;
}

bool IoTHubClient::begin() {
    Serial.println("[IoT Hub] Initializing...");
    
    // TLS設定
    Serial.println("[IoT Hub] Setting CA cert...");
    _wifiClient.setCACert(ROOT_CA);
    
    // Azure IoT Client初期化
    Serial.println("[IoT Hub] Initializing Azure client...");
    if (!initAzureClient()) {
        Serial.println("[IoT Hub] Failed to init Azure client");
        return false;
    }
    Serial.println("[IoT Hub] Azure client initialized OK");
    
    // SASトークン生成
    Serial.println("[IoT Hub] Generating SAS token...");
    if (!generateSasToken()) {
        Serial.println("[IoT Hub] Failed to generate SAS token");
        return false;
    }
    Serial.println("[IoT Hub] SAS token generated OK");
    
    // MQTT設定
    Serial.printf("[IoT Hub] Setting up MQTT to %s:8883...\n", _config.hostName);
    _mqttClient.setServer(_config.hostName, 8883);
    _mqttClient.setBufferSize(1024);
    _mqttClient.setCallback(mqttCallback);
    _mqttClient.setKeepAlive(120);
    
    // 接続
    Serial.println("[IoT Hub] Connecting to MQTT...");
    return connectMqtt();
}

bool IoTHubClient::initAzureClient() {
    Serial.printf("[IoT Hub] HostName: %s\n", _config.hostName);
    Serial.printf("[IoT Hub] DeviceId: %s\n", _config.deviceId);
    
    az_span hostname = az_span_create_from_str((char*)_config.hostName);
    az_span deviceId = az_span_create_from_str((char*)_config.deviceId);
    
    Serial.println("[IoT Hub] Calling az_iot_hub_client_init...");
    az_result result = az_iot_hub_client_init(
        &_azClient,
        hostname,
        deviceId,
        NULL);
    
    if (az_result_failed(result)) {
        Serial.printf("[IoT Hub] az_iot_hub_client_init failed: 0x%08x\n", result);
        return false;
    }
    Serial.println("[IoT Hub] az_iot_hub_client_init OK");
    
    // Client ID取得
    Serial.println("[IoT Hub] Getting client ID...");
    size_t clientIdLen;
    result = az_iot_hub_client_get_client_id(
        &_azClient,
        _azClientId,
        sizeof(_azClientId),
        &clientIdLen);
    
    if (az_result_failed(result)) {
        Serial.printf("[IoT Hub] get_client_id failed: 0x%08x\n", result);
        return false;
    }
    Serial.printf("[IoT Hub] ClientId: %s\n", _azClientId);
    
    // Username取得
    Serial.println("[IoT Hub] Getting username...");
    size_t usernameLen;
    result = az_iot_hub_client_get_user_name(
        &_azClient,
        _azUsername,
        sizeof(_azUsername),
        &usernameLen);
    
    if (az_result_failed(result)) {
        Serial.printf("[IoT Hub] get_user_name failed: 0x%08x\n", result);
        return false;
    }
    Serial.printf("[IoT Hub] Username: %s\n", _azUsername);
    
    snprintf(_azC2DTopic, sizeof(_azC2DTopic),
        "devices/%s/messages/devicebound/#", _config.deviceId);
    
    snprintf(_azTelemetryTopic, sizeof(_azTelemetryTopic),
        "devices/%s/messages/events/", _config.deviceId);
    
    Serial.printf("[IoT Hub] C2D Topic: %s\n", _azC2DTopic);
    Serial.printf("[IoT Hub] Telemetry Topic: %s\n", _azTelemetryTopic);
    
    return true;
}

bool IoTHubClient::generateSasToken() {
    // 有効期限（Unix時間）
    uint32_t expiry = (uint32_t)(time(nullptr)) + _config.sasTokenExpirySecs;
    _sasTokenExpireTime = millis() + (_config.sasTokenExpirySecs * 1000);
    
    // 署名対象文字列: {hostname}/devices/{deviceId}\n{expiry}
    char signatureString[256];
    snprintf(signatureString, sizeof(signatureString),
        "%s%%2Fdevices%%2F%s\n%u",
        _config.hostName, _config.deviceId, expiry);
    
    // Base64デコードされたキー
    unsigned char decodedKey[64];
    size_t decodedKeyLen = 0;
    
    int ret = mbedtls_base64_decode(
        decodedKey, sizeof(decodedKey), &decodedKeyLen,
        (const unsigned char*)_config.deviceKey,
        strlen(_config.deviceKey));
    
    if (ret != 0) {
        Serial.printf("[IoT Hub] Base64 decode failed: %d\n", ret);
        return false;
    }
    
    // HMAC-SHA256
    unsigned char hmacResult[32];
    mbedtls_md_context_t ctx;
    mbedtls_md_init(&ctx);
    mbedtls_md_setup(&ctx, mbedtls_md_info_from_type(MBEDTLS_MD_SHA256), 1);
    mbedtls_md_hmac_starts(&ctx, decodedKey, decodedKeyLen);
    mbedtls_md_hmac_update(&ctx, (const unsigned char*)signatureString, strlen(signatureString));
    mbedtls_md_hmac_finish(&ctx, hmacResult);
    mbedtls_md_free(&ctx);
    
    // Base64エンコード
    unsigned char signatureBase64[64];
    size_t signatureBase64Len = 0;
    mbedtls_base64_encode(
        signatureBase64, sizeof(signatureBase64), &signatureBase64Len,
        hmacResult, sizeof(hmacResult));
    
    // URLエンコード
    String signatureEncoded = "";
    for (size_t i = 0; i < signatureBase64Len; i++) {
        char c = signatureBase64[i];
        if (c == '+') signatureEncoded += "%2B";
        else if (c == '/') signatureEncoded += "%2F";
        else if (c == '=') signatureEncoded += "%3D";
        else signatureEncoded += c;
    }
    
    // SASトークン組み立て
    snprintf(_sasToken, sizeof(_sasToken),
        "SharedAccessSignature sr=%s%%2Fdevices%%2F%s&sig=%s&se=%u",
        _config.hostName, _config.deviceId,
        signatureEncoded.c_str(), expiry);
    
    Serial.println("[IoT Hub] SAS token generated");
    return true;
}

bool IoTHubClient::connectMqtt() {
    Serial.printf("[IoT Hub] Connecting to %s...\n", _config.hostName);
    
    if (_mqttClient.connect(_azClientId, _azUsername, _sasToken)) {
        Serial.println("[IoT Hub] Connected!");
        subscribeToC2D();
        return true;
    } else {
        Serial.printf("[IoT Hub] Connect failed, rc=%d\n", _mqttClient.state());
        return false;
    }
}

void IoTHubClient::subscribeToC2D() {
    if (_mqttClient.subscribe(_azC2DTopic)) {
        Serial.printf("[IoT Hub] Subscribed to: %s\n", _azC2DTopic);
    } else {
        Serial.println("[IoT Hub] Subscribe failed");
    }
}

void IoTHubClient::loop() {
    // SASトークン更新チェック（期限の5分前に更新）
    if (millis() > _sasTokenExpireTime - 300000) {
        Serial.println("[IoT Hub] Refreshing SAS token...");
        generateSasToken();
        _mqttClient.disconnect();
    }
    
    // 再接続
    if (!_mqttClient.connected()) {
        unsigned long now = millis();
        if (now - _lastReconnectAttempt > 5000) {
            _lastReconnectAttempt = now;
            Serial.println("[IoT Hub] Reconnecting...");
            if (connectMqtt()) {
                _lastReconnectAttempt = 0;
            }
        }
    }
    
    _mqttClient.loop();
}

bool IoTHubClient::isConnected() {
    return _mqttClient.connected();
}

void IoTHubClient::onC2DMessage(C2DMessageCallback callback) {
    _c2dCallback = callback;
}

bool IoTHubClient::sendTelemetry(const char* payload) {
    return sendTelemetry(payload, nullptr);
}

bool IoTHubClient::sendTelemetry(const char* payload, const char* properties) {
    if (!_mqttClient.connected()) {
        Serial.println("[IoT Hub] Not connected, cannot send");
        return false;
    }
    
    char topic[256];
    if (properties) {
        snprintf(topic, sizeof(topic), "%s%s", _azTelemetryTopic, properties);
    } else {
        strncpy(topic, _azTelemetryTopic, sizeof(topic));
    }
    
    bool success = _mqttClient.publish(topic, payload);
    if (success) {
        Serial.printf("[IoT Hub] Telemetry sent: %s\n", payload);
    } else {
        Serial.println("[IoT Hub] Telemetry send failed");
    }
    return success;
}

// Static callback
void IoTHubClient::mqttCallback(char* topic, byte* payload, unsigned int length) {
    Serial.printf("[IoT Hub] Message received on: %s\n", topic);
    
    if (_instance && _instance->_c2dCallback) {
        _instance->_c2dCallback((const char*)payload, length);
    }
}