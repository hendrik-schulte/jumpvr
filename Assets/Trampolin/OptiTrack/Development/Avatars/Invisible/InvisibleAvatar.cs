using UnityEngine;
using System.Collections;

//not much to do here - yet..! add functionality here if you like
public class InvisibleAvatar : VirtualAvatar
{
    void Awake()
    {
        AvatarType = AvatarManager.AvatarType.Invisible;
    }
    
    void Start()
    {

    }
    
    void Update()
    {

    }
}
