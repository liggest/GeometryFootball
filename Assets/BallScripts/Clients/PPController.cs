using BallScripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PPController : Singleton<PPController>
{
    private PostProcessVolume volume; //容量，里面有各个设置
    private ChromaticAberration chromatic;

    private bool isFadingIn;
    private bool isFadingOut;

    [SerializeField] private float fadeSpeed = 0.85f;



    private void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out chromatic);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ChromaticFadeIn();
        //}

        if (isFadingOut)
        {
            isFadingIn = false;
            chromatic.intensity.value -= fadeSpeed * Time.deltaTime;
            if (chromatic.intensity.value <= 0f)
            {
                isFadingOut = false;
            }
        }

        if (isFadingIn)
        {
            chromatic.intensity.value += fadeSpeed * Time.deltaTime;
            if(chromatic.intensity.value >= 0.75f)
            {
                isFadingIn = false;
                ChromaticFadeOut();
            }
        }

       
    }

    public void ChromaticFadeIn()
    {
        isFadingIn = true;
    }

    public void ChromaticFadeOut()
    {
        isFadingOut = true;
    }
}
