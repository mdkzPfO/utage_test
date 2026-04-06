# ScenarioManager プロジェクト

## プロジェクト概要

Unity 6 + URP + 宴(UTAGE) によるビジュアルノベルプロジェクト。
シナリオ原本は `シナリオ.xlsx`（独自フォーマット）、UTAGE用は `Assets/MyProject/Scenarios/MyProject.xlsx`。

## UTAGE シナリオ Excel の列レイアウト

| 列 | A | B | C | D | E | F | G | H | I | J | K | L |
|----|---|---|---|---|---|---|---|---|---|---|---|---|
| 名前 | Command | Arg1 | Arg2 | Arg3 | Arg4 | Arg5 | Arg6 | WaitType | Text | PageCtrl | Voice | WindowType |

## UTAGE コマンドの引数配置ルール（実証済み）

スキルの記述と実際の動作が異なる場合がある。以下は実機テストで確認済みのルール。

### テキスト表示
- Command空欄、Arg1=キャラ名、Text=セリフ

### FadeOut / FadeIn
- Arg1=カラー(`black`, `white`, `red`等)、**Arg5=秒数**、**WaitType(H列)=SkipOnInput**
- Arg6はWaitTypeではない（Animationとして解釈されエラーになる）
- FadeInはFadeOutの後でないと使えない

### Wait / WaitEffectTime
- **Arg6=秒数**（Arg1ではない。Arg1に入れると「Arg6が空」エラー）
- WaitEffectTimeは Arg2=WaitType（スキップ可能な待機が必要なとき）

### Shake
- Arg1=対象(`Camera`, `MessageWindow`, `Graphics`)、Arg2=パラメータ、**WaitType(H列)**

### Tween
- Arg1=対象(`BG`等)、Arg2=TweenType、Arg3=パラメータ、Arg4=EaseType、**WaitType(H列)**

### ZoomCamera
- **Arg1=ズーム値**（カメラ名省略時でもArg1必須。空だとエラー）
- Arg2=中心X、Arg3=中心Y、Arg4=秒数、**WaitType(H列)**
- スキルには「Arg1=カメラ名」とあるがカメラ名は不要、ズーム値を直接入れる

### ImageEffect / ImageEffectOff
- Arg1=`SpriteCamera`、Arg2=エフェクト名、Arg4=フェード秒、**WaitType(H列)**
- 利用可能: Blur, FishEye, GrayScale, Mosaic, NegaPosi, Sepia, Twirl, Vortex
- `ImageEffectOff`のArg2に`All`で全解除

### Jump（条件付き）
- Arg1=ラベル、Arg2=条件式（`flag_name==true` のように全体を1つの文字列で）
- **ラベルは全シートでグローバル**。Cross-sheet JumpでもArg2にシート名を入れてはいけない（条件式として解釈されエラー）

### If / ElseIf / Else / EndIf
- Arg1=条件式（全体を1文字列）
- **If内にテキスト表示(Text列)を入れてはいけない**。テキスト分岐にはJump条件を使う
- 推奨パターン:
  ```
  Jump | *true_label | condition==true
  Jump | *false_label |
  *true_label
    (テキスト)
    Jump | *merge
  *false_label
    (テキスト)
    Jump | *merge
  *merge
  ```

### Selection
- Arg1=ジャンプ先ラベル、Text=選択肢テキスト

### コメント行
- UTAGEは `;` をコメントとして認識しない（「不正な記述です」エラー）
- コメントを入れたい場合は行を完全に空にするか、Excelのコメント機能を使う

## 演出の WaitType 統一ルール

**全ての演出コマンドで `SkipOnInput` を WaitType列(H列) に入れること。**
これを怠るとSkip機能が演出で止まる。FadeOut/FadeIn/Wait/Shake/Tween/ZoomCamera/ImageEffect 全て同じ。

## Skip機能に関する注意

### デフォルト設定の問題
UTAGEのデフォルトでは:
- `IsSkipUnread = false`（未読テキストはスキップ不可）
- `IsStopSkipInSelection = true`（選択肢でスキップ解除）

これらの設定はセーブデータ(`config`ファイル)に保存され、C#のフィールド初期値より優先される。

