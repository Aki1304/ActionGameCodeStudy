using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("�� �̸�")]
    [SerializeField] private Text _enemyName;

    [Header("�� ü�� Image")]
    [SerializeField] private Image _enemyHP;

    [Header("UI ������Ʈ")]
    [SerializeField] private GameObject _enemyUIObject;

    [Header("�´� ��ƼŬ")]
    [SerializeField] private ParticleSystem _hitParticle;

    [Header("�� ���� Image")]
    [SerializeField] private Image _enemyStun;


    private Enemy _enemy;

    #region Manager
    public PlayerManager PM { get { return PlayerManager.Instance; } }
    public UIManager UM { get { return UIManager.Instance; } }
    #endregion

    private float Amount(float cur, float max) => cur / max;


    private void Start()
    {
        _enemy = GetComponent<Enemy>();

        InitUI();
    }

    private void Update()
    {
        LookPlayer();
    }

    void InitUI()
    {
        _enemyName.text = string.Format(_enemyName.text,_enemy.GetStats.name);

        _enemyHP.fillAmount = 1f;

        //_enemyUIObject.SetActive(false);
    }


    void LookPlayer()
    {
        if (!_enemyUIObject.activeSelf) return;
        
        Vector3 dir = PM.GetPlayer.currentPos - this.transform.position;
        dir.y = 0;                      // �� �Ʒ��� �̵��ص� ������ ����
        Quaternion lotation = Quaternion.LookRotation(dir);

        _enemyUIObject.transform.rotation = lotation;
    }

    public void UITakeDamge()
    {
        StartCoroutine(ParticlePlay());

        float cur = _enemy.GetStats.curHp;
        float max = _enemy.GetStats.maxHp;

        _enemyHP.fillAmount = (cur / max);
    }

    public IEnumerator StunUIEnemy(float time)
    {
        _enemyStun.fillAmount = 1f;

        float curTime = 0f;

        while(curTime < time)
        {
            curTime += Time.deltaTime;
            float duraition = Mathf.Clamp01(1f - (curTime / time));
            _enemyStun.fillAmount = duraition;
          yield return null;
        }

        _enemyStun.fillAmount = 0f;
    }

    #region ��ƼŬ
    private IEnumerator ParticlePlay()
    {
        _hitParticle.Clear();
        _hitParticle.Play();

        yield return new WaitUntil(() => _hitParticle.isStopped);

        _hitParticle.Stop();
    }
    #endregion

}
