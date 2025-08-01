using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;




public class KeySetting : MonoBehaviour
{
    // ī�޶� �Ÿ� 
    public float cameraDistacne;

    #region Ű ����
    // ������ Ű
    private readonly KeyCode[] _quickKeys = 
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Q, KeyCode.E, KeyCode.R
    };

    // �� Ű
    [HideInInspector] public KeyCode _tab = KeyCode.Tab;

    // ������ �۵� Ű 
    [HideInInspector] public KeyCode _evadeKey = KeyCode.Space;

    // ������ �۵� ����Ű
    private readonly KeyCode[] _MoveKeys =
    {
        KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D
    };

    // �и� Ű
    [HideInInspector] public KeyCode _parryingKey = KeyCode.Mouse1;     // ��Ŭ��

    // ���� �� �� ã�� Ű
    private KeyCode _pickEnemyKey = KeyCode.Mouse2;   // �� Ŭ��
    
    #endregion

    #region ��ǲ ������Ƽ
    public bool InputKey(KeyCode cap) => (Input.GetKeyDown(cap));
    public KeyCode[] QuickKeys { get { return _quickKeys; } }
    public KeyCode[] MoveKeys { get { return _MoveKeys; } }
    public KeyCode[] EvadeMoveKeys { get { return _MoveKeys; } }
    public KeyCode PickEnemyKey { get { return _pickEnemyKey; } }   // �� Ŭ��
    #endregion

    #region ���콺 ��
    private bool mouseIsLock = false;
    public bool GetMouseIsLock { get { return mouseIsLock; } set { mouseIsLock = value; } }
    #endregion

    private void Awake()
    {
        Init();
    }

    public void Update()
    {
        InputWheelMouse();
    }
    private void Init()
    {
        GetMouseIsLock = true;
        Cursor.lockState = CursorLockMode.Locked;
        cameraDistacne = 7f;
    }

    public void InputMouseLock()
    {
        GetMouseIsLock = !GetMouseIsLock;
        Cursor.lockState = (GetMouseIsLock) ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void InputWheelMouse()
    {
        if (!GetMouseIsLock) return;

        float rangeWheel = Input.GetAxis("Mouse ScrollWheel"); // + ����, - �Ʒ���
        cameraDistacne -= rangeWheel * 5f; // ������: ���ϱ� ���� �ӵ� (���� ����)
        cameraDistacne = Mathf.Clamp(cameraDistacne, 3f, 7f);  // ����� ~ �־���
    }
}