### 対処法（本プロジェクトでの実装）
- `Assets/MyProject/Scripts/AdvConfigOverride.cs` で `AdvEngine.OnPostInit` コールバックを使い強制上書き
- `AdvConfig.cs` の `StopSkipInSelection()` メソッドを無効化
- `AdvConfigSaveData.cs` のデフォルト値を変更
- セーブデータが残っていると古い設定が復元されるため、設定変更後は `persistentDataPath` の `config` ファイル削除が必要

## シナリオ変換時の注意（シナリオ.xlsx → MyProject.xlsx）

- シナリオ.xlsxの「CharacterImage」列が話者を示す（ナレーション、研究者、日付、選択肢）
- 「action」列の `fade in/out` → FadeOut/FadeIn、`GoMain` → Jump
- 「Flag」列の条件 → Jump条件式またはParam操作
- 制作メモ（「選択肢合流」「食料分配画面追加」等）はテキストとして表示されないよう除去する
- openpyxlでxlsxを編集する場合、`insert_rows()` で行挿入後のオフセット管理に注意

## ファイル構成

| パス | 説明 |
|------|------|
| `シナリオ.xlsx` | シナリオ原本（独自フォーマット、Opening/Mainシート） |
| `Assets/MyProject/Scenarios/MyProject.xlsx` | UTAGE用シナリオ（Opening/Main/Start + 設定シート群） |
| `Assets/MyProject/Scripts/AdvConfigOverride.cs` | Skip設定の強制上書き |
| `Assets/MyProject/Resources/MyProject/Texture/BG/背景.png` | 背景画像（研究施設） |
| `Assets/Utage/Scripts/ADV/Save/AdvConfigSaveData.cs` | Skip設定のデフォルト値（変更済み） |
| `Assets/Utage/Scripts/ADV/Logic/AdvConfig.cs` | StopSkipInSelection無効化（変更済み） |

## UTAGE スキル体系

このプロジェクトには宴(UTAGE)ドキュメントのRAG検索MCPサーバーと、シナリオ作成を支援する4つの専門スキルが含まれている。

### スキル一覧

| スキル | ファイル | カバー範囲 |
|--------|---------|-----------|
| RAG検索 | `.claude/skills/utage-rag.md` | `search_utage_docs` ツールの使い方、検索ガイドライン |
| シナリオ基本 | `.claude/skills/utage-scenario.md` | テキスト表示、キャラクター、背景、サウンド、PageCtrl |
| 分岐・ロジック | `.claude/skills/utage-branching.md` | Selection, Jump, If/Else, Param, ランダム分岐, サブルーチン |
| 演出・エフェクト | `.claude/skills/utage-effects.md` | Tween, Shake, Fade, ZoomCamera, ImageEffect, WaitType |
| 上級テクニック | `.claude/skills/utage-advanced.md` | マクロ, エンティティ, GUI制御, Thread, Unity連携, SendMessage |

### 使用タイミング

以下のキーワード・話題が出たら、該当するスキルを参照し `search_utage_docs` ツールも併用すること:

- 「宴」「UTAGE」「utage」への言及 → 全スキル
- テキスト・セリフ・キャラ・背景・BGM・SE → **シナリオ基本**
- 選択肢・分岐・フラグ・条件・ジャンプ・パラメーター → **分岐・ロジック**
- アニメーション・揺れ・フェード・ズーム・エフェクト・演出 → **演出・エフェクト**
- マクロ・サブルーチン・UI制御・Unity連携・SendMessage → **上級テクニック**
- Live2D, E-mote, Spine, TextMeshPro → **RAG検索**で横断検索
- エクセルベースのシナリオ編集 → **シナリオ基本** + **RAG検索**
- 宴のセットアップ・ビルド・ローカライズ → **RAG検索**

### 注意
- 検索結果は上位10件のチャンクが返る。Claudeが内容を読んで最適な回答をまとめること
- スキルに記載のないコマンド詳細は `search_utage_docs` で補完すること
- 回答には公式ドキュメントのURLを添えること
- 詳細が不足している場合はその旨をユーザーに伝え、追加収集を提案すること
- **スキルの引数説明と実際の動作が異なる場合がある。上記「引数配置ルール」を優先すること**
