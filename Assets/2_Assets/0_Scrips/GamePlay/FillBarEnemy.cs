using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillBarEnemy : MonoBehaviour
{
    [SerializeField] Image imgFillHp;
    public float hp;
    public float maxHp;
    bool isCheck = false;
    public bool isCheckFly;
    Coroutine coroutine;
    private void Start()
    {
        SetFadeImg();
    }
    private void FixedUpdate()
    {
        imgFillHp.fillAmount = hp / maxHp;
    }
    public void OnInit(float maxHp)
    {
        this.maxHp = maxHp;
        hp = maxHp;
    }
    public void SetNewHp(float hp)
    {
        SetAnimFade();
        this.hp = Mathf.Clamp(hp, 0, maxHp);
        if (this.hp <= 0)
            SetFadeImg();
    }
    private void SetFadeImg()
    {
        transform.GetChild(0).GetComponent<Image>().DOKill();
        transform.GetChild(0).GetChild(0).GetComponent<Image>().DOKill();
        transform.GetChild(1).GetComponent<TextMeshPro>().DOKill();
        transform.GetChild(0).GetComponent<Image>().DOFade(0, 0);
        transform.GetChild(0).GetChild(0).GetComponent<Image>().DOFade(0, 0);
        transform.GetChild(1).GetComponent<TextMeshPro>().DOFade(0, 0);

    }
    public void SetAnimFade()
    {
        transform.GetChild(0).GetComponent<Image>().DOKill();
        transform.GetChild(0).GetChild(0).GetComponent<Image>().DOKill();
        transform.GetChild(1).GetComponent<TextMeshPro>().DOKill();
        transform.GetChild(0).GetComponent<Image>().DOFade(1, 0);
        transform.GetChild(0).GetChild(0).GetComponent<Image>().DOFade(1, 0);
        transform.GetChild(1).GetComponent<TextMeshPro>().DOFade(1, 0);
        transform.GetChild(0).GetComponent<Image>().DOFade(0, 2);
        transform.GetChild(0).GetChild(0).GetComponent<Image>().DOFade(0, 2);
        transform.GetChild(1).GetComponent<TextMeshPro>().DOFade(0, 2);
    }
}
