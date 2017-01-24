using UnityEngine;
using System.Collections.Generic;

public class EnityMng : MonoBehaviour {

    protected Dictionary<ulong, Enity> enityDict = new Dictionary<ulong, Enity>();

    protected ulong currentEnityId = 0;

    protected ulong GetUniqueEnityId()
    {
        currentEnityId++;
        return currentEnityId;
    }
}
