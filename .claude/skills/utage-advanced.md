# UTAGE 上級テクニックスキル

## 概要

宴(UTAGE)シナリオのマクロ、サブルーチン、エンティティ、UI制御、Unity連携など上級パターンを支援するスキル。

## いつ使うか

以下の話題が出たとき:
- マクロの定義・引数・呼び出し
- 構造化マクロ
- エンティティ（動的パラメーター展開）
- GUI操作（表示/非表示/移動）
- メッセージウィンドウの制御・切替
- SendMessage / Unity連携
- スプライト表示・パーティクル
- レイヤー制御 (LayerOff, LayerReset)
- キーフレームアニメーション (PlayAnimation)
- ビデオ再生
- 会話シーンとしての組み込み
- サブシナリオの再生
- カメラ・レイヤー手動設定
- 宴の仕組みを活用した複雑な演出パターン

## マクロ

### 基本

マクロは「Macro」シートに記述する。複数コマンドをまとめた新コマンドを作るイメージ。

**定義:**
| Command | Arg1 | Arg2 | ... |
|---------|------|------|-----|
| *マクロ名 | | | |
| *(コマンド群)* | | | |
| EndMacro | | | |

**呼び出し:** Command列にマクロ名を書くだけ。

### 引数付きマクロ

マクロ内で `%列名` を使うと、呼び出し時の値で置換される。

**定義例（背景フェード切替マクロ）:**
| Command | Arg1 | Arg2 | Arg3 | Arg4 | Arg5 |
|---------|------|------|------|------|------|
| *FadeBg | | black | 0.2 | 1 | 0.2 |
| FadeOut | %Arg2 | | | %Arg3 | |
| Wait | %Arg4 | | | | |
| Bg | %Arg1 | | | | |
| FadeIn | %Arg2 | | | %Arg5 | |
| EndMacro | | | | | |

2行目のデフォルト引数: Arg2=black, Arg3=0.2, Arg4=1, Arg5=0.2

**呼び出し:**
| Command | Arg1 |
|---------|------|
| FadeBg | bg_sunset |

### デフォルト引数

マクロ名の行の各Arg列に初期値を書く。呼び出し時に空欄なら初期値が使われる。

### エンティティ

マクロ内で `&パラメーター名` を使うと、実行時のパラメーター値で動的に置換。

**注意点:**
- インポート時のエラーチェックが効かない
- PageCtrlなど実行前に情報が必要な箇所では不可
- Textへの使用は不安定（`<param=名前>`タグを推奨）
- 処理が重くなる可能性あり
- 初期値を正しく設定すること

### 構造化マクロ (v4.2.6+)

`%引数列名.プロパティ名` で、1セル内に複数要素を記述可能。

**セットアップ:**
1. Create > Utage > Scenario > StructuredMacroParser
2. CustomProjectSettings の Structured Macro Parser に設定

**記述:** `key=value,key2=value2` 形式。

### マクロの注意

- マクロ内にシナリオラベルを書かない（重複エラー）
- サブルーチンとの違い: マクロは「埋め込み」、サブルーチンは「呼び出し」
- 既読判定はマクロ使用箇所ごとに個別

## UI制御コマンド

### メッセージウィンドウ

| Command | Arg1 | 説明 |
|---------|------|------|
| ShowMessageWindow | | ウィンドウ表示 |
| HideMessageWindow | | ウィンドウ非表示 |
| ChangeMessageWindow | ウィンドウ名 | ウィンドウ切替 |

### GUI操作

| Command | Arg1 | Arg2 | Arg3 | 説明 |
|---------|------|------|------|------|
| GuiActive | GUI名 | true/false | | GUI表示/非表示 |
| GuiPosition | GUI名 | X | Y | GUI位置移動 |

### メニューボタン

| Command | 説明 |
|---------|------|
| ShowMenuButton | メニューボタン表示 |
| HideMenuButton | メニューボタン非表示 |

## スプライト・パーティクル

### Sprite

Textureシートで Type=Sprite として登録したオブジェクト。

| Command | Arg1 | Arg2 | Arg3 | Arg4 | Arg5 | Arg6 |
|---------|------|------|------|------|------|------|
| Sprite | スプライトラベル | レイヤー名 | X | Y | フェード秒(0.2) | |
| SpriteOff | レイヤー名 | フェード秒(0.2) | | | | |

### Particle

| Command | Arg1 | Arg2 | Arg3 |
|---------|------|------|------|
| Particle | パーティクルラベル | | |
| ParticleOff | パーティクルラベル | | |

### LayerOff / LayerReset

| Command | Arg1 | Arg2 |
|---------|------|------|
| LayerOff | レイヤー名 | フェード秒(0.2) |
| LayerReset | (全レイヤーリセット) | |

## キーフレームアニメーション

