using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ワクチン収集システムの進行を管理するManager
/// </summary>
public class VaccineManager : MonoBehaviour
{
    [SerializeField] private List<VaccineView> vaccineItems;
    private VaccineModel _model;

    private void Awake()
    {
        _model = new VaccineModel();

        // 各アイテムのインタラクトイベントを購読
        foreach (var item in vaccineItems)
        {
            if (item != null)
            {
                item.OnInteracted += () => 
                {
                    _model.Collect(item.Type);
                    Debug.Log($"[VaccineManager] ワクチン回収！ 現在: {_model.CollectedCount}個, 種類: {item.Type}");
                };
            }
        }

        _model.OnVaccineCollectedWithType += (type) =>
        {
            UIEvents.OnVaccineCollected?.Invoke(type);
        };

        // 全て集まった時の処理
        _model.OnAllVaccinesCollected += () =>
        {
            Debug.Log("[VaccineManager] 3つのワクチンを全て回収しました！クリア条件達成。");
            SessionData.SetVaccineClear(true);
        };
    }
}
