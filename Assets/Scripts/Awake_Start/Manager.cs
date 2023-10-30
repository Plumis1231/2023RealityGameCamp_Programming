using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    public static Manager InstanceUnsafe;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        InstanceUnsafe = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerEnter()
    {
        print("Player Enter");
    }
}