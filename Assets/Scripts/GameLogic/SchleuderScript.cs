using UnityEngine;
using System.Collections;

public class SchleuderScript : MonoBehaviour {

    public GameObject zahnrad;
    public GameObject geschossParent;
    public GameObject schleuderArm;
    public Animator animator;
    public float forceScale;
    [Range(45, 90)]
    public float anglePlus;
    Quaternion test;
	Vector3 catToPlayer;
    private float elapsedTime;

	// Use this for initialization
	void Start () {
        geschossParent.GetComponent<SteinScript>().setParent(true);
        elapsedTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        elapsedTime += Time.deltaTime;
		catToPlayer = Camera.main.transform.position - transform.position;
		catToPlayer.Set(catToPlayer.x, 0, catToPlayer.z);
            
        test = Quaternion.LookRotation(catToPlayer, Vector3.up);
        zahnrad.transform.rotation = Quaternion.RotateTowards(zahnrad.transform.rotation, test, .7f);

        if (Input.GetKeyUp(KeyCode.L) || elapsedTime >= 5f)
        {
            elapsedTime = 0f;
            animator.SetTrigger("Fire");
            Invoke("shoot", 0.5f);
            //shoot();
        }
        

    }

	public void shoot(){
        GameObject geschoss = Instantiate<GameObject>(geschossParent, schleuderArm.transform);
        geschoss.SetActive(true);
        geschoss.GetComponent<Rigidbody>().isKinematic = false;

        Vector3 shootVec = Quaternion.Euler(anglePlus, 0f, 0f) * zahnrad.transform.forward;

        geschoss.GetComponent<Rigidbody>().AddForce(shootVec * forceScale, ForceMode.Impulse);
        geschoss.transform.parent = null;
        
	}
}
