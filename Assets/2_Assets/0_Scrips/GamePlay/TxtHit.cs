using TMPro;
using UnityEngine;

public class TxtHit : MonoBehaviour
{
    TextMeshPro _txt;
    [SerializeField] Color[] _txtColors;
    float disappearTimer;

    bool isCheck;
    bool isRun;
    private void Awake()
    {
        _txt = GetComponent<TextMeshPro>();
    }
    public void SetTxt(float Dame, bool isCheck)
    {
        this.isCheck = isCheck;
        _txt.color = _txtColors[!isCheck ? 0 : 1];
        _txt.text = ((int)Dame).ToString();
        disappearTimer = 1;
    }
    void Update()
    {
        if (isRun) return;
        float moveYSpeed = 2f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disapperTimer = 3;
            _txtColors[!isCheck ? 0 : 1].a -= disapperTimer * Time.deltaTime;
            _txt.color = _txtColors[!isCheck ? 0 : 1];
            if (_txtColors[!isCheck ? 0 : 1].a < 0)
                Destroy(gameObject);
        }

    }
}
