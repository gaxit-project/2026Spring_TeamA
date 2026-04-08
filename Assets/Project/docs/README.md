# 1. ドキュメントの目的

このディレクトリ（docs/）には、バイオ・エスケープの技術仕様と設計思想がまとめられています。

# 2. 技術スタック

- Engine: Unity 6000.0.69f1
- Language: C#
- Architecture: MVP(Model-View-Presenter)
- Libraries: InputSystem, DOTween, UniTask, Cinemachine

# 3. フォルダ構成

```
/
├─Assets/
│ ├── アセットストアなどからダウンロードしたファイル群
│ └── Project/
│     ├── Scenes/
│     │   ├─ 本番用シーン
│     │   └── Tests/
│     ├── Scripts/ # C#スクリプト
│     │    └─ 詳細はScripts/README.mdを参照
│     ├── Prefabs/ # プレハブ
│     │   ├── Player/ # プレイヤー本体、武器モデル
│     │   ├── Enemies/ # 各種敵キャラクター
│     │   └── Stage/ # 設置物、ギミック、壁・床
│     ├── Models/ # FBX、メッシュデータ
│     │   ├── Player/
│     │   ├── Enemies/
│     │   └── Environment/
│     ├── SO_Data/ # ScriptableObject (.assetファイル)
│     │   ├── Player/ # プレイヤーの初期パラメーター
│     │   ├── Enemy/ # ゾンビや変異体の個別の数値データ
│     │   └── Weapons/ # 武器ごとの攻撃力や弾数データ
│     ├── UI/ # UI用アセット（スクリプト以外）
│     │   ├── Fonts/ # フォントファイル
│     │   └── Sprites/ # アイコン、UI用画像
│     ├── VFX/ # エフェクト、パーティクル
│     ├── Shaders/ # Shader Graph、カスタムシェーダー
│     ├── Animations/ # アニメーターコントローラー、クリップ
│     ├── Materials/ # マテリアル、テクスチャ
│     ├── Audio/ # BGM、SE、ボイス
│     ├── Tests/ # ユニットテスト用
│     │   ├── Editor/
│     │   └── Runtime/
│     └── Settings/ # Input Action, URP, Cinemachine等の設定ファイル
├─Packages/
└─ ...
```
