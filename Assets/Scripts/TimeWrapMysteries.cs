using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWrapMysteries : MonoBehaviour
{
    public GameObject TreeNow;

    public void OnTreeDestroyedBefore()
    {
        TreeNow.SetActive(false);
    }
}
