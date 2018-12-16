using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerControllerScript : MonoBehaviour {

    //List of the customers
    public GameObject customer;

    //For customer timing.
    public float customerWaitTime;

    //initial customer wait time
    public float customerInitialTime = 180.0f;

    //Customer time buffer rate for dynamic timing based in the combinations required for the customer.
    public int customerTimerRate = 4;

    //Timer progress bar for the customer.
    public Transform timerScale;
    public float scaleSize;

    //List for players who served wrong combinations
    private List<GameObject> penalizePlayer;

    public bool isCustomerAngry =false;
	// Use this for initialization
	void Start () {

        customer = this.gameObject;
        timerScale = customer.transform.Find("Timebar");
        customerWaitTime = customerInitialTime;
        scaleSize = timerScale.localScale.x;

        //finiding the combinaiton set of the customer for time calculation.
        customerTimerRate -= customer.transform.Find("DiningPlate").childCount;
        penalizePlayer = new List<GameObject>(2);
	}
	
	// Update is called once per frame
	void Update () {
        StartWaiting();
	}

    //Waiting time for cusotmers
    void StartWaiting()
    {
        customerWaitTime -= (Time.deltaTime*customerTimerRate);

        float newTimerScale = (customerWaitTime / customerInitialTime) * scaleSize;
        timerScale.localScale= new Vector2(newTimerScale,timerScale.transform.localScale.y);

        if (customerWaitTime <= 0)
        {
            
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
            foreach(var player in playerList)
            {
                player.GetComponent<PlayerControllerScript>().playerScore -= 10;
                Destroy(customer);
            }
            if (isCustomerAngry)
            {
                foreach(var player in penalizePlayer)
                {
                    player.GetComponent<PlayerControllerScript>().playerScore -= 10;
                }
            }
        }
    }

    //Angry customer implementation if a customer is served wrong combination.
    public void AngryCustomer(GameObject player)
    {
        customerTimerRate += 2;
        penalizePlayer.Add(player);
        isCustomerAngry = true;
       
    }

    //Cooldown for customer if an angry customer is served within the time limit.
   public void CustomerCoolDown(GameObject player)
    {
        customerTimerRate -= 2;
        penalizePlayer.Remove(player);
        isCustomerAngry = false;
       
    }


}
