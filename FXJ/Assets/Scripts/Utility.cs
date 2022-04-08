using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static void SetLayerRecursively(GameObject root, string layer)
    {
        root.layer = LayerMask.NameToLayer(layer);
        Transform[] allChildren = root.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren) {
            child.gameObject.layer = LayerMask.NameToLayer(layer);
        }
    }

}
