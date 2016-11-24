using UnityEngine;
using System.Collections;

public class SchleuderScript : MonoBehaviour {

    public GameObject zahnrad;
    public Rigidbody geschoss;
    public GameObject geschossStartPos;
    public float forceScale;
    Quaternion test;
	Vector3 catToPlayer;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

		catToPlayer = Camera.main.transform.position - transform.position;
		catToPlayer.Set(catToPlayer.x, 0, catToPlayer.z);
            
        test = Quaternion.LookRotation(catToPlayer, Vector3.up);
        zahnrad.transform.rotation = Quaternion.RotateTowards(zahnrad.transform.rotation, test, .5f);

        if (Input.GetKeyUp(KeyCode.L))
        {
            shoot();
        }
        if (geschoss.transform.position.y < -1)
        {
            geschoss.isKinematic = true;
            geschoss.transform.position = geschossStartPos.transform.position;
        }
		
	}

	public void shoot(){
        geschoss.isKinematic = false;
        geschoss.AddForce(Vector3.Normalize(Camera.main.transform.position - geschoss.transform.position) * forceScale, ForceMode.Impulse);
        
	}
}
