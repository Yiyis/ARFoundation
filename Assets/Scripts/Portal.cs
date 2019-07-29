﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Portal : MonoBehaviour
{
    public Material[] materials;
    public InstagramAPIIntergration GetInstagramAPIIntergrationScript;

    // Start is called before the first frame update
    void Start()

    {
        foreach(var mat in materials)
            {
                mat.SetInt("stest",(int)CompareFunction.Equal);
            }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerStay(Collider collider)
	{
        if (collider.tag!="MainCamera")
        {
            return;
        }

        Vector3 user_position = Camera.main.transform.position + Camera.main.transform.forward*Camera.main.nearClipPlane;
        Vector3 relativePosition = transform.InverseTransformPoint(user_position);

        //outside
        if(relativePosition.z<0)
        {
            foreach(var mat in materials)
            {
                mat.SetInt("stest",(int)CompareFunction.Equal);
            }
            GetInstagramAPIIntergrationScript.UpdateInstagramPictureMaterials(true);

            
        }

        //inside

        else{
            foreach (var mat in materials)
            {
                mat.SetInt("stest", (int)CompareFunction.NotEqual);
            }
            GetInstagramAPIIntergrationScript.UpdateInstagramPictureMaterials(false);

        }
	}
}
