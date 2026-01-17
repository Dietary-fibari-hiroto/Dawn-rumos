#include <M5StickCPlus2.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>
#include "config.h"

// 振り方向の列挙型
enum ShakeDirection {
    SHAKE_NONE,
    SHAKE_DOWN,//縦下(魔法発動)
    SHAKE_RIGHT,//横右(次のルーチン)
    SHAKE_UP,//縦上(全消灯)
    SHAKE_LEFT//横左(ルーチン再取得)
};

//MagicRoutinの構造体
struct MagicRoutin {
    int id;
    String name;
    String imgUrl;
};

//グローバル変数
std::vector<MagicRoutin> magicRoutins;
int currentRoutinIndex = 0;
unsigned long lastShakeTime = 0;
bool isConnected = false;
int wifiRetryCount = 0;
int apiRetryCount = 0;

//関数プロトタイプ
void connectWiFi();
bool fetchMagicRoutins();
void displayCurrentRoutin();
ShakeDirection detectShake();
bool executeMagicRoutin(int id);
bool turnOffAllLights();
void showError(const String& message);

void setup() {
    auto cfg = M5.config();
    M5.begin(cfg);
    
    //シリアル通信開始
    Serial.begin(115200);
    Serial.println("Magic Wand Starting...");
    
    M5.Lcd.setRotation(1);
    M5.Lcd.fillScreen(BLACK);
    M5.Lcd.setTextSize(2);
    M5.Lcd.setTextColor(WHITE);
    
    //起動メッセージ
    M5.Lcd.setCursor(10, 10);
    M5.Lcd.println("Magic Wand");
    M5.Lcd.setCursor(10, 30);
    M5.Lcd.println("Starting...");
    
    //WiFi接続
    connectWiFi();
    
    if (isConnected) {
        //MagicRoutin一覧を取得
        M5.Lcd.setCursor(10, 50);
        M5.Lcd.println("Loading...");
        
        if (fetchMagicRoutins()) {
            displayCurrentRoutin();
            Serial.println("Ready! Start shaking!");
        } else {
            showError("Failed to load");
        }
    }
}

void loop() {
    M5.update();
    
    if (!isConnected) {
        return;
    }
    
    //クールダウン期間中は検知しない
    if (millis() - lastShakeTime < SHAKE_COOLDOWN_MS) {
        delay(10);
        return;
    }
    
    //振り検知
    ShakeDirection shake = detectShake();
    
    if (shake != SHAKE_NONE) {
        lastShakeTime = millis();
        
        M5.Lcd.fillScreen(BLACK);
        M5.Lcd.setCursor(10, 10);
        
        switch (shake) {
            case SHAKE_DOWN:
                //縦下:魔法発動
                Serial.println("=== ACTION: SHAKE_DOWN (Magic Routine) ===");
                M5.Lcd.setTextColor(YELLOW);
                M5.Lcd.println("Casting!");
                if (magicRoutins.size() > 0) {
                    int routinId = magicRoutins[currentRoutinIndex].id;
                    Serial.printf("Executing Magic Routine ID: %d\n", routinId);
                    if (executeMagicRoutin(routinId)) {
                        M5.Lcd.fillScreen(BLACK);
                        M5.Lcd.setCursor(10, 10);
                        M5.Lcd.setTextColor(GREEN);
                        M5.Lcd.println("Success!");
                        M5.Lcd.setTextColor(WHITE);
                        M5.Lcd.setCursor(10, 35);
                        M5.Lcd.println(magicRoutins[currentRoutinIndex].name);
                        delay(1500);
                    }
                }
                displayCurrentRoutin();
                break;
                
            case SHAKE_RIGHT:
                //横右:次のルーチン
                Serial.println("=== ACTION: SHAKE_RIGHT (Next Routine) ===");
                if (magicRoutins.size() > 0) {
                    currentRoutinIndex = (currentRoutinIndex + 1) % magicRoutins.size();
                    Serial.printf("Switched to routine: %s (ID: %d)\n", 
                                  magicRoutins[currentRoutinIndex].name.c_str(),
                                  magicRoutins[currentRoutinIndex].id);
                    displayCurrentRoutin();
                }
                break;
                
            case SHAKE_UP:
                //縦上:全消灯
                Serial.println("=== ACTION: SHAKE_UP (All Lights Off) ===");
                M5.Lcd.setTextColor(BLUE);
                M5.Lcd.println("Lights Off");
                if (turnOffAllLights()) {
                    M5.Lcd.fillScreen(BLACK);
                    M5.Lcd.setCursor(10, 10);
                    M5.Lcd.setTextColor(GREEN);
                    M5.Lcd.println("All Off!");
                    delay(1500);
                }
                displayCurrentRoutin();
                break;
                
            case SHAKE_LEFT:
                //横左:ルーチン再取得
                Serial.println("=== ACTION: SHAKE_LEFT (Reload Routines) ===");
                M5.Lcd.setTextColor(MAGENTA);
                M5.Lcd.println("Reloading...");
                delay(500);
                apiRetryCount = 0;//リトライカウントをリセット
                if (fetchMagicRoutins()) {
                    currentRoutinIndex = 0;
                    M5.Lcd.fillScreen(BLACK);
                    M5.Lcd.setCursor(10, 10);
                    M5.Lcd.setTextColor(GREEN);
                    M5.Lcd.println("Reloaded!");
                    M5.Lcd.setTextColor(WHITE);
                    M5.Lcd.setCursor(10, 35);
                    M5.Lcd.printf("%d routins", magicRoutins.size());
                    delay(1500);
                    displayCurrentRoutin();
                }
                break;
                
            default:
                break;
        }
    }
    
    delay(10);
}

