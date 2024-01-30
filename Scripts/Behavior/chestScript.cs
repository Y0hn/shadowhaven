using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chestScript : MonoBehaviour
{
    public GameObject interactable;
    private Animator animator;
    private bool isEnabled;
    private bool open;

    void Start()
    {
        animator = GetComponent<Animator>();
        isEnabled = true;
        open = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Player" && isEnabled)
        {
            if (open)
            {
                //Debug.Log("Dotyka sa chestky");
                animator.SetBool("open", open);
                interactable.SetActive(open);
                interactable.GetComponent<Interactable>().enabled = open;
                open = !open;
            }
            else
            {
                interactable.GetComponent<Interactable>().AddToInventory();
                isEnabled = false;
                // TO BE REMOVED
                GameManager.instance.EndTheGame();
            }
        }
    }
}
