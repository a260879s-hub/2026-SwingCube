# スイングキューブ!

フックを掛け、引き寄せ、勢いのまま次の足場へ。『スイングキューブ!』は、フックアクションでステージを駆け抜ける2Dアクションゲームです。

麗陵祭で実際に遊んでいただいた作品です。

## ゲームプレイ

画像をクリックするとGoogle Driveで紹介映像を開きます。動画を再生できない場合は、Google Driveの共有設定が「リンクを知っている全員」になっていることと、動画の処理が完了していることを確認してください。

[![フックで足場を渡るゲームプレイ](./materials/スクリーンショット%202026-10-30%20010800.png)](https://drive.google.com/file/d/1WbUqxREB2l0OoGx7V2FHMNQZekTpWkmX/view?usp=drive_link)

## スクリーンショット

| ステージ | フックアクション | タイトルイメージ |
| --- | --- | --- |
| ![ステージ](./materials/スクリーンショット%202026-10-30%20010332.png) | ![フックアクション](./materials/スクリーンショット%202026-10-30%20010800.png) | ![タイトルイメージ](./materials/スクリーンショット%202026-10-29%20235716.png) |

## 操作方法

| 操作 | 入力 |
| --- | --- |
| 左右移動 | A / D または左右矢印キー |
| ジャンプ | W / 上矢印キー / Space |
| フックを掛けてスイング | 左クリック（地形をクリック） |
| フックで引き寄せ | 右クリック（地形をクリック） |
| フックを離す | 各マウスボタンを離す |

## こだわったポイント

### 引き寄せの速さ

右クリックの引き寄せ中は、フック地点へ向かう方向に一定速度で移動します。初期値は `20`。入力を受けてからフックが伸びるまでの待ちを作らず、狙った場所へ一気に詰める手触りを目指しました。

- 引き寄せ速度の設定: [`PlayerController.cs` `pullSpeed`](./Assets/PlayerController.cs#L30)
- 引き寄せ中の速度適用: [`PlayerController.cs` `FixedUpdate`](./Assets/PlayerController.cs#L106)

### 振り子の勢いと動きのメリハリ

左クリックでは `DistanceJoint2D` でフック地点とプレイヤーをつなぎ、振り子のように移動できます。スイング中は空気抵抗にあたる `linearDamping` をゼロにして、加速した勢いが残るようにしています。一方、速度が十分に乗ったら横移動入力による追加加速を止め、慣性で飛び出す気持ちよさを際立たせています。

- フック速度の上限設定: [`PlayerController.cs` `minSpeedToLoop`](./Assets/PlayerController.cs#L31)
- 速度に応じた入力制御: [`PlayerController.cs`](./Assets/PlayerController.cs#L67)
- スイングの支点設定と減衰調整: [`PlayerController.cs`](./Assets/PlayerController.cs#L166)、[`PlayerController.cs`](./Assets/PlayerController.cs#L172)

### 落下は素早く、着地後は切り替える

下降中だけ重力を追加し、落下をきびきびさせています。ジャンプの上昇と落下の速度差が生まれ、フック移動と合わせて動きにメリハリが出るようにしています。

- 落下時の重力倍率: [`PlayerController.cs` `fallMultiplier`](./Assets/PlayerController.cs#L21)
- 下降中の追加重力: [`PlayerController.cs`](./Assets/PlayerController.cs#L115)

### 狙った地形にフックを掛ける

マウス位置へ向けて地形にレイを飛ばし、届く範囲内に命中したときだけフックを有効にします。フックの軌跡も表示し、移動先と支点を見失いにくくしています。

- フックの最大距離: [`PlayerController.cs` `maxHookDistance`](./Assets/PlayerController.cs#L29)
- 地形への命中判定: [`PlayerController.cs`](./Assets/PlayerController.cs#L146)
- ワイヤーの描画: [`PlayerController.cs`](./Assets/PlayerController.cs#L195)

## 起動方法

1. Unity `6000.4.5f1` でこのリポジトリを開きます。
2. `Assets/Scenes/SampleScene.unity` を開いて再生します。

## 使用技術

- Unity 6 (`6000.4.5f1`)
- C#
- Unity 2D Physics / Tilemap

## 動画について

紹介映像はGoogle Driveで公開し、このリポジトリには含めていません。大容量動画のダウンロードを避け、GitHubから手軽に閲覧できるようにしています。
