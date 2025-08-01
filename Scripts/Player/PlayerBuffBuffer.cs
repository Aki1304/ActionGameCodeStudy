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

    [Header("버프 이펙트")]
    [SerializeField] private ParticleSystem _buffParticel;

    private List<(SkillData inBuffer, float duraition)> _listBuffBuffer = new List<(SkillData, float )>();      // 튜플 활용
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
            if (_listBuffBuffer[i].inBuffer == data)                            //만약 버퍼 안에 똑같은 애가 존재한다면
            {
                _listBuffBuffer[i] = (_listBuffBuffer[i].inBuffer, data.SkillBuffDuration);         // 버퍼 시간 초기화를 시켜준다
                return;
            }
        }

        _listBuffBuffer.Add((data, data.SkillBuffDuration));
        UpdateStatsFromBuffs();
    }
    public void DequeBuffer(int index)
    {
        // 버프 버퍼 이미지 제거
        GetPlayerUI.GetRemoveBufferIndex(index);

        // 버프 스탯 제거
        RemoveStatsFormBuffs(index);

        // 버프 이미지 등 관리 시작
        _listBuffBuffer.RemoveAt(index);
    }

    void UseParticle()
    {
        _buffParticel.Clear();
        _buffParticel.Play();
    }

    void StopParticle()
    {
        Debug.Log("마구마구 멈추기");
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
        // 버퍼가 비어있으면 리턴
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
