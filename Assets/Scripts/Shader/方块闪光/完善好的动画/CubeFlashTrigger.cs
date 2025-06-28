using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CubeFlashTrigger : SingletonMono<CubeFlashTrigger>
{ 
    CubeFlashTrigger() { }
    public Animator FlashAnimator;
    private void Start()
    {
        FlashAnimator = GetComponent<Animator>();
    }

    public void FlashTrigger()//¥•∑¢…¡π‚
    {
        FlashAnimator.SetTrigger("Flash");
    }
    // Update is called once per frame
    void Update()
    {
    }
}
