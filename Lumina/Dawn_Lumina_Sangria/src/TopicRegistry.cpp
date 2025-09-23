#include <PubSubClient.h>
#include "TopicRegistry.h"
#include "MessageController.h"
#include "./services/LedService.h"

/**
 * ここでTopicと呼び出されるべきService関数への紐づけを行う
 */
extern PubSubClient client; // mainのclientを参照

namespace TopicRegistry{
    void registerAll(const char* device_name){
        MessageController::registerHandler("dawn/led/all",LedService::modeBranch);
        String topic = String("dawn/led/") + String(device_name);
        MessageController::registerHandler(topic.c_str(),LedService::modeBranch);

        //String fadeTopic = topic + String("/fadein");
        //MessageController::registerHandler(fadeTopic.c_str(),LedService::WhiteGradient);

        // subscribeもここでまとめる
        client.subscribe("dawn/led/all");
        client.subscribe(("dawn/led/" + String(device_name)).c_str());
        client.subscribe(("dawn/led/"+String(device_name)+"/fadein").c_str());
    }
}