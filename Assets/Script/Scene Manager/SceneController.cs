using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public string sceneName;
    public string sceneType;
    protected MasterController masterController;

    public virtual void Initialize(MasterController controller)
    {
        masterController = controller;
    }
}