void connectWiFi() {
    M5.Lcd.setCursor(10, 50);
    M5.Lcd.println("WiFi...");
    
    WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
    
    unsigned long startTime = millis();
    while (WiFi.status() != WL_CONNECTED && millis() - startTime < WIFI_TIMEOUT_MS) {
        delay(500);
        M5.Lcd.print(".");
    }
    
    if (WiFi.status() == WL_CONNECTED) {
        isConnected = true;
        wifiRetryCount = 0;//リセット
        M5.Lcd.fillScreen(BLACK);
        M5.Lcd.setCursor(10, 10);
        M5.Lcd.setTextColor(GREEN);
        M5.Lcd.println("Connected!");
        M5.Lcd.setTextColor(WHITE);
        M5.Lcd.setCursor(10, 30);
        M5.Lcd.println(WiFi.localIP());
        delay(1500);
    } else {
        isConnected = false;
        wifiRetryCount++;
        
        M5.Lcd.fillScreen(BLACK);
        M5.Lcd.setCursor(10, 10);
        M5.Lcd.setTextColor(RED);
        M5.Lcd.println("WiFi Failed");
        M5.Lcd.setTextColor(WHITE);
        M5.Lcd.setCursor(10, 35);
        M5.Lcd.printf("Retry %d/%d", wifiRetryCount, MAX_RETRY_COUNT);
        
        if (wifiRetryCount >= MAX_RETRY_COUNT) {
            M5.Lcd.setCursor(10, 55);
            M5.Lcd.setTextColor(RED);
            M5.Lcd.println("Power Off...");
            delay(2000);
            M5.Power.powerOff();
        } else {
            unsigned long retryDelay = RETRY_DELAY_MS + (wifiRetryCount - 1) * RETRY_DELAY_INCREMENT_MS;
            M5.Lcd.setCursor(10, 55);
            M5.Lcd.printf("Wait %ds", retryDelay / 1000);
            delay(retryDelay);
            connectWiFi();//再試行
        }
    }
}

