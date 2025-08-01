using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEditor.Animations;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Player : Character
{
    #region ������Ƽ
    public PlayerSkill GetPlayerSkill { get { return _playerSkill; } set { _playerSkill = value; } }
    public QuickSlot GetQuickSlot { get { return _quickSlot; } set { _quickSlot = value; } }
    public PerkManager PerkM { get { return GameManager.Instance.GetPM; } }
    public DungeonUI DUI { get { return UIManager.Instance.GetDGUI; } }
    #endregion

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    protected override void CharStatusSetting()
    {
        // ĳ���� ���� ����
        _status.SetCharCode(0);
        _status.SetCharStat(_isCharName, 10, 15);

        // ü�� 
        _characterHP.CharHPInit(_status);

        _playerUI.InitPlayerUI(_characterHP);
    }


    protected override void CharNormalAttack()
    {
        _playerAnimator.CharNormalAttackAnim();
    }

    private enum AttackType { Normal, S1 };

    public override void AttackRaycast(int number)
    {
        AttackType curType = ReturnAttackType();
        float rayLineDistance = (curType == AttackType.Normal) ? 2.8f : 3f;


        if (curType == AttackType.Normal)
        {
            Collider[] hitCols = Physics.OverlapSphere(transform.position, rayLineDistance);

            foreach (var target in hitCols)
            {
                if (!target.CompareTag("Enemy")) continue;

                float heightTolerance = 1.0f; // ���� ���� Ȯ���ϱ�
                if (Mathf.Abs(target.transform.position.y - transform.position.y) > heightTolerance) continue;      // �� ���� �ȿ� ������ �ѱ��

                Vector3 dirToTarget = (target.transform.position - this.transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToTarget);

                bool Hit = (number == 3) || (angle <= 60f);         // ���� 3��°�Ŵ� ���� ��Ʈ �ƴ� ���� ��ä�� ���� ���� ����

                if(Hit)
                {
                    GameObject enemyObject = target.gameObject;
                    Enemy enemy = enemyObject.GetComponent<Enemy>();


                    DUI.TakeEnemyInfo(target);
                    enemy.TakeDamage();
                    if (PerkM.GetSlash.Count > 0) { foreach(var work in PerkM.GetSlash) work.SlashPerkWork(); }
                }
            }
        }


        AttackType ReturnAttackType()
        {
            return number switch
            {
                4 => AttackType.S1,
                _ => AttackType.Normal
            };
        }
    }
    private void OnDrawGizmosSelected()
    {
        float rayLineDistance = 2.8f;
        float angleRange = 60f;
        int segmentCount = 30; // ��ä���� ������ �� ����

        // ���� ����
        Gizmos.color = new Color(1, 0, 0, 0.3f); // ������ ������

        // ���� �� ���� ����
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        // ��ä���� �� ������
        Quaternion leftRayRotation = Quaternion.Euler(0, -angleRange, 0);
        Quaternion rightRayRotation = Quaternion.Euler(0, angleRange, 0);
        Vector3 leftBoundary = leftRayRotation * forward;
        Vector3 rightBoundary = rightRayRotation * forward;

        // ������ �� �׸���
        Gizmos.DrawLine(origin, origin + leftBoundary * rayLineDistance);
        Gizmos.DrawLine(origin, origin + rightBoundary * rayLineDistance);

        // ��ä�� ���� ä��� (������ ǥ��)
        Vector3 prevPoint = origin + leftBoundary * rayLineDistance;
        for (int i = 1; i <= segmentCount; i++)
        {
            float lerpFactor = (float)i / segmentCount;
            float angle = Mathf.Lerp(-angleRange, angleRange, lerpFactor);
            Quaternion segmentRot = Quaternion.Euler(0, angle, 0);
            Vector3 segmentDir = segmentRot * forward;
            Vector3 nextPoint = origin + segmentDir * rayLineDistance;

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        // �߽ɿ� OverlapSphere �� �׸���
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, rayLineDistance / 2);
    }

}
