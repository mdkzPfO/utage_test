using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Utage;

namespace MyProject.Tests
{
    public class ScenarioTest
    {
        [UnityTest]
        public IEnumerator ScenarioRunTest()
        {
            Debug.Log("ScenarioRunTest: Start");

            // シーンをロード
            yield return SceneManager.LoadSceneAsync("MyProject");
            Debug.Log("ScenarioRunTest: Scene MyProject Loaded");

            // タイトル画面があればスタートボタンを押す
            UtageUguiTitle title = null;
            for(int i = 0; i < 10; ++i)
            {
                title = Object.FindAnyObjectByType<UtageUguiTitle>(FindObjectsInactive.Include);
                if (title != null) break;
                yield return null;
            }

            if (title != null)
            {
                Debug.Log($"ScenarioRunTest: UtageUguiTitle found. Calling OnTapStart by {title}");
                title.OnTapStart();
                yield return null;
                Debug.Log("ScenarioRunTest: OnTapStart called.");
            }
            else
            {
                Debug.Log("ScenarioRunTest: UtageUguiTitle NOT found.");
            }

            // AdvEngineを取得
            var engine = Object.FindAnyObjectByType<AdvEngine>();
            Assert.IsNotNull(engine, "AdvEngine not found in the scene.");
            Debug.Log("ScenarioRunTest: AdvEngine found.");

            // テスト用ヘルパーコンポーネントを追加
            var tester = engine.gameObject.AddComponent<ScenarioTester>();
            tester.Engine = engine;
            Debug.Log("ScenarioRunTest: ScenarioTester added.");

            // シナリオ終了まで待機
            float timeout = 30.0f; // タイムアウト設定 (60秒 -> 120秒)
            float time = 0;
            int lastPage = -1;

            while (!tester.IsFinished)
            {
                if(engine.Page.PageNo != lastPage)
                {
                    Debug.Log($"ScenarioRunTest: Current Page = {engine.Page.PageNo}");
                    lastPage = engine.Page.PageNo;
                }

                if (time > timeout)
                {
                    Debug.LogError($"ScenarioRunTest: Timed out! Current Page: {engine.Page.PageNo}");
                    Assert.Fail($"Scenario timed out. Current Page: {engine.Page.PageNo}");
                    yield break;
                }
                time += Time.deltaTime;
                yield return null;
            }

            Assert.IsFalse(tester.HasError, "Scenario finished with errors.");
            Debug.Log("ScenarioRunTest: Finished successfully.");
        }

        // シナリオ自動進行＆監視用ヘルパー
        public class ScenarioTester : MonoBehaviour
        {
            public AdvEngine Engine { get; set; }
            public bool IsFinished { get; private set; } = false;
            public bool HasError { get; private set; } = false;

            void Start()
            {
                if (Engine == null) return;
                
                // イベント登録
                Engine.SelectionManager.OnBeginWaitInput.AddListener(OnBeginWaitInput);
                Engine.ScenarioPlayer.OnEndScenario.AddListener(OnEndScenario);
                Debug.Log("ScenarioTester: Started and events registered.");
            }

            void OnDestroy()
            {
                if (Engine == null) return;

                Engine.SelectionManager.OnBeginWaitInput.RemoveListener(OnBeginWaitInput);
                Engine.ScenarioPlayer.OnEndScenario.RemoveListener(OnEndScenario);
            }

            // 選択肢が表示されたらランダムに選択
            void OnBeginWaitInput(AdvSelectionManager selection)
            {
                Debug.Log("ScenarioTester: OnBeginWaitInput");
                int totalCount = selection.Selections.Count + selection.SpriteSelections.Count;
                if (totalCount > 0)
                {
                    // ランダムに選択 (必要に応じてロジック変更)
                    int index = Random.Range(0, totalCount);
                    Debug.Log($"ScenarioTester: Selecting index {index} from total {totalCount}");

                    if (index < selection.Selections.Count)
                    {
                        selection.Select(index);
                    }
                    else
                    {
                        selection.Select(selection.SpriteSelections[index - selection.Selections.Count]);
                    }
                }
            }

            // クリック待ちなどで止まらないように自動進行
            float logTimer = 0;
            void Update()
            {
                if (Engine == null || IsFinished) return;

                logTimer += Time.deltaTime;
                if (logTimer > 1.0f)
                {
                   Debug.Log($"ScenarioTester: Status={Engine.Page.Status}, IsWaitInputInPage={Engine.Page.IsWaitInputInPage}, IsWaitBrPage={Engine.Page.IsWaitBrPage}, IsWaitTextCommand={Engine.Page.IsWaitTextCommand}");
                   logTimer = 0;
                }

                // メッセージ送りなどの入力待ち状態なら進める
                if (Engine.Page.IsWaitInputInPage || Engine.Page.IsWaitBrPage || Engine.Page.IsSendChar)
                {
                    Engine.Page.InputSendMessage();
                }
            }

            // シナリオ終了検知
            void OnEndScenario(AdvScenarioPlayer player)
            {
                Debug.Log("ScenarioTester: OnEndScenario");
                IsFinished = true;
            }
        }
    }
}
