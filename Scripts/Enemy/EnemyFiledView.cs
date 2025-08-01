using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct CastInfo
{
    public bool Hit;
    public Vector3 Point;
    public float Distance;
    public float Angle;
}

public class EnemyFiledView : MonoBehaviour
{
    [Header("�þ� ����")]
    [Range(0, 30)]
    [SerializeField] private float viewRange = 10f;
    [Range(0, 360)]
    [SerializeField] private float viewAngle = 120f;

    [Header("���̾� ����ũ")]
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Ž�� ���")]
    [SerializeField] public Transform _playerTransform;

    [Header("�� �׸���")]
    [Range(0.1f, 1f)]
    [SerializeField] private float angle = 1f;
    [SerializeField] private List<CastInfo> lineList = new();
    [SerializeField] private Vector3 offset = Vector3.zero;

    [Header("Ž�� �� ���� Ȯ��")]
    [SerializeField] private bool playerChecking = false;
    public float _delayTime = 0f;


    [Header("���������� Ž�� ����")]
    [SerializeField, Tooltip("3~5 ���� ��õ")] private float findDistance = 3f;

    private Enemy _enemy;

    #region üũ ��
    public bool GetLive() => _enemy.SetState != Enemy.EnemyState.Die;
    #endregion

    #region ���� �� Ž�� ���

    private void Start()
    {
        lineList = new();
        _enemy = GetComponent<Enemy>(); // ������Ʈ ã���ֱ�

        StartCoroutine(CheckTarget());
        StartCoroutine(DrawRayLine());
    }

    private IEnumerator CheckTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);

        while (GetLive())
        {
            if (playerChecking && _playerTransform != null) CheckTrackState();
            else SearchForTarget();

            yield return wait;
        }
    }


    private void SearchForTarget()                                                          // �þ� ���� Ȥ�� ���� ���� Ž��
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, viewRange, playerMask);

        foreach (var target in targets)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.transform.position);
            float angleToTarget = Vector3.Angle(transform.forward, direction);
            RaycastHit hit;

            if (distance <= findDistance)
            {
                FindTarget(target.transform);
                return;
            }

            if (angleToTarget <= viewAngle / 2)
            {
                if(Physics.Raycast(transform.position, direction, out hit, distance))
                {
                    if(hit.collider.CompareTag("Player"))
                    {
                        FindTarget(target.transform);
                        return;
                    }
                }

            }
        }
    }

    private void CheckTrackState()                                      // ���� �����ϰ� �ִ��� �ƴ��� �˻�
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, viewRange, playerMask);

        bool untracking = false;

        if (targets.Length == 0) untracking = true;

        foreach (var target in targets)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target.transform.position);
            float angleToTarget = Vector3.Angle(transform.forward, direction);
            RaycastHit hit;

            if (distance <= findDistance) break; // ���� ����

            if (angleToTarget <= viewAngle / 2)
            {
               if(Physics.Raycast(transform.position, direction, out hit, distance))
               {
                    if(!hit.collider.CompareTag("Player")) untracking = true;
               }
                break;
            }

            if (angleToTarget > viewAngle / 2)
            {
                if (distance <= findDistance) break; // ���� ����

                if (Physics.Raycast(transform.position, direction, distance, playerMask))
                {
                    untracking = true;
                    break;
                }
            }
        }

        if (!untracking) _delayTime = 0f;
        else
        {
            _delayTime += Time.deltaTime;
            if (_delayTime >= 0.6f) { TargetFail(); return; }
        }

        void TargetFail()
        {
            playerChecking = false;
            _playerTransform = null;
            _enemy.SetState = Enemy.EnemyState.Patrol;
        }
    }

    private void FindTarget(Transform targetTransform)
    {
        playerChecking = true;
        _playerTransform = targetTransform;

        _enemy.EnemyPathReset();
        _enemy.SetState = Enemy.EnemyState.Battle;
    }


    #endregion

    #region ���� ��ο� ��ɵ� 
    private IEnumerator DrawRayLine()
    {
        while (GetLive())
        {
            lineList.Clear();
            int count = Mathf.RoundToInt(viewAngle / angle) + 1;
            float fAngle = -(viewAngle * 0.5f) + transform.eulerAngles.y;

            for (int i = 0; i < count; ++i)
            {
                CastInfo info = GetCastInfo(fAngle + (angle * i));
                lineList.Add(info);
                Debug.DrawLine(transform.position + offset, info.Point, Color.green);
            }

            yield return null;
        }
    }

    private CastInfo GetCastInfo(float _angle)
    {
        Vector3 dir = new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0.0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
        CastInfo Info;
        RaycastHit hit;

        if (Physics.Raycast(transform.position + offset, dir, out hit, viewRange, obstacleMask))
        {
            Info.Hit = true;
            Info.Angle = _angle;
            Info.Distance = hit.distance;
            Info.Point = hit.point;
        }
        else
        {
            Info.Hit = false;
            Info.Angle = _angle;
            Info.Distance = viewRange;
            Info.Point = transform.position + offset + dir * viewRange;
        }

        return Info;
    }

    private void OnDrawGizmos()
    {
        if (lineList == null || lineList.Count == 0)
            return;

        // �̵� ��� ��
        Gizmos.color = Color.yellow;
        for (int i = 0; i < lineList.Count - 1; i++)
        {
            Vector3 from = lineList[i].Point;
            Vector3 to = lineList[i + 1].Point;
            Gizmos.DrawLine(from, to);
        }

        // ���� �þ� ���� (���� ����� ��)
        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
        Gizmos.DrawWireSphere(transform.position + offset, viewRange);

        // ���� �þ� ���� (���� ��Ȳ�� ��)
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f); // �ణ ��Ȳ��
        Gizmos.DrawWireSphere(transform.position + offset, findDistance);
    }

    private void Update()
    {
        if (_playerTransform != null)
        {
            Debug.DrawLine(transform.position + offset, _playerTransform.position + offset, Color.red);
        }
    }
    #endregion
}
