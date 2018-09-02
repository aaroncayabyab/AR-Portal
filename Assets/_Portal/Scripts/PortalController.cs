using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalController : MonoBehaviour {

    public Transform device;


    //bool for checking if the device is not in the same direction as it was
    bool isOutside;
    //bool for knowing that on the next change of state, what to set the stencil test
    bool isInside;

    //This bool is on while device colliding, done so we ensure the shaders are being updated before render frames
    //Avoids flickering
    bool isColliding;

    private void Awake()
    {
        device = Camera.main.transform;
    }

    void Start()
    {
        //start outside other world
        SetMaterials(false);
    }

    void SetMaterials(bool fullRender)
    {
        var stencilTest = fullRender ? CompareFunction.NotEqual : CompareFunction.Equal;
        Shader.SetGlobalInt("_StencilRef", (int)stencilTest);
    }

    bool GetIsOutside()
    {
        Vector3 worldPos = device.position + device.forward * Camera.main.nearClipPlane;

        Vector3 pos = transform.InverseTransformPoint(worldPos);
        return pos.z >= 0 ? true : false;
    }


    //This technique registeres if the device has hit the portal, flipping the bool

    void OnTriggerEnter(Collider other)
    {
        if (other.transform != device)
            return;

        //Important to do this for if the user re-enters the portal from the same side
        isOutside = GetIsOutside();
        isColliding = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform != device)
            return;



        isColliding = false;
    }


    /*If there has been a change in the relative position of the device to the portal, flip the
     *Stencil Test
     */

    void WhileCameraColliding()
    {
        if (!isColliding)
            return;
        bool isInFront = GetIsOutside();
        if ((isInFront && !isOutside) || (isOutside && !isInFront))
        {
            isInside = !isInside;
            SetMaterials(isInside);
        }
        isOutside = isInFront;
    }

    void OnDestroy()
    {
        //ensure geometry renders in the editor
        SetMaterials(true);
    }


    void Update()
    {
        WhileCameraColliding();
    }
}
