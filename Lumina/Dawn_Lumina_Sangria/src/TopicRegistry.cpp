#include <PubSubClient.h>
#include "TopicRegistry.h"
#include "MessageController.h"
#include "LedService.h"

extern PubSubClient client; // mainのclientを参照

namespace TopicRegistry{
    void registerAll(const char* device_name){
        MessageController::registerHandler("dawn/led/all",LedService::handleLed);
        String topic = String("dawn/led/") + String(device_name);
        MessageController::registerHandler(topic.c_str(),LedService::handleLed);

        // subscribeもここでまとめる
        client.subscribe("dawn/led/all");
        client.subscribe(("dawn/led/" + String(device_name)).c_str());
    }
}