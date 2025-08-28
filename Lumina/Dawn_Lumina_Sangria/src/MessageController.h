#pragma once
#include <map>
#include <string>
#include <functional>
#include <Arduino.h>

class MessageController{
public:
static void registerHandler(const std::string& topic, std::function<void(String)> handler);
static void handleMessage(const char* topic,const char* payload);
private:
static std::map<std::string, std::function<void(String)>> handlers;
};