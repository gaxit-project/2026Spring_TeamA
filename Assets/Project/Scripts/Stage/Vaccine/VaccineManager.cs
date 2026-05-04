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
            item.OnInteracted += () => _model.Collect();
        }

        // 全て集まった時の処理
        _model.OnAllVaccinesCollected += () =>
        {
            SessionData.SetVaccineClear(true);

            if (GamePresenter.Instance != null)
            {
                GamePresenter.Instance.TriggerGameClear();
            }
        };
    }
}
