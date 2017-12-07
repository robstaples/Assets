using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnPlay : MonoBehaviour {

    public GameObject objectToShow;

    // Use this for initialization
	void Start ()
    {
        objectToShow.SetActive(true);
    }
}
