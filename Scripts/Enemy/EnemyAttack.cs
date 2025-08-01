using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public enum EnemyAttackState
{
    Pan, Square, Circle
}

/// <summary>
/// Enemy �� ���� �� ���ݹ� ��ƼŬ �ε������� ����
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    [Header("�ش� ������ Indicate MeshRender")]
    [SerializeField] private MeshRenderer[] _thisMeshRender;

    private List<MaterialPropertyBlock> _InsMesh = new List<MaterialPropertyBlock>();

    [Header("�ش� ������ Indicate Object ����")]
    [SerializeField] public GameObject[] _thisIndicator;

    [Header("�ش� ������ Particle ����")]
    [SerializeField] public ParticleSystem[] _particleSystem;

    [Header("������ �и� ����Ʈ")]
    [SerializeField] public ParticleSystem _parryingParticle;

    [Header("�� ���ݿ� ���� state �迭")]
    [SerializeField] private EnemyAttackState[] _enemyArrayStates;

    [Header("�ݶ��̴� Object")]
    [SerializeField] private GameObject _enemyColObject;

    private Enemy _enemy;
    private Animator _animator;
    private EnemyUI _enemyUI;

    private int _curIdxNum;

    public bool _enemyParried = false;
    private bool _isStunWait =false;

    #region ������Ƽ
    private Player _player { get { return PlayerManager.Instance.GetPlayer; } }
    #endregion

    public void Start()
    {
        // ��ü ����
        _enemy = GetComponent<Enemy>();
        _enemyUI = GetComponent<EnemyUI>();
        _animator = GetComponent<Animator>();

        // �޽������� block���� ������Ƽ ���� �� ������ ����
        foreach (var renderer in _thisMeshRender)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(propBlock);
            _InsMesh.Add(propBlock);
        }

        // ���� �� ���� Collider�� ������ ���� ���� ������Ʈ ����
        foreach(var component in _enemyColObject.GetComponents<Collider>()) component.enabled = false;
    }

    public void SetIndicateRange(float var)
    {
        _InsMesh[_curIdxNum].SetFloat("_Range", var);
        _thisMeshRender[_curIdxNum].SetPropertyBlock(_InsMesh[_curIdxNum]);
    }


    public IEnumerator ReceiveHash(int stateHash, int idxNum, float attackTime)
    {
        AnimatorStateInfo stateinfo;
        _curIdxNum = idxNum;

        bool End = false;
        bool firstCheck = false;

        bool Check()
        {
            if (_enemy.IsCheckStop()) return true;

            stateinfo = _animator.GetCurrentAnimatorStateInfo(0);
            return stateinfo.shortNameHash == stateHash;
        }

        yield return new WaitUntil(() => Check());

        _thisIndicator[_curIdxNum].SetActive(true);

        while (true)
        {
            if (_enemy.IsCheckStop()) break;

            stateinfo = _animator.GetCurrentAnimatorStateInfo(0);

            float animTime = stateinfo.normalizedTime;
            float progress = Mathf.Clamp01(animTime / attackTime);

            SetIndicateRange(progress);
            EnemyEffectDiff(progress);

            if (End) break;

            yield return null; // ������ ������ ����
        }

        _thisIndicator[_curIdxNum].SetActive(false);

        
        void EnemyEffectDiff(float progress)                        // �� ĳ���͸��� �ٸ� Ư���� �ְ� �ʹٸ� �ۼ�
        {
            switch (_enemy._monster)
            {
                case Enemy.Monster:
                    {
                        if (idxNum == 3)
                        {
                            if(progress >= 0.9f && !firstCheck)
                            {
                                firstCheck = true;
                                Invoke("StateCollider",1f);
                                UseParticle(idxNum);
                                End = true;
                                break;
                            }
                        }
                        else ReturnDefault(); break;

                    }
            }

            void ReturnDefault()
            {
                if (progress >= 1.0f)
                {
                    AttackEffect();
                    StartCoroutine(ReturnParticle());
                    End = true;
                }
            }
        }

    }


    /// <summary>
    /// �� Ÿ�� �ݶ��̴� ���� �� ��ƼŬ ����
    /// index ����
    /// </summary>
    public void AttackEffect()
    {
        StateCollider();
        UseParticle(_curIdxNum);
    }

    private void StateCollider()
    {
        EnemyAttackState state = _enemyArrayStates[_curIdxNum];

        // ������ �ε��������� ������ �� ����
        Vector3 size = _thisIndicator[_curIdxNum].transform.localScale;
        Vector3 pos = _thisIndicator[_curIdxNum].transform.localPosition;

        switch (state)
        {
            case EnemyAttackState.Pan:
                {
                    float panAngle = _thisMeshRender[_curIdxNum].material.GetFloat("_Angle"); ;
                    float radius = transform.localScale.x * size.x * 0.5f;                  // ���� �θ� ������Ʈ ������ ���� ���������... ���� ������ �����ֱ� 

                    Collider[] hitCols = Physics.OverlapSphere(transform.position, radius);

                    //Debug.Log($"���� ũ��{panAngle} radius {radius}");

                    foreach(var target in hitCols)
                    {
                        Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
                        float angle = Vector3.Angle(transform.forward, dirToTarget);

                        if (angle <= panAngle / 2f)
                        {
                            if(target.CompareTag("Player"))
                            {
                                // ������ Ÿ���� ���� ��Ų��. �Ŵ������� �̹� Player�� �޸𸮸� ���� ����.
                                // �׷��� ���Ǹ� �����ϸ� �׳� �Ŵ������� �ٷ� Ÿ�� ����
                                //Debug.Log("��ä�� ��");
                                _player.TakeDamage(_enemy.GetStats.atk);
                                break;
                            }
                        }
                    }

                    break;
                }
            case EnemyAttackState.Square:
                {
                    BoxCollider col = _enemyColObject.GetComponent<BoxCollider>();

                    col.center = pos;
                    col.size = new Vector3(size.x, size.z, size.y);

                    col.enabled = true;
                    break;
                }
            case EnemyAttackState.Circle:
                {
                    SphereCollider col = _enemyColObject.GetComponent<SphereCollider>();
                    
                    col.center = pos;
                    col.radius = size.x * 0.5f;     // radius �� ������...

                    col.enabled = true;
                    break;
                }
            default: break;
        }

      
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            // �ȿ� �÷��̾ �ִٸ�
            Debug.Log("Ÿ�� ����");
            _player.TakeDamage(_enemy.GetStats.atk);
            DisableComponent();
        }
        else
        { DisableComponent(); Debug.Log("���� ����"); }


        void DisableComponent()
        {
            // ���� �� ���� Collider�� ������ ���� ���� ������Ʈ ����
            foreach (var component in _enemyColObject.GetComponents<Collider>())
            {
                if (component.enabled)
                {
                    component.enabled = false;
                }

            }
        }

    }


    private void ParryingParticle()
    {
        _parryingParticle.Clear();
        _parryingParticle.Play();

        StartCoroutine(ParryingAction());

        IEnumerator ParryingAction()
        {
            float startTime = Time.time;
            float distancetime = 0.6f;

            while(Time.time - startTime <= distancetime)
            {
                //Debug.Log($"üŷ{_enemyParried}�и����� {_enemy._isStun}���� ����");
                if(!_enemyParried) _enemyParried = true;
                if (_enemy._isStun) { _enemyParried = false; break; }
                yield return null;
            }

            _enemyParried = false;


            if (_enemy._isStun)                  // ������ ���
            {
                yield return new WaitUntil(() => _isStunWait);
                yield return StartCoroutine(_enemyUI.StunUIEnemy(5f));
                _animator.speed = 1f;
                _enemy._isStun = false;
                _isStunWait = false;

            }
        }
    }

    public void SetSuccessParrying()
    {
        _enemy._isStun = true;               // ���� ���·� ����.

        Vector3 playerPos = PlayerManager.Instance.GetPlayer.currentPos;
        transform.LookAt(playerPos);

        _animator.SetTrigger("Stun");
    }

    private void StunDelay()
    {
        _animator.speed = 0f;
        _isStunWait = true;
    }

    private void UseParticle(int idxNum)
    {
        _particleSystem[idxNum].Clear();
        _particleSystem[idxNum].Play();
    }
    public void StopParticle(int idxNum)
    {
        _particleSystem[idxNum].Stop();
    }

    private IEnumerator ReturnParticle()
    {
        yield return new WaitUntil(() => _particleSystem[_curIdxNum].isStopped);
        //Debug.Log("��ƼŬ ����");
        StopParticle(_curIdxNum);
    }
}
