using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegManager : MonoBehaviour
{
    [SerializeField] private GameObject[] roundPegs;
    [SerializeField] private GameObject[] squarePegs;
    [SerializeField] private GameObject[] formations;
    [SerializeField] private GameObject formation;

    void Start()
    {
        newBoard();
    }

    public void newBoard()
    {
        if(formation != null)
        {
            Destroy(formation);
        }

        int rnd = Random.Range(0, formations.Length);
        formation = Instantiate(formations[rnd], transform.position, Quaternion.identity);

        formation.transform.parent = gameObject.transform;
    }

    public void respawnPeg(int type, int amount = 1)
    {
        if(type == 1)
        {
            GameObject[] shuffledRoundPegs = new GameObject[roundPegs.Length];
            roundPegs.CopyTo(shuffledRoundPegs, 0);

            for (int i = 0; i < shuffledRoundPegs.Length; i++)
            {
                int rnd = Random.Range(0, shuffledRoundPegs.Length);
                GameObject tempGO = shuffledRoundPegs[rnd];
                shuffledRoundPegs[rnd] = shuffledRoundPegs[i];
                shuffledRoundPegs[i] = tempGO;
            }

            int amountAdded = 0;
            for (int i = 0; i < shuffledRoundPegs.Length; i++)
            {
                if (!shuffledRoundPegs[i].activeSelf)
                {
                    shuffledRoundPegs[i].SetActive(true);
                    amountAdded++;
                }

                if (amountAdded >= amount) break;
            }
        } else if(type == 2)
        {
            GameObject[] shuffledSquarePegs = new GameObject[squarePegs.Length];
            squarePegs.CopyTo(shuffledSquarePegs, 0);

            for (int i = 0; i < shuffledSquarePegs.Length; i++)
            {
                int rnd = Random.Range(0, shuffledSquarePegs.Length);
                GameObject tempGO = shuffledSquarePegs[rnd];
                shuffledSquarePegs[rnd] = shuffledSquarePegs[i];
                shuffledSquarePegs[i] = tempGO;
            }


            int amountAdded = 0;
            for (int i = 0; i < shuffledSquarePegs.Length; i++)
            {
                if (!shuffledSquarePegs[i].activeSelf)
                {
                    shuffledSquarePegs[i].SetActive(true);
                    amountAdded++;
                }

                if (amountAdded >= amount) break;
            }
        }
    }
}
