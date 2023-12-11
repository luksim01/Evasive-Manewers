using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator sheepdogBodyAnim;
    public Animator sheepdogHeadAnim;

    // sheepdog animation
    public bool playDogJumpAnimation = false;
    public bool playDogBarkMoveCommandAnimation = false;
    public bool playDogBarkJumpCommandAnimation = false;

    // sheep animation
    public string sheepId;
    public bool playSheepJumpAnimation = false;

    // wolf animation
    public string wolfId;
    public bool playWolfBiteAnimation = false;


    // Start is called before the first frame update
    void Start()
    {
        sheepdogBodyAnim = GameObject.Find("sheepdog_body").GetComponent<Animator>();
        sheepdogHeadAnim = GameObject.Find("sheepdog_head").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playDogJumpAnimation)
        {
            sheepdogBodyAnim.Play("dog jump");
            sheepdogHeadAnim.Play("dog head jump");
            playDogJumpAnimation = false;
        }

        if (playDogBarkMoveCommandAnimation)
        {
            sheepdogHeadAnim.Play("dog head bark move");
            playDogBarkMoveCommandAnimation = false;
        }

        if (playDogBarkJumpCommandAnimation)
        {
            sheepdogHeadAnim.Play("dog head bark jump");
            playDogBarkJumpCommandAnimation = false;
        }

        if (playSheepJumpAnimation)
        {
            Animator sheepBodyAnim = GameObject.Find(sheepId + "/sheep_body").GetComponent<Animator>();
            Animator sheepHeadAnim = GameObject.Find(sheepId + "/sheep_head").GetComponent<Animator>();
            sheepBodyAnim.Play("sheep jump");
            sheepHeadAnim.Play("sheep head jump");
            playSheepJumpAnimation = false;
            sheepId = "";
        }

        if (playWolfBiteAnimation)
        {
            Animator wolfHeadAnim = GameObject.Find(wolfId + "/wolf_head").GetComponent<Animator>();
            wolfHeadAnim.Play("wolf head bite");
            playWolfBiteAnimation = false;
            wolfId = "";
        }
    }
}
