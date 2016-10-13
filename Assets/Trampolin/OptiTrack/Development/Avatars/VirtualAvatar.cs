using UnityEngine;
using System.Collections;

public abstract class VirtualAvatar : MonoBehaviour
{
    //every avatar has a specified AvatarType
    public AvatarManager.AvatarType AvatarType {get; protected set;}
}
