using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    public class RiddleSafe : MonoBehaviour, IRiddle
    {
        public event Action<bool> OnRiddleDone;

        public void StartRiddle()
        {
            throw new NotImplementedException();
        }
    }
}
