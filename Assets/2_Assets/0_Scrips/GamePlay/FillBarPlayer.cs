using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FillBarPlayer : MonoBehaviour
{
    [SerializeField] Image imgFillHp;
    [SerializeField] Image imgFillMana;
    public float hp;
    public float maxHp;
    public float mana;
    public float maxMana;
    bool isCheck = false;
    public bool isCheckFly;
    private void FixedUpdate()
    {
        //imgFillHp.DOFillAmount(hp / maxHp, 0.3f);
        //if (!isCheckFly)
        //    imgFillMana.DOFillAmount(mana / maxMana, 0.3f);

        imgFillHp.fillAmount = hp / maxHp;
        if(!isCheckFly)
            imgFillMana.fillAmount = mana / maxMana;
    }
    public void OnInit(float maxHp, float mana)
    {
        this.maxHp = maxHp;
        hp = maxHp;
        maxMana = 100;
        this.mana = mana;
        GamePlayManager.Instance.SetBtnUlti(mana == maxMana);
    }
    public void SetNewHp(float hp)
    {
        this.hp = Mathf.Clamp(hp, 0, maxHp);
    }
    public void SetNewMana(float mana)
    {
        GamePlayManager.Instance.SetBtnUlti(mana == maxMana);
        this.mana = Mathf.Clamp(mana, 0, maxMana);
    }
}
