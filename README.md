# ScenarioManager — UTAGE RAG System

Unity用ビジュアルノベルツール「宴(UTAGE)」の公式ドキュメントをRAG化し、Claude CodeからMCPツールとして検索できるシステム。

## 目的

ゲーム開発において、既存シナリオシステムの移行や演出の実現可否を確認する際に、宴のドキュメントを自然言語で検索して回答を得る。

## アーキテクチャ

```
Claude Code
  │ stdin/stdout (MCP Protocol)
  ▼
MCP Server (ローカル Node.js)
  │ AWS SDK (HTTPS)
  ├── Bedrock Titan Embedding V2  ← クエリをベクトル化
  └── S3 Vectors                  ← コサイン類似度で検索
        └── utage-docs-index (174ベクトル)
```

### ポイント
- Lambda, API Gateway, OpenSearch 等のインフラは不要
- MCP Serverはローカルの Node.js プロセスとして Claude Code が自動起動
- S3 Vectors は従量課金のみで月額アイドルコスト 0 円

## データパイプライン

```
宴 公式サイト (67ページ)
  │ WebFetch でスクレイピング
  ▼
S3 (ap-northeast-1)
  s3://utage-game-assets-787306238618/utage-docs/
  │ Markdown 67ファイル
  ▼
ingest.mjs
  │ ① ## 見出しでチャンク分割 (174チャンク)
  │ ② Titan Embedding V2 で 1024次元ベクトル化
  │ ③ メタデータ付きで S3 Vectors に PUT
  ▼
S3 Vectors (us-east-1)
  bucket: bedrock-knowledge-base-dx2712
  index:  utage-docs-index
```

## ディレクトリ構成

```
ScenarioManager/
├── README.md
├── .mcp.json              ← Claude Code の MCP サーバー設定
└── utage-rag/
    ├── package.json
    ├── scripts/
    │   └── ingest.mjs     ← データ投入スクリプト
    └── src/
        ├── index.mjs      ← MCP Server エントリポイント
        ├── embedding.mjs  ← Titan Embedding V2 呼び出し
        └── search.mjs     ← S3 Vectors 検索ロジック
```

## セットアップ

### 前提条件
- Node.js v22+
- AWS CLI 設定済み (us-east-1 リージョンへのアクセス)
- Bedrock Titan Embedding V2 のモデルアクセス有効化

### インストール
```bash
cd utage-rag
npm install
```

### データ投入 (初回のみ)
```bash
npm run ingest
```

### Claude Code で利用
`.mcp.json` が配置されていれば、Claude Code 起動時に自動で MCP Server が立ち上がる。
宴に関する質問をすると `search_utage_docs` ツールが自動的に呼ばれる。

### 他のプロジェクトから利用する場合
そのプロジェクトの `.mcp.json` に以下を追加:
```json
{
  "mcpServers": {
    "utage-rag": {
      "command": "node",
      "args": ["utage-rag/src/index.mjs"],
      "cwd": "c:\\Users\\silve\\OneDrive\\Desktop\\ScenarioManager"
    }
  }
}
```

## MCP ツール仕様

### search_utage_docs

| パラメータ | 型 | 必須 | 説明 |
|-----------|-----|------|------|
| query | string | Yes | 検索クエリ (日本語/英語) |
| category | enum | No | カテゴリフィルタ |
| top_k | number | No | 取得件数 (デフォルト: 10, 最大: 20) |

**カテゴリ一覧:** tutorial, reference, manual, extension, textmeshpro, sample, unity-general, case-study, qa-download

## コスト

| 項目 | コスト |
|------|--------|
| 初回データ投入 (Embedding) | 約 0.1 円 |
| S3 Vectors ストレージ (月額) | 約 0.01 円 |
| 検索クエリ (月300回想定) | 約 0.001 円 |
| **月額アイドル** | **実質 0 円** |

## AWS リソース

| サービス | リージョン | リソース名 |
|---------|-----------|-----------|
| S3 | ap-northeast-1 | utage-game-assets-787306238618 |
| S3 Vectors | us-east-1 | bedrock-knowledge-base-dx2712 |
| S3 Vectors Index | us-east-1 | utage-docs-index |
| Bedrock | us-east-1 | amazon.titan-embed-text-v2:0 |