| Command | Arg1 | Arg2 |
|---------|------|------|
| PlayAnimation | 対象名 | アニメーションラベル |

Animationシートで定義。プロパティ: X,Y,Z, Scale, Angle, Alpha, R,G,B, Texture, Pattern 等。

## ビデオ再生

| Command | Arg1 |
|---------|------|
| Video | ビデオラベル |

CharacterまたはTextureシートで FileType=Video として登録。透過動画対応。

## Unity連携

### SendMessage

| Command | Arg1 | Arg2 | Arg3 |
|---------|------|------|------|
| SendMessage | オブジェクト名 | メソッド名 | 引数 |
| SendMessageByName | オブジェクト名 | メソッド名 | 引数 |
| BroadcastMessageByName | オブジェクト名 | メソッド名 | 引数 |

### 会話シーンとしての組み込み

プログラムから宴を制御:
```csharp
engine.JumpScenario("*ラベル");     // シナリオ再生
engine.IsEndOrPauseScenario          // 終了/一時停止判定
engine.ResumeScenario();             // 再開
```

シナリオ側: `*ラベル` → コマンド群 → `EndScenario` または `PauseScenario`

### パラメーターのプログラム操作

```csharp
// 取得
int point = engine.Param.GetParameterInt("love");
bool flag = engine.Param.GetParameterBoolean("flag_item");

// 設定
engine.Param.SetParameterInt("love", 100);
engine.Param.SetParameterBoolean("flag_item", true);
```

### WaitCustom + カスタムUI

```csharp
// 自作UIのボタンクリックでWaitCustomを解除
public void OnClick()
{
    engine.UiManager.IsInputTrigCustom = true;
}
```

### サブシナリオ再生

メインシナリオを中断してサブシナリオを再生→復帰。
```csharp
engine.JumpSubScenario("*SubLabel");
```

## 複合演出パターン

### 並列アニメーション（NoWait活用）

| Command | Arg1 | Arg2 | Arg3 | Arg6 |
|---------|------|------|------|------|
| Tween | Haruka | MoveTo | time=1 x=200 | NoWait |
| Tween | Yuki | MoveTo | time=1 x=-200 | NoWait |
| Tween | BG | ScaleTo | time=1 x=1.2 y=1.2 | |

最後のTweenだけWaitType空欄→そこまで全部並列実行して最後のが終わるまで待つ。

### Add でタイミング同期

| Command | Arg1 | Arg2 | Arg3 | Arg6 |
|---------|------|------|------|------|
| Tween | Haruka | MoveTo | time=0.5 x=100 | Add |
| Tween | Haruka | ScaleTo | time=0.5 x=1.2 y=1.2 | |

Add指定のTweenは次のTween終了タイミングに合わせて同時待機。

### Threadで非同期演出

| Command | Arg1 |
|---------|------|
| Thread | *BgLoop |
| | テキスト表示は通常通り続く |

**スレッド定義:**
| Command | Arg1 | Arg2 | Arg3 | Arg5 |
|---------|------|------|------|------|
| *BgLoop | | | | |
| Tween | BG | ScaleTo | time=3 x=1.05 y=1.05 | pingPong=0 |
| EndThread | | | | |

### 既読・選択済みの色変更

AdvPage コンポーネントで設定:
- **Read Color Mode** → "Change" + 色指定
- **Selected Color Mode** → "Change" + 色指定

## 回答時の注意

- マクロは強力だが複雑になりやすい。まず基本コマンドの組み合わせを提案し、頻出パターンならマクロ化を提案する流れが良い
- エンティティは不安定要素があるため、使用時は必ず注意点を添えること
- Unity連携が必要な場面では、C#コードとシナリオの両方を提示すること
- 不明点は `search_utage_docs` で追加検索すること

## 参考URL

- [マクロ](https://madnesslabo.net/utage/?page_id=2984)
- [サブルーチン](https://madnesslabo.net/utage/?page_id=2973)
- [似たような処理を効率的に記述する](https://madnesslabo.net/utage/?page_id=3017)
- [会話シーンに使う](https://madnesslabo.net/utage/?page_id=402)
- [UIを変更する](https://madnesslabo.net/utage/?page_id=4782)
- [キーフレームアニメーション](https://madnesslabo.net/utage/?page_id=8608)
- [演出用スレッド](https://madnesslabo.net/utage/?page_id=8597)
- [サブシナリオ再生](https://madnesslabo.net/utage/?page_id=11907)
- [シナリオ終了位置](https://madnesslabo.net/utage/?page_id=1816)
- [ビデオオブジェクト](https://madnesslabo.net/utage/?page_id=9589)
- [既読テキスト色変更](https://madnesslabo.net/utage/?page_id=6381)
- [コマンド一覧](https://madnesslabo.net/utage/?page_id=252)
