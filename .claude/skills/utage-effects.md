# UTAGE 演出・エフェクトスキル

## 概要

宴(UTAGE)シナリオにおける視覚演出・アニメーション・画面効果を支援するスキル。Tween、Shake、フェード、カメラ制御、イメージエフェクトをカバーする。

## いつ使うか

以下の話題が出たとき:
- キャラや背景の移動・回転・拡大縮小アニメーション
- 画面・オブジェクトの揺らし演出
- 画面フェードイン・フェードアウト
- ルール画像を使ったトランジション
- カメラズーム
- グレースケール・セピア・ブラー等のイメージエフェクト
- ポストエフェクト (URP)
- 待機処理・演出スレッド
- WaitType の設定

## コマンドリファレンス

### Tween（アニメーション）

| 項目 | 内容 |
|------|------|
| Command | Tween |
| Arg1 | 対象名（キャラ名、スプライト名、`BG`、レイヤー名） |
| Arg2 | TweenType |
| Arg3 | パラメーター（`time=秒 x=値 y=値` 形式） |
| Arg4 | EaseType |
| Arg5 | LoopType |
| Arg6 | WaitType |

#### TweenType一覧

**移動:**
| TweenType | 説明 |
|-----------|------|
| MoveTo | 指定座標へ移動 |
| MoveFrom | 指定座標から現在位置へ |
| MoveBy | 相対移動 |
| MoveAdd | 他Tweenと同時実行可能 |
| PunchPosition | パンチ効果→元に戻る |
| ShakePosition | 揺れ効果→元に戻る |

**回転:**
| TweenType | 説明 |
|-----------|------|
| RotateTo | 指定角度へ回転 |
| RotateFrom | 指定角度から現在角度へ |
| RotateBy | 相対回転 |
| RotateAdd | 同時実行可能 |
| PunchRotation | パンチ回転→戻る |
| ShakeRotation | ブレ回転→戻る |

**拡大縮小:**
| TweenType | 説明 |
|-----------|------|
| ScaleTo | 指定スケールへ |
| ScaleFrom | 指定スケールから現在値へ |
| ScaleBy | 相対変化 |
| ScaleAdd | 同時実行可能 |
| PunchScale | パンチ→戻る |
| ShakeScale | ブレ→戻る |

**色:**
| TweenType | 説明 |
|-----------|------|
| ColorTo | 指定色へ変化 |
| ColorFrom | 指定色から現在色へ |

#### Tweenパラメーター (Arg3)

`key=value` 形式。スペース区切りで複数指定。

| パラメーター | 説明 | デフォルト |
|------------|------|----------|
| time | 継続秒数 | 1秒 |
| speed | time代わりのスピード | - |
| delay | 開始遅延秒数 | 0 |
| x, y, z | アニメーション値 | - |
| islocal | ローカル座標系 | false |
| color | カラー値 | - |
| alpha | アルファ値(0〜1) | - |
| r, g, b, a | RGB+アルファ(0〜1) | - |

#### EaseType (Arg4)

主要なもの: `linear`, `easeInOutQuad`, `easeOutBounce`, `easeInOutBack`, `easeOutElastic`, `spring` 他30種以上

#### LoopType (Arg5)

| 記述 | 説明 |
|------|------|
| loop=3 | 3回ループ |
| loop=0 | 無限ループ |
| pingPong=6 | 6回往復 |
| pingPong=0 | 無限往復 |

**Tween例:**
| Command | Arg1 | Arg2 | Arg3 | Arg4 |
|---------|------|------|------|------|
| Tween | Haruka | MoveTo | time=0.5 x=200 | easeOutQuad |
| Tween | BG | ScaleTo | time=0.8 x=1.5 y=1.5 | easeInOutQuad |
| Tween | Haruka | ColorTo | time=1 alpha=0 | linear |
| Tween | Yuki | PunchPosition | time=0.3 x=20 y=0 | |
| Tween | BG | RotateBy | time=2 z=360 | linear | 

### Shake（揺らし）

Tweenの簡略版。カメラやMessageWindowも対象にできる。

| Command | Arg1 | Arg2 | WaitType |
|---------|------|------|----------|
| Shake | 対象名 | パラメーター | WaitType |

**対象名:**
| 指定 | 説明 |
|------|------|
| キャラ名/スプライト名 | 個別オブジェクト |
| MessageWindow | メッセージウィンドウ |
| Graphics | 全グラフィック |
| Camera | 全カメラ（UIも） |

