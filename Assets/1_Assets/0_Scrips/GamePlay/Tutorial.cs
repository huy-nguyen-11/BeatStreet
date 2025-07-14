using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public int id;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        if (id == 0)
            anim.Play("Skill1");
        else if (id == 1)
            anim.Play("Skill2");
        else
            anim.Play("Skill3");
    }
}
