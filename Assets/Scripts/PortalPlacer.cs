﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;


public class PortalPlacer : MonoBehaviour
{
    ARRaycastManager m_ARRaycastManager;

    List<ARRaycastHit> raycast_hits = new List<ARRaycastHit>();

    //This is the prefab to be instantiated
    public GameObject portalPrefab;

    //This is the gameobject reference instantiated after a successsful raycast intersection with a plane
    private GameObject spawnedPortal;

    public static event Action onPlaceObject;


    private void Awake()
	{
        m_ARRaycastManager = GetComponent<ARRaycastManager>();
	}
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount>0)
        {
            //Getting touch inputs
            Touch touch = Input.GetTouch(0);

            if (m_ARRaycastManager.Raycast(touch.position,raycast_hits,UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                //Getting the pose of the hit
                Pose pose = raycast_hits[0].pose;

                if(spawnedPortal==null)
                {
                    spawnedPortal = Instantiate(portalPrefab, pose.position, Quaternion.Euler(0, 0, 0));
                    var rotationOfPortal = spawnedPortal.transform.rotation.eulerAngles;
                    spawnedPortal.transform.eulerAngles = new Vector3(rotationOfPortal.x,Camera.main.transform.rotation.eulerAngles.y,rotationOfPortal.z);
                    
                }else
                {
                    spawnedPortal.transform.position = pose.position;
                    var rotationOfPortal = spawnedPortal.transform.rotation.eulerAngles;
                    spawnedPortal.transform.eulerAngles = new Vector3(rotationOfPortal.x,Camera.main.transform.rotation.eulerAngles.y,rotationOfPortal.z);

                }
                onPlaceObject();

            }
          
        }
    }
}

