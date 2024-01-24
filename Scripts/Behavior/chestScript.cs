using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chestScript : MonoBehaviour
{
    public bool open;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        open = false;
    }
    void Update()
    {
        animator.SetBool("open", open);
    }
}
