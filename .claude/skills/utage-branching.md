# UTAGE 分岐・ロジックスキル

## 概要

宴(UTAGE)シナリオにおける分岐、選択肢、パラメーター操作、条件制御を支援するスキル。

## いつ使うか

以下の話題が出たとき:
- 選択肢の表示・分岐 (Selection)
- 条件ジャンプ (Jump)
- ランダム分岐 (JumpRandom)
- If/ElseIf/Else/EndIf 条件ブロック
- パラメーター・フラグの定義と操作 (Param)
- サブルーチン・サブシナリオ
- オブジェクトクリックで分岐 (SelectionClick)
- 複数選択の擬似実装

## コマンドリファレンス

### Selection（選択肢）

| 項目 | 内容 |
|------|------|
| Command | Selection |
| Arg1 | ジャンプ先シナリオラベル |
| Arg2 | 表示条件式（空欄=無条件表示） |
| Arg3 | 選択時に実行する計算式 |
| Arg4 | 使用プレハブ名 |
| Arg5 | X座標 |
| Arg6 | Y座標 |
| Text | 選択肢テキスト |

**基本例:**
| Command | Arg1 | Text |
|---------|------|------|
| Selection | *Route_A | 森に行く |
| Selection | *Route_B | 街に行く |
| Selection | *Route_C | 海に行く |

**条件付き選択肢（隠し選択肢）:**
| Command | Arg1 | Arg2 | Text |
|---------|------|------|------|
| Selection | *Secret | flag_item==true | 秘密の道を行く |

**選択時にフラグ操作:**
| Command | Arg1 | Arg3 | Text |
|---------|------|------|------|
| Selection | *Next | route="forest" | 森に行く |
| Selection | *Next | route="city" | 街に行く |

**カスタムUI + 位置指定:**
Arg4でプレハブ名、Arg5/Arg6でXY座標（両方必須）を指定。

### SelectionClick（オブジェクトクリック分岐）

キャラやスプライトをクリックして分岐。

| 項目 | 内容 |
|------|------|
| Command | SelectionClick |
| Arg1 | ジャンプ先ラベル |
| Arg2 | 有効条件式 |
| Arg3 | 選択時計算式 |
| Arg4 | 対象オブジェクト名 |

**例: キャラクリックで分岐**
| Command | Arg1 | Arg4 |
|---------|------|------|
| SelectionClick | *Talk_Haruka | Haruka |
| SelectionClick | *Talk_Yuki | Yuki |

### Jump（条件ジャンプ）

| Command | Arg1 | Arg2 |
|---------|------|------|
| Jump | ジャンプ先ラベル | 条件式（空欄=無条件） |

**複合分岐パターン（上から順に評価）:**
| Command | Arg1 | Arg2 |
|---------|------|------|
| Jump | *GoodEnd | love>=100 |
| Jump | *NormalEnd | love>=50 |
| Jump | *BadEnd | |

### JumpRandom（ランダム分岐）

| Command | Arg1 | Arg2 | Arg3 |
|---------|------|------|------|
| JumpRandom | ジャンプ先ラベル | 条件式 | 確率重み（デフォルト1） |

**例: 重み付きランダム**
| Command | Arg1 | Arg3 |
|---------|------|------|
| JumpRandom | *Common | 5 |
| JumpRandom | *Rare | 3 |
| JumpRandom | *SuperRare | 1 |

重みは割合（5:3:1 = 約56%:33%:11%）。パラメーターも使用可（`lv/2`等）。

### If/ElseIf/Else/EndIf（条件ブロック）

| Command | Arg1 |
|---------|------|
| If | 条件式 |
| ElseIf | 条件式 |
| Else | |
| EndIf | |

**例:**
| Command | Arg1 | *(他の列)* |
|---------|------|------------|
| If | love>=100 | |
| Bg | bg_sunset | |
| ElseIf | love>=50 | |
| Bg | bg_afternoon | |
| Else | | |
| Bg | bg_cloudy | |
| EndIf | | |

**重要な制限:**
- If内にテキスト表示（Text, PageCtrl）を入れない
- サブルーチン呼び出しを入れない
- 推奨用途: Param操作、Bg切替、表示制御など非テキスト系コマンド
- テキスト分岐には **Jump** を使うこと

