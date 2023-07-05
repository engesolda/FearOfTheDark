using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
    }

    public void ConfigureInstance(string _txt, Color _color, int _fontSize)
    {
        animator.GetComponent<Text>().text = _txt;
        animator.GetComponent<Text>().color = _color;
        animator.GetComponent<Text>().fontSize = _fontSize;
    }
}
