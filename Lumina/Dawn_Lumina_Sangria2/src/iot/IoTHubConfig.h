#pragma once

#include <Arduino.h>

struct IoTHubConfig {
    const char* hostName;
    const char* deviceId;
    const char* deviceKey;
    uint32_t sasTokenExpirySecs;
};