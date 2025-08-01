using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlashPerk", menuName = "Scriptable Object/Perks/SlashPerk")]

public class SlashPerk : PerkEffect
{

    public int _curLevel = 0;                            // 패시브 상승 현재 레벨

    public const int _maxLevel = 5;                      // 패시브 상승  최대 레벨

    public GameObject _slashPrefab;                      // 슬래시 Prefab
    public override void ApplyEffect()
    {
        if (_curLevel == _maxLevel) return;

        _curLevel += 1;                     // 레벨업하기

        GameObject gameObj = Instantiate(_slashPrefab, PlayerManager.Instance.GetPlayer.transform);

        gameObj.transform.position = new Vector3(gameObj.transform.position.x, 1f, gameObj.transform.position.z);

        SlashWork work = gameObj.GetComponent<SlashWork>();

        GameManager.Instance.GetPM.GetSlash.Add(work);

        if (work != null) work.GetPerk(this);
    }

    public override string StringToLevel()
    {
        return $"{_curLevel} / {_maxLevel}";
    }

    public override string StringToMainToolTip()
    {
        return $"공격 시 검격을 추가로 날립니다. {_curLevel * 3}의 고정 데미지를 줍니다. {_curLevel} 수 만큼 타수가 증가합니다.";
    }

    public override string StringToPickToolTip()
    {
        int level = (_curLevel == _maxLevel) ? _maxLevel : _curLevel + 1;
        return $"공격 시 검격을 추가로 날립니다. {level * 3}의 고정 데미지를 줍니다. 현재 퍽의 레벨 만큼 타수가 증가합니다.";
    }
    public override void InitPerk()
    {
        _curLevel = 0;
    }
}
