﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VREscape
{
    public class GameManager : MonoBehaviour
    {
        public List<IRiddle> riddles;
        private IEnumerator<IRiddle> currentRiddle;

        private void NextRiddle()
        {
            if (currentRiddle.MoveNext())
            {
                currentRiddle.Current.OnRiddleDone += OnRiddleDoneListener;
            }
        }

        void OnRiddleDoneListener(bool value)
        {
            currentRiddle.Current.OnRiddleDone -= OnRiddleDoneListener;
            NextRiddle();
        }

        // Use this for initialization
        void Start()
        {
            try
            {
                currentRiddle = riddles.GetEnumerator();
                NextRiddle();
            }
            catch (Exception e)
            {
                
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}