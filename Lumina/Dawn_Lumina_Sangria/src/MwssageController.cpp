#include "MessageController.h"

std::map<std::string,std::function<void(String)>> MessageController::handlers;

void MessageController::registerHandler(const std::string& topic,std::function<void(String)> handler){
    handlers[topic] = handler;
}

void MessageController::handleMessage(const char* topic,const char* payload){
    String msg = String(payload);
    auto it = handlers.find(std::string(topic));
    if(it != handlers.end()){
        it->second(msg);
    }else{
        Serial.print("No handler for topic: ");
        Serial.println(topic);
    }
}