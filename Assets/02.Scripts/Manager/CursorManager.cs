using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; set; }

    [SerializeField] private Texture2D[] _cursorImage;
    [SerializeField] private Vector2 _clickSpot = new Vector2(0, 0);
    [SerializeField] private CursorMode _cursorMode = CursorMode.ForceSoftware;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetCursorSize(0);
    }

    private void Update()
    {
        if (!Input.anyKey) return;

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetCursorSize(0);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            SetCursorSize(1);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            SetCursorSize(2);
        }
    }

    public void SetCursorSize(int num)
    {
        Cursor.SetCursor(_cursorImage[num], _clickSpot, CursorMode.ForceSoftware);
    }
}
