using UnityEngine;
using System.Collections;

public class SchleuderScript : MonoBehaviour {

    [Header("Objects")]
    public GameObject zahnrad;
    public GameObject geschossParent;
    public GameObject schleuderArm;
    public Animator animator;
    public GameObject trollKopf;
    public GameObject katapult;

    [Header("Variables for shooting")]
    public float maxTimeUntilShoot;
    public float forceScale;
    [Range(45, 90)]
    public float anglePlus;
    public AnimationCurve shootProbability;

    Quaternion rotation;
	Vector3 catToPlayer;
    private float elapsedTime;
    private bool canShoot;

    // Use this for initialization
    void Start () {
        geschossParent.GetComponent<SteinScript>().setParent(true);
        elapsedTime = 0f;
        canShoot = true;
	}
	
	// Update is called once per frame
	void Update () {
        elapsedTime += Time.deltaTime;
        

        if (elapsedTime >= maxTimeUntilShoot)
        {
            elapsedTime = maxTimeUntilShoot;
        }

		catToPlayer = trollKopf.transform.position - transform.position;
		catToPlayer.Set(catToPlayer.x, 0, catToPlayer.z);
            
        rotation = Quaternion.LookRotation(catToPlayer, Vector3.up);
        zahnrad.transform.rotation = Quaternion.RotateTowards(zahnrad.transform.rotation, rotation, .7f);

        if (canShoot && Mathf.Abs(Vector3.Angle(zahnrad.transform.forward, catToPlayer)) < 20f)
        {
            print(shootProbability.Evaluate(elapsedTime));
            if (Random.value <= shootProbability.Evaluate(elapsedTime))
            {
                canShoot = false;
                elapsedTime = 0f;
                animator.SetTrigger("Fire");
                Invoke("shoot", 0.3f);
                //shoot();
            }
        }else if (Input.GetKeyUp(KeyCode.L))
        {
            canShoot = false;
            elapsedTime = 0f;
            animator.SetTrigger("Fire");
            Invoke("shoot", 0.3f);
        }

    }

	public void shoot(){

        
        GameObject geschoss = Instantiate<GameObject>(geschossParent, schleuderArm.transform);
        geschoss.SetActive(true);
        geschoss.GetComponent<Rigidbody>().isKinematic = false;
        Vector3 shootVec = Quaternion.AngleAxis(-anglePlus, katapult.transform.right) * katapult.transform.forward;

        geschoss.GetComponent<Rigidbody>().AddForce(shootVec * forceScale, ForceMode.Impulse);
        geschoss.transform.parent = null;

            
        Invoke("onShootingFinished", 2.25f); //Die Shoot-Animation des Katapults dauert 2.25 Sekunden
    }

    private void onShootingFinished()
    {
        canShoot = true;
    }
}
