using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface IRiddle {

    void StartRiddle();
    event Action<bool> OnRiddleDone;

}