bool fetchMagicRoutins() {
    if (!isConnected) return false;
    
    HTTPClient http;
    String url = String(API_BASE_URL) + "/MagicRoutin";
    
    http.begin(url);
    http.addHeader("X-API-Key", API_KEY);
    http.setTimeout(HTTP_TIMEOUT_MS);
    
    int httpCode = http.GET();
    
    if (httpCode == 200) {
        String payload = http.getString();

        
        //JSONをパース
        DynamicJsonDocument doc(4096);
        DeserializationError error = deserializeJson(doc, payload);
        
        if (error) {
            Serial.print("JSON Parse Error: ");
            Serial.println(error.c_str());
            http.end();
            apiRetryCount++;
            
            if (apiRetryCount >= MAX_RETRY_COUNT) {
                showError("Parse Error");
                M5.Lcd.setCursor(10, 55);
                M5.Lcd.println("Power Off...");
                delay(2000);
                M5.Power.powerOff();
                return false;
            }
            
            showError("Retry Parse");
            delay(RETRY_DELAY_MS + (apiRetryCount - 1) * RETRY_DELAY_INCREMENT_MS);
            return fetchMagicRoutins();
        }
        
        //既存のリストをクリア
        magicRoutins.clear();
        
        //配列をパース
        JsonArray array = doc.as<JsonArray>();
        Serial.printf("Array size: %d\n", array.size());
        
        for (JsonObject obj : array) {
            MagicRoutin routin;
            //サーバーは小文字のフィールド名を返す
            routin.id = obj["id"];
            routin.name = obj["name"].as<String>();
            routin.imgUrl = obj["img_url"].as<String>();
            
            Serial.printf("Parsed: ID=%d, Name=%s, Img=%s\n", 
                          routin.id, routin.name.c_str(), routin.imgUrl.c_str());
            
            magicRoutins.push_back(routin);
        }
        
        //IDでソート(昇順)
        std::sort(magicRoutins.begin(), magicRoutins.end(), 
            [](const MagicRoutin& a, const MagicRoutin& b) {
                return a.id < b.id;
            });
        
        Serial.printf("Total routines loaded: %d\n", magicRoutins.size());
        
        http.end();
        apiRetryCount = 0;  //成功したのでリセット
        return magicRoutins.size() > 0;
    }
    
    http.end();
    apiRetryCount++;
    
    if (apiRetryCount >= MAX_RETRY_COUNT) {
        showError("API Failed");
        M5.Lcd.setCursor(10, 55);
        M5.Lcd.println("Power Off...");
        delay(2000);
        M5.Power.powerOff();
        return false;
    }
    
    M5.Lcd.fillScreen(BLACK);
    M5.Lcd.setCursor(10, 10);
    M5.Lcd.setTextColor(RED);
    M5.Lcd.println("API Failed");
    M5.Lcd.setTextColor(WHITE);
    M5.Lcd.setCursor(10, 35);
    M5.Lcd.printf("HTTP: %d", httpCode);
    M5.Lcd.setCursor(10, 55);
    M5.Lcd.printf("Retry %d/%d", apiRetryCount, MAX_RETRY_COUNT);
    
    delay(RETRY_DELAY_MS + (apiRetryCount - 1) * RETRY_DELAY_INCREMENT_MS);
    return fetchMagicRoutins();
}

void displayCurrentRoutin() {
    M5.Lcd.fillScreen(BLACK);
    M5.Lcd.setTextSize(2);
    M5.Lcd.setCursor(10, 10);
    M5.Lcd.setTextColor(CYAN);
    M5.Lcd.println("Current:");
    
    if (magicRoutins.size() > 0) {
        M5.Lcd.setTextColor(WHITE);
        M5.Lcd.setCursor(10, 35);
        
        String displayName = magicRoutins[currentRoutinIndex].name;
        //長すぎる場合は省略
        if (displayName.length() > 12) {
            displayName = displayName.substring(0, 12) + "...";
        }
        M5.Lcd.println(displayName);
        
        //ID表示
        M5.Lcd.setTextSize(1);
        M5.Lcd.setCursor(10, 65);
        M5.Lcd.setTextColor(DARKGREY);
        M5.Lcd.printf("ID:%d (%d/%d)", 
            magicRoutins[currentRoutinIndex].id,
            currentRoutinIndex + 1,
            magicRoutins.size());
    } else {
        M5.Lcd.setTextColor(RED);
        M5.Lcd.setCursor(10, 35);
        M5.Lcd.println("No Routins");
    }
}

