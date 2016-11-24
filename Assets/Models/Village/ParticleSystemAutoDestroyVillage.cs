using UnityEngine;
using System.Collections;

public class ParticleSystemAutoDestroyVillage : MonoBehaviour 
{
	private ParticleSystem ps;


	public void Start() 
	{
		
	}

	public void Update() 
	{
		if(Input.GetKeyDown (KeyCode.Q))
		{
			Destroy (gameObject, GetComponent<ParticleSystem> ().duration);
		}
	}
}