### Param（パラメーター操作）

**Paramシートで定義:**
| Label | Type | Value | FileType |
|-------|------|-------|----------|
| love | Int | 0 | Default |
| flag_item | Bool | false | Default |
| name | String | "プレイヤー" | Default |
| clearCount | Int | 0 | System |

**FileType:**
- **Default**: 通常セーブデータ（スロット別）
- **System**: 起動時自動ロード、全セーブ共有（周回数など）
- **Const**: 固定値（バランス調整用）

**演算子:**
- 代入: `=` `+=` `-=` `*=` `/=` `%=`
- 比較: `==` `!=` `>=` `<=` `>` `<`
- 論理: `&&` `||` `!`
- 関数: `Random(min,max)` `RandomF(min,max)` `Ceil()` `Floor()` `CeilToInt()` `FloorToInt()`

**シナリオでの使用例:**
| Command | Arg1 |
|---------|------|
| Param | love+=10 |
| Param | flag_item=true |
| Param | point=Random(1,6) |

**テキスト内でパラメーター表示:** `<param=name>さん、好感度は<param=love>です`

### サブルーチン

| Command | Arg1 | Arg2 | Arg3 |
|---------|------|------|------|
| JumpSubroutine | ジャンプ先ラベル | 条件式 | 復帰先ラベル |
| EndSubroutine | | | |
| ExitSubroutine | (全サブルーチン解除) | | |

**JumpSubroutineRandom**: ランダム版。Arg4に確率重み。

### シナリオ終了

| Command | 説明 |
|---------|------|
| EndScenario | シナリオ終了（全表示も終了） |
| PauseScenario | 一時停止（表示を残す） |
| EndPage | ページ終了（シーン回想用） |

## 応用パターン

### 複数選択（トグル式）の擬似実装

宴のSelectionは1つ選ぶと即ジャンプするため、複数選択はフラグ+ループで実現する。

**Paramシート:**
| Label | Type | Value |
|-------|------|-------|
| sel1 | Bool | false |
| sel2 | Bool | false |
| sel3 | Bool | false |

**シナリオ:**
| Command | Arg1 | Arg3 | Text |
|---------|------|------|------|
| *SelectMenu | | | |
| | | | 持っていくものを選んでね |
| Selection | *toggle1 | sel1=!sel1 | りんご |
| Selection | *toggle2 | sel2=!sel2 | みかん |
| Selection | *toggle3 | sel3=!sel3 | ぶどう |
| Selection | *Confirm | | 【決定】 |
| *toggle1 | | | |
| Jump | *SelectMenu | | |
| *toggle2 | | | |
| Jump | *SelectMenu | | |
| *toggle3 | | | |
| Jump | *SelectMenu | | |
| *Confirm | | | |

### 好感度ルート分岐

| Command | Arg1 | Arg2 |
|---------|------|------|
| Jump | *TrueEnd | love>=100 && flag_secret==true |
| Jump | *GoodEnd | love>=80 |
| Jump | *NormalEnd | love>=40 |
| Jump | *BadEnd | |

### ランダムイベント（条件付き）

| Command | Arg1 | Arg2 | Arg3 |
|---------|------|------|------|
| JumpRandom | *RareEvent | flag_item==true | 3 |
| JumpRandom | *NormalEvent | | 7 |

## 回答時の注意

- 分岐が複雑になる場合はフローチャートも併記すると分かりやすい
- If文はテキスト系に使わないよう必ず注意喚起する
- 不明点は `search_utage_docs` で追加検索すること

## 参考URL

- [選択肢](https://madnesslabo.net/utage/?page_id=1808)
- [SelectionClick](https://madnesslabo.net/utage/?page_id=1810)
- [シナリオジャンプ](https://madnesslabo.net/utage/?page_id=1804)
- [ランダム分岐](https://madnesslabo.net/utage/?page_id=1806)
- [If分岐](https://madnesslabo.net/utage/?page_id=1812)
- [パラメーター操作](https://madnesslabo.net/utage/?page_id=1802)
- [Paramシート](https://madnesslabo.net/utage/?page_id=1715)
- [サブルーチン](https://madnesslabo.net/utage/?page_id=2973)
- [選択肢チュートリアル](https://madnesslabo.net/utage/?page_id=641)
