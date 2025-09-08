#include <PubSubClient.h>
#include "TopicRegistry.h"
#include "MessageController.h"
#include "LedService.h"

/**
 * ここでTopicと呼び出されるべきService関数への紐づけを行う
 */
extern PubSubClient client; // mainのclientを参照

namespace TopicRegistry{
    void registerAll(const char* device_name){
        MessageController::registerHandler("dawn/led/all",LedService::handleLed);
        String topic = String("dawn/led/") + String(device_name);
        MessageController::registerHandler(topic.c_str(),LedService::handleLed);

        String fadeTopic = topic + String("/fadein");
        MessageController::registerHandler(fadeTopic.c_str(),LedService::ledFadein);

        // subscribeもここでまとめる
        client.subscribe("dawn/led/all");
        client.subscribe(("dawn/led/" + String(device_name)).c_str());
        client.subscribe(("dawn/led/"+String(device_name)+"/fadein").c_str());
    }
}