ShakeDirection detectShake() {
    auto imu_update = M5.Imu.update();
    if (!imu_update) {
        return SHAKE_NONE;
    }
    
    auto data = M5.Imu.getImuData();
    
    float ax = data.accel.x;
    float ay = data.accel.y;
    float az = data.accel.z;
    
    //重力を引いた加速度の大きさを計算(手首のスナップを検出)
    //静止時は重力(約9.8m/s^2)のみなので、急激な動きで大きく変化する
    float accelMagnitude = sqrt(ax * ax + ay * ay + az * az);
    
    //デバッグ出力(加速度の値を常に表示)
    static unsigned long lastPrintTime = 0;
    if (millis() - lastPrintTime > 500) {  //500msごとに表示
        Serial.printf("Accel: X=%.2f Y=%.2f Z=%.2f Mag=%.2f (Threshold=%.1f)\n", 
                      ax, ay, az, accelMagnitude, SHAKE_THRESHOLD);
        lastPrintTime = millis();
    }
    
    //閾値を超えない場合は振りなし
    if (accelMagnitude < SHAKE_THRESHOLD) {
        return SHAKE_NONE;
    }
    
    //閾値を超えた！
    Serial.printf("*** SHAKE DETECTED! Mag=%.2f ***\n", accelMagnitude);
    
    //どの軸が最も大きく変化したかで方向を判定
    float absX = abs(ax);
    float absY = abs(ay);
    float absZ = abs(az);
    
    ShakeDirection direction = SHAKE_NONE;
    
    //Z軸(縦方向)が最大の場合
    if (absZ > absX && absZ > absY) {
        if (az > 0) {
            direction = SHAKE_UP;//縦上(全消灯)
            Serial.println("Direction: UP");
        } else {
            direction = SHAKE_DOWN;//縦下(魔法発動)
            Serial.println("Direction: DOWN");
        }
    }
    //X軸(横方向)が最大の場合
    else if (absX > absY && absX > absZ) {
        if (ax > 0) {
            direction = SHAKE_RIGHT; // 横右
            Serial.println("Direction: RIGHT");
        } else {
            direction = SHAKE_LEFT;  // 横左
            Serial.println("Direction: LEFT");
        }
    }
    //Y軸方向の振り(予備)
    else if (absY > absX && absY > absZ) {
        //Y軸も念のため対応
        Serial.println("Direction: Y-axis (ignored)");
    }
    
    return direction;
}

bool executeMagicRoutin(int id) {
    if (!isConnected) return false;
    
    HTTPClient http;
    String url = String(API_BASE_URL) + "/Lumina/magicroutin/execution/" + String(id);
    
    http.begin(url);
    http.addHeader("X-API-Key", API_KEY);
    http.addHeader("Content-Type", "application/json");
    http.setTimeout(HTTP_TIMEOUT_MS);
    
    int httpCode = http.POST("");
    
    bool success = (httpCode == 200);
    
    if (!success) {
        M5.Lcd.fillScreen(BLACK);
        M5.Lcd.setCursor(10, 10);
        M5.Lcd.setTextColor(RED);
        M5.Lcd.println("Failed");
        M5.Lcd.setTextColor(WHITE);
        M5.Lcd.setCursor(10, 35);
        M5.Lcd.printf("HTTP: %d", httpCode);
        delay(1500);
    }
    
    http.end();
    return success;
}

bool turnOffAllLights() {
    if (!isConnected) return false;
    
    HTTPClient http;
    String url = String(API_BASE_URL) + "/Lumina/all";
    
    // JSON作成(MqttModels.csのLedColorに合わせる)
    DynamicJsonDocument doc(256);
    doc["R"] = 0;
    doc["G"] = 0;
    doc["B"] = 0;
    doc["Brightness"] = 0;
    doc["Mode"] = "normal";
    
    String jsonString;
    serializeJson(doc, jsonString);
    
    http.begin(url);
    http.addHeader("X-API-Key", API_KEY);
    http.addHeader("Content-Type", "application/json");
    http.setTimeout(HTTP_TIMEOUT_MS);
    
    int httpCode = http.POST(jsonString);
    
    bool success = (httpCode == 200);
    
    if (!success) {
        M5.Lcd.fillScreen(BLACK);
        M5.Lcd.setCursor(10, 10);
        M5.Lcd.setTextColor(RED);
        M5.Lcd.println("Failed");
        M5.Lcd.setTextColor(WHITE);
        M5.Lcd.setCursor(10, 35);
        M5.Lcd.printf("HTTP: %d", httpCode);
        delay(1500);
    }
    
    http.end();
    return success;
}

void showError(const String& message) {
    M5.Lcd.fillScreen(BLACK);
    M5.Lcd.setTextSize(2);
    M5.Lcd.setCursor(10, 10);
    M5.Lcd.setTextColor(RED);
    M5.Lcd.println("Error:");
    M5.Lcd.setCursor(10, 35);
    M5.Lcd.setTextColor(WHITE);
    M5.Lcd.println(message);
    delay(2000);
}