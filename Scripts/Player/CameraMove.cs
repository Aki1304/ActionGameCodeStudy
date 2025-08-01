using System.Collections;
using System.Collections.Generic;
using UnityEditor.DeviceSimulation;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField, Tooltip("���콺 ����")] private float mouseSens = 1f;

    [SerializeField, Tooltip("ī�޶� ���� ������")] private float offsetY = 1.297f;

    [SerializeField, Tooltip("ī�޶� �浹 üũ ���̾�")] private LayerMask camCol;

    [SerializeField] private float shakeAmplitude = 0.05f; // ��鸲�� ���� (����)
    [SerializeField] private float shakeFrequency = 30f;  // ��鸲�� ������ (���ļ�)

    private const int _countCamIndex = 2;
    [SerializeField, Tooltip("ī�޶� �׼�")] private Transform[] _actionCamTransform = new Transform[_countCamIndex];

    private float currentX = 0f; // ���콺X ȸ�� ��
    private float currentY = 0f; // ���콺Y ȸ�� ��

    private float minAngle = -35f;  // ���콺Y ȸ�� ���� �ּ�
    private float maxAngle = 60f;   // ���콺Y ȸ�� ���� �ִ�

    #region ī�޶� ������Ƽ
    public Player GetPlayer { get { return PlayerManager.Instance.GetPlayer; } }
    public KeySetting KeySet { get { return GameManager.Instance.KeySet; } }
    public GameObject GetCameraObject() { return this.gameObject; }
    public float GetCamDistance { get { return GameManager.Instance.KeySet.cameraDistacne; } set { GameManager.Instance.KeySet.cameraDistacne = value; } }
    #endregion

    private bool _actionMove = false;
    private bool wasMouseLocked = false;

    /// <summary>
    /// ī�޶� �̵� (���̾� ����ũ�� �浹 ���� ����)
    /// </summary>
    public void CamMove(Vector3 charPos)
    {
        // ȸ���� ������ ������� ī�޶� ��ǥ ��ġ ���
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -GetCamDistance);
        Vector3 targetPos = charPos + rotation * direction + Vector3.up * offsetY;

        CameRayCast();
        // ĳ���� �߽� �ٶ󺸱�
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
    /// ī�޶� ȸ��
    /// </summary>
    public void CameraRotate(Transform character, Vector3 charPos)
    {
        if (_actionMove) return;

        if (KeySet.GetMouseIsLock)
        {
            // �� ��ݵ� ���¶��, ù �������� �Է� ����
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

        // ������ ī�޶�� �����δ�
        CamMove(charPos);
    }

    public IEnumerator CamAction(Vector3 enemyPos,int randomIndex)
    {
        _actionMove = true;

        Vector3 targetPos = _actionCamTransform[randomIndex].position;   // ��ǥ ��ġ
        Vector3 endPos = _actionCamTransform[2].position;                // ���ƿ� ��ġ

        yield return StartCoroutine(MoveCameraTo(targetPos, enemyPos, 3.8f));

        // --- �и� ��ٸ� ---
        yield return new WaitUntil(() => GetPlayer.ReturnCamParrying);

        // ����� ����ŷ ȿ��
        StartCoroutine(MoveShakeCam(0.5f,shakeAmplitude, shakeFrequency));

        yield return StartCoroutine(MoveCameraTo(endPos, enemyPos, 5f));

        // --- ȸ���� ����ȭ ---
        Vector3 rot = transform.rotation.eulerAngles;
        currentX = rot.y;
        currentY = rot.x;  // ���콺 �Է� �ݿ��� ���� ����

        // �Ÿ� �ʱ�ȭ �� ��ó��
        GetCamDistance = 7f;
        wasMouseLocked = true;
        _actionMove = false;
    }

    private IEnumerator MoveCameraTo(Vector3 targetPos, Vector3 enemyPos, float moveSpeed)
    {
        // �÷��̾�� �� �߰� ���� ����
        Vector3 focusPoint = Vector3.Lerp(GetPlayer.currentPos, enemyPos, 0.7f) +
            new Vector3(0, 1f, 0);              // ������ y�� �÷��� �� �ٶ󺸰� �����

        float threshold = 0.01f;

        while (Vector3.Distance(transform.position, targetPos) > threshold)
        {
            // ��ġ ����
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);

            // ȸ�� ���� (LookAt�� Lerpó�� �����)
            Quaternion targetRot = Quaternion.LookRotation(focusPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * moveSpeed);

            yield return null;
        }

        // ���� ���� ����
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