**例:**
| Command | Arg1 | Arg2 |
|---------|------|------|
| Shake | Camera | time=0.5 x=50 y=50 |
| Shake | Haruka | time=0.3 x=20 |
| Shake | MessageWindow | |

デフォルト: x=30, y=30

### ZoomCamera（カメラズーム）

| Command | Arg1 | Arg2 | Arg3 | Arg4 | Arg5 | WaitType |
|---------|------|------|------|------|------|----------|
| ZoomCamera | カメラ名 | ズーム値 | 中心X | 中心Y | 秒数(0.2) | WaitType |

**重要:**
- ズーム値は最終的に **必ず1に戻す**こと
- ズーム1.0になると中心点は自動的に(0,0)にリセット

**例: ズームイン→戻す**
| Command | Arg2 | Arg3 | Arg4 | Arg5 |
|---------|------|------|------|------|
| ZoomCamera | 2 | 0 | 100 | 0.5 |
| *(演出)* | | | | |
| ZoomCamera | 1 | | | 0.5 |

### FadeOut / FadeIn（画面フェード）

**FadeOut:**
| Arg1 | Arg2 | Arg3 | Arg4 | Arg5 | Arg6 |
|------|------|------|------|------|------|
| カラー(white) | カメラ名(SpriteCamera) | ルール画像 | 境界フェード値(0.2) | フェード秒数(0.2) | WaitType |

**FadeIn:**
同じ引数構成。**FadeInはFadeOutの後でないと使えない**。

シナリオ冒頭で暗転開始: `FadeOut`を秒数0で実行→`FadeIn`

**カラー指定:** 色名(`black`, `white`, `red`等)または16進(`#ff0000ff`)

**UIカメラへのフェード:** Arg2に`UICamera`

**例: 暗転→明転**
| Command | Arg1 | Arg5 |
|---------|------|------|
| FadeOut | black | 0.5 |
| Wait | 1 | |
| FadeIn | black | 0.5 |

### RuleFadeIn / RuleFadeOut（ルール画像フェード）

個別オブジェクトにパターンフェードをかける。

| Command | Arg1 | Arg2 | Arg3 | Arg4 | WaitType |
|---------|------|------|------|------|----------|
| RuleFadeIn | 対象名 | ルール画像名 | 境界サイズ(0.2) | 秒数(0.2) | WaitType |
| RuleFadeOut | 対象名 | ルール画像名 | 境界サイズ(0.2) | 秒数(0.2) | WaitType |

### ImageEffect（イメージエフェクト）

| Command | Arg1 | Arg2 | Arg3 | Arg4 | WaitType |
|---------|------|------|------|------|----------|
| ImageEffect | カメラ名 | エフェクト名 | アニメーション名 | フェード秒(0) | WaitType |
| ImageEffectOff | カメラ名 | エフェクト名/"All" | アニメーション名 | フェード秒 | WaitType |

**エフェクト一覧:**
| エフェクト名 | 効果 |
|-------------|------|
| GrayScale | グレースケール |
| Sepia | セピア調 |
| NegaPosi | ネガポジ反転 |
| Blur | ブラー |
| MotionBlur | モーションブラー |
| Bloom | ブルーム/グロー |
| Mosaic | モザイク |
| FishEye | 魚眼レンズ |
| Twirl | 渦巻き |
| Vortex | ボルテックス |

**カメラの違い:**
- `SpriteCamera`: 背景/キャラのみ（UIに影響なし）
- `UICamera`: 全画面（セーブ/ロード画面にも持続）

**例: セピア調回想シーン**
| Command | Arg1 | Arg2 | Arg4 |
|---------|------|------|------|
| ImageEffect | SpriteCamera | Sepia | 0.5 |
| *(回想シーン)* | | | |
| ImageEffectOff | SpriteCamera | Sepia | 0.5 |

### PostEffect（URP専用ポストエフェクト）

| Command | Arg1 | Arg2 | Arg3 | Arg4 | Arg5 |
|---------|------|------|------|------|------|
| PostEffect | カメラ名 | ボリューム名 | エフェクト名(カンマ区切り) | フェード秒(0) | WaitType |
| PostEffectOff | カメラ名 | ボリューム名 | フェード秒(0) | WaitType | |

Unity公式+アセットストアのポストエフェクトも使用可能。

