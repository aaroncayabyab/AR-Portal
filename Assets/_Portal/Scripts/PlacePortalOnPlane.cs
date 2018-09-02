using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ARSessionOrigin))]
public class PlacePortalOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_placedPortal;

    //Gameobjects
    public GameObject spawner;
    public GameObject setupInstructions;
    public GameObject resetARButton;
    public GameObject swapPortalButton;
    public GameObject ARTestPlane;


    //Bool variable to handle behaviour. Used to trigger behavior once.
    bool isPlaneDetected = false;
    bool isPortalSpawned = false;

    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPortal
    {
        get { return m_placedPortal; }
        set { m_placedPortal = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }

    ARSessionOrigin m_SessionOrigin;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();


    }

    private void Start()
    {
        if (spawner == null)
            spawner = GameObject.Find("Spawner");

        spawner.SetActive(false);
        //Find test plane, disable when not in Unity Editor
        if (ARTestPlane == null)
            ARTestPlane = GameObject.Find("ARTestPlane");

        if(Application.isEditor)
        {
            ARTestPlane.SetActive(true);
        }
        else
        {
            ARTestPlane.SetActive(false);   
        }

        //Toggle UI variables
            setupInstructions.SetActive(true);
            resetARButton.SetActive(false);
            swapPortalButton.SetActive(false);            
    }

    void Update()
    {
        //Only allow user to tap on screen that does not have UI
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && isPlaneDetected == true && isPortalSpawned == false)
            {
                SpawnPortal();
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight * 0.5f));

#if UNITY_EDITOR
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500f, LayerMask.GetMask("ARTestPlane")) && isPortalSpawned == false)
        {
            OnPlaneDetect(hit.point);
        }
#else

        if (m_SessionOrigin.Raycast(ray, s_Hits, TrackableType.PlaneWithinInfinity) && isPortalSpawned == false)
        {
            Pose hitPose = s_Hits[0].pose;

            OnPlaneDetect(hitPose.position);
        }
#endif

    }

    void OnPlaneDetect(Vector3 hitPos)
    {
        //Used to allow portal to be spawned when plane is detected
        isPlaneDetected = true;

        //Toggle UI variables
        setupInstructions.SetActive(false);
        resetARButton.SetActive(true);
        swapPortalButton.SetActive(true);


        //Along plane, spawner transform will continuously update until portal is spawned  
        spawner.SetActive(true);
        spawner.transform.position = hitPos;

        var cameraGroundPos = Camera.main.transform.position;
        cameraGroundPos.y = hitPos.y;

        spawner.transform.LookAt(cameraGroundPos);
    }

    //Disables spawner indicator and spawns Portal at spawner's transform
    void SpawnPortal()
    {
        isPortalSpawned = true;

        if (spawnedObject == null && m_placedPortal != null)
            spawnedObject = Instantiate(m_placedPortal) as GameObject;

        spawnedObject.transform.position = spawner.transform.position;
        spawnedObject.transform.rotation = spawner.transform.rotation;
        spawner.SetActive(false);
        
    }

}
