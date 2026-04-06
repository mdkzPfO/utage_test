using UnityEngine;
using Utage;

/// <summary>
/// AdvEngineの設定を初期化完了後に上書きする
/// </summary>
public class AdvConfigOverride : MonoBehaviour
{
    void Awake()
    {
        var engine = FindFirstObjectByType<AdvEngine>();
        if (engine != null)
        {
            engine.OnPostInit.AddListener(OnEnginePostInit);
        }
    }

    void OnEnginePostInit()
    {
        var engine = FindFirstObjectByType<AdvEngine>();
        if (engine != null)
        {
            engine.Config.IsSkipUnread = true;
            engine.Config.IsStopSkipInSelection = false;
        }
    }
}
