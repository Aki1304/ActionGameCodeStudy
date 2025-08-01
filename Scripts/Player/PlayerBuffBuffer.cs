using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerBuffBuffer : MonoBehaviour
{
    private Character _character;
    private QuickSlot _quickSlot;

    [Header("���� ����Ʈ")]
    [SerializeField] private ParticleSystem _buffParticel;

    private List<(SkillData inBuffer, float duraition)> _listBuffBuffer = new List<(SkillData, float )>();      // Ʃ�� Ȱ��
    public IReadOnlyList<(SkillData inBuffer, float duration)> ListBuffBuffer => _listBuffBuffer;

    public PlayerUI GetPlayerUI { get { return UIManager.Instance.GetPlayerUI; } }

    private void Awake()
    {
        InitPlayerSkill();
    }

    private void Update()
    {
        BuffUpdate();
    }

    private void InitPlayerSkill()
    {
        _character = GetComponent<Character>();
        _quickSlot = GetComponent<QuickSlot>();
    }


    public void BuffEffect(int number)
    {
        SkillData effectData = _quickSlot.QuickSlotSkills[number];

        EnqueBuffer(effectData);
        StartCoroutine(ParticleStop());
    }

    public void EnqueBuffer(SkillData data)
    {
        for (int i = 0; i < _listBuffBuffer.Count; i++)
        {
            if (_listBuffBuffer[i].inBuffer == data)                            //���� ���� �ȿ� �Ȱ��� �ְ� �����Ѵٸ�
            {
                _listBuffBuffer[i] = (_listBuffBuffer[i].inBuffer, data.SkillBuffDuration);         // ���� �ð� �ʱ�ȭ�� �����ش�
                return;
            }
        }

        _listBuffBuffer.Add((data, data.SkillBuffDuration));
        UpdateStatsFromBuffs();
    }
    public void DequeBuffer(int index)
    {
        // ���� ���� �̹��� ����
        GetPlayerUI.GetRemoveBufferIndex(index);

        // ���� ���� ����
        RemoveStatsFormBuffs(index);

        // ���� �̹��� �� ���� ����
        _listBuffBuffer.RemoveAt(index);
    }

    void UseParticle()
    {
        _buffParticel.Clear();
        _buffParticel.Play();
    }

    void StopParticle()
    {
        Debug.Log("�������� ���߱�");
        _buffParticel.Stop();
    }

    private IEnumerator ParticleStop()
    {
        UseParticle();
        yield return new WaitUntil(() => !_buffParticel.isPlaying);
        StopParticle();
    }

    private void BuffUpdate()
    {
        // ���۰� ��������� ����
        if (!_listBuffBuffer.Any()) return;

        float curTime = Time.deltaTime;

        for(int i = 0; i < _listBuffBuffer.Count(); i++)
        {
            float remainTime = _listBuffBuffer[i].duraition - curTime;
            _listBuffBuffer[i] = (_listBuffBuffer[i].inBuffer, remainTime);

            if (remainTime <= 0) { DequeBuffer(i); continue; }
        }
    }

    private void UpdateStatsFromBuffs()
    {
        CharacterStatus baseStats = _character.charStats;
        
        foreach(var data in _listBuffBuffer)
        {
            BuffType type = data.inBuffer.BuffType;

            switch (type)
            {
                case BuffType.Attack:
                    {
                        baseStats._buffAtk = data.inBuffer.PercentFormula() / 100f;
                        break;
                    }
                case BuffType.Critical:
                    {
                        baseStats._buffCri = data.inBuffer.PercentFormula();
                        Debug.Log(baseStats._buffCri);
                        break;
                    }
                default: break;
            }
        }

    }

    private void RemoveStatsFormBuffs(int idx)
    {
        CharacterStatus baseStats = _character.charStats;
        BuffType type = _listBuffBuffer[idx].inBuffer.BuffType;

        switch (type)
        {
            case BuffType.Attack:
                {
                    baseStats._buffAtk = 0;
                    break;
                }
            case BuffType.Critical:
                {
                    baseStats._buffCri = 0;
                    break;
                }
            default: break;
        }

    }

}
