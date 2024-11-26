using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NationsManager : MonoBehaviour
{
    [SerializeField] Nation[] Nations;




    public static NationsManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            for (int i = 0; i < Nations.Length; i++)
            {
                Nations[i].SetTeam(i);
            }
        }
    }


    public static Nation GetNation(int teamId)
    {
        return instance.Nations[teamId];
    }

    public static int GetPlayerTeam()
    {
        for (int i = 0; i < instance.Nations.Length; i++)
        {
            if (instance.Nations[i].isPlayer)
            {
                return i;
            }
        }
        return 0;
    }
}
