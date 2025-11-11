<p align="center"><img src="rumos_icon.png" alt="ロゴ" width="full" /></p>
# Dawn-rumos

> **部屋中のライトを自在に操作・管理できるスマート照明システム！**
> ESP32 と M5GO、TP-Link スマートデバイスを組み合わせて、電源の ON/OFF や光の色・輝度を一括制御。
> プリセット機能でシーンに合わせた雰囲気づくりも簡単に実現できます。

---

**_ 主な技術スタック _**

<p align="center">
      <img src="https://www.jetbrains.com/guide/assets/csharp-logo-265a149e.svg" alt=".NET" width="60" height="60"/>
        <img src="https://upload.wikimedia.org/wikipedia/commons/1/18/ISO_C%2B%2B_Logo.svg" alt=".NET" width="60" height="60"/>
          <img src="https://upload.wikimedia.org/wikipedia/commons/9/99/Unofficial_JavaScript_logo_2.svg" alt=".NET" width="60" height="60"/>
                  <img src="https://images.icon-icons.com/2107/PNG/512/file_type_html_icon_130541.png" alt=".NET" width="60" height="60"/>
          <img src="https://images.icon-icons.com/2107/PNG/512/file_type_css_icon_130661.png" alt=".NET" width="60" height="60"/>

  <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/7/7d/Microsoft_.NET_logo.svg/1200px-Microsoft_.NET_logo.svg.png" alt=".NET" width="60" height="60"/>
  <img src="https://learn.microsoft.com/ja-jp/windows/apps/images/logo-winui.png" alt="Node.js" width="60" height="60"/>
  <img src="https://www.w2solution.co.jp/wp-content/uploads/2023/01/asp.net_.logo_-e1674006912485.png" alt=".NET" width="60" height="60"/>
  <img src="https://images.icon-icons.com/2699/PNG/512/nodejs_logo_icon_169910.png" alt=".NET" width="60" height="60"/>

  <img src="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRf7u6IFvFishXrGbwZiPBZfPxJMrQH3eUKhuE6Vb5wEMrhoazewOBy9iLvxpLGu97wgnU&usqp=CAU" alt="Blazor" width="60" height="60"/>
  <img src="https://devblogs.microsoft.com/dotnet/wp-content/uploads/sites/16/2019/04/BrandBlazor_nohalo_1000x.png" alt=".NET" width="60" height="60"/>
  <img src="https://cdn.sanity.io/images/3do82whm/next/1c2aa4d10fe71c23d7a36f69fc11c419c5f4ac35-1000x667.png?w=720&h=480&fit=clip&auto=format" alt=".NET" width="60" height="60"/>
</p>

---

### **追加/改善すべき機能(メモ)**

・ソフト側のリクエストを Https 化

---

### **次やること(メモ)**

・プリセット用クライアント UI 追加

## 悩みメモ:

・今は特になし

---

## 🚀 プロジェクト概要

**Dawn-rumos**は、部屋中のさまざまなライトデバイスを操作・管理できるシステムです。
電源の ON/OFF や光の色、輝度を柔軟に制御でき、プリセット機能を活用した一括制御で、シーンに合わせて部屋の雰囲気を自在に演出できます。

---

## 🎯 機能

| 機能                   | 説明                                                 |
| ---------------------- | ---------------------------------------------------- |
| **状態管理**           | 各受信端末の情報を DB で管理                         |
| **Windows アプリ制御** | 専用ソフトから、受信端末や電源の状態を確認・操作可能 |
| **一括制御**           | 複数デバイスの一括 ON/OFF                            |
| **状態取得**           | 現在の ON/OFF 状態をリアルタイムで取得               |

---

## 🛠 技術スタック

### **組込み**

- **ESP32** — ライトデバイス
- **TP-Link スマートデバイス**

### **フロントエンド**

- **WinUI (C#)** — Windows 向け専用制御アプリケーション
- **.NET MAUI Blazor Hybrid (C#,HTML,CSS,JS)** — クロスプラットフォーム 向け専用制御アプリケーション

### **バックエンド**

- **ASP.NET Core API (C#)** — メイン API
- **Node.js マイクロサービス** — TP-Link 制御専用

### **データベース**

- **Mysql**

---

## 🖥 用意するデバイス

- **ライトデバイス**: ESP32

---

## 📡 システム構成

[フロントエンド: WinUI3 or .NET MAUI Blazor Hybrid]
↓ HTTP/SignalR
[ASP.NET Core API] ← 状態管理 / データベース更新
↓ HTTP/gRPC
[Node.js マイクロサービス] ← TP-Link 制御
↓
[TP-Link スマートデバイス]

---

## 🔄 処理フロー

### **アプリ操作フロー**

1. Windows アプリから電源状態を管理・操作
2. 複数端末の一括制御や状態取得も可能

---
