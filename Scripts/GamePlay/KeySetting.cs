using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;




public class KeySetting : MonoBehaviour
{
    // 카메라 거리 
    public float cameraDistacne;

    #region 키 매핑
    // 퀵슬룻 키
    private readonly KeyCode[] _quickKeys = 
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Q, KeyCode.E, KeyCode.R
    };

    // 탭 키
    [HideInInspector] public KeyCode _tab = KeyCode.Tab;

    // 구르기 작동 키 
    [HideInInspector] public KeyCode _evadeKey = KeyCode.Space;

    // 구르기 작동 방향키
    private readonly KeyCode[] _MoveKeys =
    {
        KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D
    };

    // 패링 키
    [HideInInspector] public KeyCode _parryingKey = KeyCode.Mouse1;     // 우클릭

    // 범위 내 적 찾는 키
    private KeyCode _pickEnemyKey = KeyCode.Mouse2;   // 휠 클릭
    
    #endregion

    #region 인풋 프로퍼티
    public bool InputKey(KeyCode cap) => (Input.GetKeyDown(cap));
    public KeyCode[] QuickKeys { get { return _quickKeys; } }
    public KeyCode[] MoveKeys { get { return _MoveKeys; } }
    public KeyCode[] EvadeMoveKeys { get { return _MoveKeys; } }
    public KeyCode PickEnemyKey { get { return _pickEnemyKey; } }   // 휠 클릭
    #endregion

    #region 마우스 락
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

        float rangeWheel = Input.GetAxis("Mouse ScrollWheel"); // + 위로, - 아래로
        cameraDistacne -= rangeWheel * 5f; // 조절감: 곱하기 값은 속도 (조절 가능)
        cameraDistacne = Mathf.Clamp(cameraDistacne, 3f, 7f);  // 가까움 ~ 멀어짐
    }
}
