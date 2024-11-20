using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SlingShotStrings : MonoBehaviour
{
    [Header("Rope Settings")]
    [SerializeField] private Transform pointRight; 
    [SerializeField] private Transform pointLeft; 
    [SerializeField] public Transform pointCenter; 

    LineRenderer slingString;

    // Start is called before the first frame update
    void Start()
    {
        slingString = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    public void Update()
    {
        slingString.SetPositions(new Vector3[3] { pointLeft.position, pointCenter.position, pointRight.position});    
    }
}
