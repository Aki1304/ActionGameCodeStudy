using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class PickEnemyInfo : MonoBehaviour
{
    public DungeonUI _dgUI { get { return UIManager.Instance.GetDGUI; } }

    [SerializeField] private Image _pickEnemyHP;
    [SerializeField] private Image _underEnemyHP;

    [SerializeField] private Text _pickEnemyName;
    [SerializeField] private Text _pickEnemyTextHP;

    public bool ReturnChbeckHP { get; set; }

    // Amount계산식
    private float Amount(float cur, float max) => cur / max;

    public void PickEnemyInit()
    {
        if(this.gameObject.activeSelf) this.gameObject.SetActive(false);
    }

    public void EnableObjectsUI()
    {
        if(!this.gameObject.activeSelf) this.gameObject.SetActive(true);

        EnemyStats stats = _dgUI.ReturnPickEnemy.GetStats;

        _pickEnemyHP.fillAmount = Amount(stats.curHp, stats.maxHp);
        _underEnemyHP.fillAmount = _pickEnemyHP.fillAmount;
    }

    private void PickEnemyHP()
    {
        if (_dgUI.ReturnPickEnemy == null) { return; }                      // 객체 값이 없으면 실행하지 않음

        EnemyStats stats = _dgUI.ReturnPickEnemy.GetStats;

        _pickEnemyHP.fillAmount = Amount(stats.curHp, stats.maxHp);

        float epsilon = 0.001f;

        if (Mathf.Abs(_underEnemyHP.fillAmount - _pickEnemyHP.fillAmount) < epsilon)
        {
            _underEnemyHP.fillAmount = _pickEnemyHP.fillAmount;
        }
        else
        {
            _underEnemyHP.fillAmount = Mathf.Lerp(_underEnemyHP.fillAmount, _pickEnemyHP.fillAmount, Time.deltaTime * 3.5f);
        }

    }

    private void PickEnemyText()
    {
        if (_dgUI.ReturnPickEnemy == null) return;

        EnemyStats stats = _dgUI.ReturnPickEnemy.GetStats;

        _pickEnemyName.text = stats.name;
        _pickEnemyTextHP.text = $"{stats.curHp}/{stats.maxHp}";
    }

    private void Update()
    {
        if(_dgUI.GetPickCollider.Count == 0)
        {
            PickEnemyInit();
            return;
        }

        PickEnemyHP();
        PickEnemyText();

    }

    
}
