using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : SingleTon<OptionManager>
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }


    
}
