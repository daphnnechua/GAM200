using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public string sceneName;
    protected MasterController masterController;

    public virtual void Initialize(MasterController controller)
    {
        masterController = controller;
    }
}
