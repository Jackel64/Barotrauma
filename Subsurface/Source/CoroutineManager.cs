﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subsurface
{
    enum CoroutineStatus
    {
        Running, Success, Failure
    }

    // Keeps track of all running coroutines, and runs them till the end.
    static class CoroutineManager
    {
        static List<IEnumerator<object>> Coroutines = new List<IEnumerator<object>>();

        // Starting a coroutine just means adding an enumerator to the list.
        // You might also want to be able to stop coroutines or delete them,
        // which might mean putting them into a dictionary
        public static void StartCoroutine(IEnumerable<object> func)
        {
            Coroutines.Add(func.GetEnumerator());
        }

        public static void StopCoroutine(string name)
        {
            IEnumerator<object> coroutine = Coroutines.FirstOrDefault(c => c.ToString().Contains(name));

            if (coroutine != null) Coroutines.Remove(coroutine);
        }

        // Updating just means stepping through all the coroutines
        public static void Update(float deltaTime)
        {
            for (int i = Coroutines.Count-1; i>=0; i--)
            {
                if (Coroutines[i].Current != null)
                {
                    if (Coroutines[i].Current is WaitForSeconds)
                    {
                        WaitForSeconds wfs = (WaitForSeconds)Coroutines[i].Current;
                        if (!wfs.CheckFinished(deltaTime)) continue;
                                               
                    }
                    else
                    {
                        switch ((CoroutineStatus)Coroutines[i].Current)
                        {
                            case CoroutineStatus.Success:
                                Coroutines.RemoveAt(i);
                                continue;
                            case CoroutineStatus.Failure:
                                DebugConsole.ThrowError("Coroutine ''" + Coroutines[i]+ "'' has failed");
                                break;
                        }
                    }

                }


                Coroutines[i].MoveNext();


            }
        }
    }
  
    class WaitForSeconds
    {
        float timer;


        public WaitForSeconds(float time)
        {
            timer = time;
        }

        public bool CheckFinished(float deltaTime) 
        {
            timer -= deltaTime;
            return timer<=0.0f;

        }


    }


}