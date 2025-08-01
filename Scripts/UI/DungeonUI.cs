using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonUI : MonoBehaviour
{
    public List<Collider> GetPickCollider { get; set; } = new List<Collider>();

    public PerkUI _perUI;
    public PickEnemyInfo _pickEnemyInfo;

    public Enemy ReturnPickEnemy { get; set; }  
    
    private int _pickEnemyIndex = -1;

    public void InitClass()
    {
        _perUI.PerkInit();
        _pickEnemyInfo.PickEnemyInit();
    }

    #region 던전 내 플레이어 범위 안 조사
    public void Update()
    {
        ColliderRangeEnemy();
        PickEnemyOne();
    }

    public void ColliderRangeEnemy()
    {
        if (PlayerManager.Instance.GetPlayer == null) return;                   // 플레이어가 없으면 실행하지 않음.

        Collider[] tempCols = Physics.OverlapSphere(PlayerManager.Instance.GetPlayer.currentPos, 15f, LayerMask.GetMask("Enemy"));

        GetPickCollider.Clear();
        GetPickCollider.AddRange(tempCols);
    }

    private void PickEnemyOne()
    {
        if (GetPickCollider.Count == 0)
        {
            if(_pickEnemyIndex != -1)  _pickEnemyIndex = -1;
            if (ReturnPickEnemy != null) ReturnPickEnemy = null;
            return;
        }

        if (Input.GetKeyDown(GameManager.Instance.KeySet.PickEnemyKey))
        {
            _pickEnemyIndex++;                                                  // 인덱스 업데이트

            if (_pickEnemyIndex >= GetPickCollider.Count) _pickEnemyIndex = 0;  // 인덱스 값 초기화

            SetPickUI(GetPickCollider[_pickEnemyIndex], _pickEnemyIndex);
        }
    }

    public void TakeEnemyInfo(Collider other)
    {
        for(int i = 0; i < GetPickCollider.Count; i++)
        {
            if(other == GetPickCollider[i])
            {
                SetPickUI(other, i);
                break;
            }
        }
    }

    void SetPickUI(Collider pickCol, int idx)
    {
        _pickEnemyIndex = idx;
        ReturnPickEnemy = pickCol.GetComponent<Enemy>();
        _pickEnemyInfo.EnableObjectsUI();
    }

    #endregion
}