### 待機コマンド

| Command | Arg1 | Arg2 | 説明 |
|---------|------|------|------|
| Wait | 秒数 | | 指定時間待機 |
| WaitInput | 秒数 | | 入力 or 時間経過で解除 |
| WaitCustom | | | プログラムから解除 |
| WaitConditional | 条件式 | 最低秒数 | 条件TRUE中待機 |
| WaitEffectTime | 秒数 | WaitType | スキップ可能な待機 |
| WaitFadeObjects | 対象名 | WaitType | フェード終了待ち |
| WaitSound | サウンド種類 | 対象名 | サウンド終了待ち |

### 演出用スレッド (Thread)

テキスト表示と非同期にエフェクトを実行。

| Command | Arg1 |
|---------|------|
| Thread | スレッドラベル |
| WaitThread | スレッドラベル |

スレッド内はエフェクト系コマンドのみ使用可。テキスト表示不可。`EndThread`で終了。

### WaitType リファレンス

| WaitType | 説明 |
|----------|------|
| (空欄) | エフェクト終了まで待機 |
| NoWait | 待たずに次へ |
| PageWait | 改ページ時に終了待ち |
| Skippable | クリックでスキップ可能 |
| SkipOnInput | クリックで強制終了 |
| SkipOnBrPage | 改ページで強制終了 |
| Add | 次エフェクトと同期 |

**Skippable系**: 2クリック必要（スキップ→進行）。Auto時はエフェクト完了待ち。
**SkipOn系**: 1クリックで進行+スキップ同時。Auto時もスキップ。無限ループにはSkipOn系推奨。

## 演出パターン集

### キャラ登場（画面外から滑り込み）
| Command | Arg1 | Arg2 | Arg3 | Arg4 | Arg6 |
|---------|------|------|------|------|------|
| | Haruka | smile | Left | | |
| Tween | Haruka | MoveFrom | time=0.4 x=-400 | easeOutQuad | |

### 衝撃演出（画面揺れ + SE）
| Command | Arg1 | Arg2 |
|---------|------|------|
| Se | se_impact | |
| Shake | Camera | time=0.5 x=40 y=40 |

### 回想シーン（セピア + ズーム）
| Command | Arg1 | Arg2 | Arg3 | Arg4 | Arg5 |
|---------|------|------|------|------|------|
| ImageEffect | SpriteCamera | Sepia | | 0.5 | |
| ZoomCamera | | 1.3 | 0 | 0 | 1.0 |

### キャラ退場（フェードアウト）
| Command | Arg1 | Arg2 | Arg3 |
|---------|------|------|------|
| Tween | Haruka | ColorTo | time=0.5 alpha=0 |

### 場面転換（暗転）
| Command | Arg1 | Arg5 |
|---------|------|------|
| FadeOut | black | 0.5 |
| Bg | bg_room2 | |
| FadeIn | black | 0.5 |

### ドキドキ演出（心拍風スケール）
| Command | Arg1 | Arg2 | Arg3 | Arg5 |
|---------|------|------|------|------|
| Tween | Haruka | ScaleTo | time=0.2 x=1.05 y=1.05 | pingPong=4 |

## 回答時の注意

- WaitTypeの選択は演出体験に大きく影響するので適切なものを提案すること
- ZoomCameraは必ず1に戻す注意を添えること
- 複合演出ではNoWait/Addを使った並列実行を提案すること
- 不明点は `search_utage_docs` で追加検索すること

## 参考URL

- [Tweenアニメーション](https://madnesslabo.net/utage/?page_id=1791)
- [Shake](https://madnesslabo.net/utage/?page_id=1789)
- [ZoomCamera](https://madnesslabo.net/utage/?page_id=8587)
- [フェード処理](https://madnesslabo.net/utage/?page_id=1787)
- [ルール画像フェード](https://madnesslabo.net/utage/?page_id=8576)
- [イメージエフェクト](https://madnesslabo.net/utage/?page_id=8594)
- [ポストエフェクト](https://madnesslabo.net/utage/?page_id=15969)
- [WaitType](https://madnesslabo.net/utage/?page_id=9081)
- [待機処理](https://madnesslabo.net/utage/?page_id=1785)
- [演出用スレッド](https://madnesslabo.net/utage/?page_id=8597)
- [キーフレームアニメーション](https://madnesslabo.net/utage/?page_id=8608)
