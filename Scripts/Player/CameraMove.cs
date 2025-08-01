using System.Collections;
using System.Collections.Generic;
using UnityEditor.DeviceSimulation;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField, Tooltip("마우스 감도")] private float mouseSens = 1f;

    [SerializeField, Tooltip("카메라 높이 오프셋")] private float offsetY = 1.297f;

    [SerializeField, Tooltip("카메라 충돌 체크 레이어")] private LayerMask camCol;

    [SerializeField] private float shakeAmplitude = 0.05f; // 흔들림의 높이 (진폭)
    [SerializeField] private float shakeFrequency = 30f;  // 흔들림의 빠르기 (주파수)

    private const int _countCamIndex = 2;
    [SerializeField, Tooltip("카메라 액션")] private Transform[] _actionCamTransform = new Transform[_countCamIndex];

    private float currentX = 0f; // 마우스X 회전 값
    private float currentY = 0f; // 마우스Y 회전 값

    private float minAngle = -35f;  // 마우스Y 회전 각도 최소
    private float maxAngle = 60f;   // 마우스Y 회전 각도 최대

    #region 카메라 프로퍼티
    public Player GetPlayer { get { return PlayerManager.Instance.GetPlayer; } }
    public KeySetting KeySet { get { return GameManager.Instance.KeySet; } }
    public GameObject GetCameraObject() { return this.gameObject; }
    public float GetCamDistance { get { return GameManager.Instance.KeySet.cameraDistacne; } set { GameManager.Instance.KeySet.cameraDistacne = value; } }
    #endregion

    private bool _actionMove = false;
    private bool wasMouseLocked = false;

    /// <summary>
    /// 카메라 이동 (레이어 마스크로 충돌 감지 적용)
    /// </summary>
    public void CamMove(Vector3 charPos)
    {
        // 회전된 방향을 기반으로 카메라 목표 위치 계산
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -GetCamDistance);
        Vector3 targetPos = charPos + rotation * direction + Vector3.up * offsetY;

        CameRayCast();
        // 캐릭터 중심 바라보기
        transform.LookAt(charPos + Vector3.up * offsetY);

        void CameRayCast()
        {
            if (Physics.Raycast(charPos + Vector3.up * offsetY, targetPos
                - (charPos + Vector3.up * offsetY), out RaycastHit hit, GetCamDistance, camCol))
            { transform.position = hit.point; }
            else { transform.position = targetPos; }
        }
    }

    /// <summary>
    /// 카메라 회전
    /// </summary>
    public void CameraRotate(Transform character, Vector3 charPos)
    {
        if (_actionMove) return;

        if (KeySet.GetMouseIsLock)
        {
            // 갓 잠금된 상태라면, 첫 프레임은 입력 무시
            if (!wasMouseLocked)
            {
                wasMouseLocked = true;
                CamMove(charPos);
                return;
            }

            float mouseX = Input.GetAxis("Mouse X") * mouseSens;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSens;

            currentX += mouseX;
            currentY -= mouseY;
            currentY = Mathf.Clamp(currentY, minAngle, maxAngle);
        }
        else
        {
            wasMouseLocked = false;
        }

        // 무조건 카메라는 움직인다
        CamMove(charPos);
    }

    public IEnumerator CamAction(Vector3 enemyPos,int randomIndex)
    {
        _actionMove = true;

        Vector3 targetPos = _actionCamTransform[randomIndex].position;   // 목표 위치
        Vector3 endPos = _actionCamTransform[2].position;                // 돌아올 위치

        yield return StartCoroutine(MoveCameraTo(targetPos, enemyPos, 3.8f));

        // --- 패링 기다림 ---
        yield return new WaitUntil(() => GetPlayer.ReturnCamParrying);

        // 잠깐의 쉐이킹 효과
        StartCoroutine(MoveShakeCam(0.5f,shakeAmplitude, shakeFrequency));

        yield return StartCoroutine(MoveCameraTo(endPos, enemyPos, 5f));

        // --- 회전값 동기화 ---
        Vector3 rot = transform.rotation.eulerAngles;
        currentX = rot.y;
        currentY = rot.x;  // 마우스 입력 반영된 방향 유지

        // 거리 초기화 등 후처리
        GetCamDistance = 7f;
        wasMouseLocked = true;
        _actionMove = false;
    }

    private IEnumerator MoveCameraTo(Vector3 targetPos, Vector3 enemyPos, float moveSpeed)
    {
        // 플레이어와 적 중간 지점 기준
        Vector3 focusPoint = Vector3.Lerp(GetPlayer.currentPos, enemyPos, 0.7f) +
            new Vector3(0, 1f, 0);              // 오프셋 y축 올려서 위 바라보게 만들기

        float threshold = 0.01f;

        while (Vector3.Distance(transform.position, targetPos) > threshold)
        {
            // 위치 보간
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);

            // 회전 보간 (LookAt을 Lerp처럼 만들기)
            Quaternion targetRot = Quaternion.LookRotation(focusPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * moveSpeed);

            yield return null;
        }

        // 최종 정렬 보정
        transform.position = targetPos;
        transform.rotation = Quaternion.LookRotation(focusPoint - transform.position);
    }

    public IEnumerator MoveShakeCam(float duration, float amplitude, float frequency)
    {
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float shakeOffset = Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position += transform.up * shakeOffset;

            yield return null;
        }
    }

}
