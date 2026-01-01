#include <Arduino.h>              // Arduino の基本ライブラリ
#include <Adafruit_NeoPixel.h>    // NeoPixel(フルカラーLED) 制御用ライブラリ

const int PIN_NUM = 21;           // LEDストリップを接続したピン番号
const int NUM_LEDS = 16;          // LED の数

// 基本色（オレンジっぽい炎色）
// 明るさによってこの色をスケールして出力する
const int baseR = 255;
const int baseG = 60;
const int baseB = 0;

// NeoPixel オブジェクトの生成
Adafruit_NeoPixel strip(NUM_LEDS, PIN_NUM, NEO_GRB + NEO_KHZ800);

// 🍂 炎の揺らぎ用パラメータ
const int GROUP_COUNT = 4;             // LED を 4 グループにまとめる
const int GROUP_SIZE = NUM_LEDS / GROUP_COUNT;  // 各グループのLED数（今回は4）

int groupCurrent[GROUP_COUNT];         // 各グループの現在の明るさ
int groupTarget[GROUP_COUNT];          // 各グループの目標の明るさ

int individualOffset[NUM_LEDS];        // LED 個別のゆらぎ（±20ぐらいの乱数）

// 🔥 キャンドル用ランダム
// 0〜50（暗い）か 200〜255（明るい）だけを返す
int getCandleRandom() {
  if (random(0, 2) == 0) {     // 50%で暗い
    return random(0, 50);
  } else {                     // 50%で明るい
    return random(200, 255);
  }
}

void setup() {
  Serial.begin(115200);        // デバッグ用シリアル開始
  strip.begin();               // LEDストリップ初期化
  strip.show();                // すべての LED を消灯

  randomSeed(analogRead(0));   // ランダムの初期化（アナログノイズを利用）

  // グループの初期明るさ設定
  for (int g = 0; g < GROUP_COUNT; g++) {
    groupCurrent[g] = getCandleRandom();   // 現在値
    groupTarget[g]  = getCandleRandom();   // 次の目標値
  }

  // LED 個別の微妙なゆらぎの初期値
  for (int i = 0; i < NUM_LEDS; i++) {
    individualOffset[i] = random(-10, 10); // 最初は軽め
  }
}

void loop() {

  // 🔥 各グループの明るさゆらぎ（大きな炎の塊の動き）
  for (int g = 0; g < GROUP_COUNT; g++) {

    // 目標に向かって 1 ずつ変化（フェード効果）
    if (groupCurrent[g] < groupTarget[g]) {
      groupCurrent[g]++;
    } else if (groupCurrent[g] > groupTarget[g]) {
      groupCurrent[g]--;
    }

    // もし目標に達したら、新しい目標をランダムで作る
    if (groupCurrent[g] == groupTarget[g]) {
      groupTarget[g] = getCandleRandom();
    }
  }

  // 🔥 LED 個別の細かい揺れ
  for (int i = 0; i < NUM_LEDS; i++) {

    // ランダムに -2〜+2 変化させる
    individualOffset[i] += random(-2, 3);

    // ゆらぎの振れ幅を -20 ～ +20 に制限
    individualOffset[i] = constrain(individualOffset[i], -20, 20);
  }

  // 🔥 隣接スムージング（炎っぽい流れの動き）
  int smoothedBrightness[NUM_LEDS];

  for (int i = 0; i < NUM_LEDS; i++) {
    int g = i / GROUP_SIZE;                         // LED が属するグループ番号
    int baseBrightness = groupCurrent[g] + individualOffset[i]; // 中心の明るさ

    // 左LEDと右LEDも少し考慮
    int left  = (i > 0) ? smoothedBrightness[i-1] : baseBrightness;
    int right = baseBrightness; // 右は後で上書きされるので仮で同じ値

    // 0.7 自分 + 0.15 左 + 0.15 右 の加重平均
    smoothedBrightness[i] =
      baseBrightness * 0.7 +
      left * 0.15 +
      right * 0.15;

    // 明るさは0〜255に収める
    smoothedBrightness[i] = constrain(smoothedBrightness[i], 0, 255);
  }

  // 🔥 各 LED の色をセットして書き込み
  for (int i = 0; i < NUM_LEDS; i++) {

    float scale = smoothedBrightness[i] / 255.0; // 0〜1 に正規化

    int r = baseR * scale;  // 炎色をスケール
    int g = baseG * scale;
    int b = baseB * scale;

    strip.setPixelColor(i, strip.Color(r, g, b)); // LED へ送信データセット
  }

  strip.show();   // LED に反映
  delay(5);       // 動作速度（ゆらぎの速さを決める）
}
