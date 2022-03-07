using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : NetworkedBehaviour
{
    public void OnFireInputChanged()
    {
        Debug.Log("asdf");
     //   CameraShake.instance.ViewportCameraShake(1, 0.5f);
    }

}
