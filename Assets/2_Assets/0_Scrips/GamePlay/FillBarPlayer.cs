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
    public void OnInit(float maxHp, float mana)
    {
        this.maxHp = maxHp;
        hp = maxHp;
        maxMana = 100;
        this.mana = mana;
        imgFillHp.fillAmount = 1f;
        imgFillMana.fillAmount = mana / maxMana;
        GamePlayManager.Instance.SetBtnUlti(mana == maxMana);
    }
    public void SetNewHp(float hp)
    {
        this.hp = Mathf.Clamp(hp, 0, maxHp);
        imgFillHp.fillAmount = this.hp / maxHp;
    }
    public void SetNewMana(float mana)
    {
        GamePlayManager.Instance.SetBtnUlti(mana == maxMana);
        this.mana = Mathf.Clamp(mana, 0, maxMana);
        if (!isCheckFly)
            imgFillMana.fillAmount = this.mana / maxMana;
    }
}
