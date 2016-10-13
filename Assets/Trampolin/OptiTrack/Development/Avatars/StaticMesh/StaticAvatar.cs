using UnityEngine;
using System.Collections;

//not much functionality needed here... yet! add functionality here if you like
public class StaticAvatar : VirtualAvatar
{
    void Awake()
    {
        AvatarType = AvatarManager.AvatarType.Static;
    }
    
    void Start()
    {

    }
    
    void Update()
    {

    }
}
