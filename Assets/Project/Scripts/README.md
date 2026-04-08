# フォルダ構成

```
Project/
  └── Scripts/
      ├── Common/ # インターフェース(IDamageable等)、汎用ツール
      ├── Player/ # 移動、攻撃、ステータス、アニメーション制御
      ├── Enemy/ # 敵Baseクラス、個別AI(Zombie, Mutant)
      ├── Stage/ # ギミック(ドラム缶等)、レベル遷移(ダクト/階段)
      ├── Camera/ # Cinemachineの制御、カメラ演出
      ├── UI/ # UI制御ロジック
      ├── Data/ # ScriptableObjectのクラス定義ファイル
      └── Editor/ # エディタ拡張用
```
