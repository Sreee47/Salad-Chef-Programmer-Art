using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerControllerScript : MonoBehaviour {

    //List of the customers
    public GameObject customer;
    public float customerWaitTime;
    public float customerInitialTime = 150.0f;
    public int customerTimerRate = 4;

    public Transform timerScale;
    public int childCount;
    public float scaleSize;
    private List<GameObject> penalizePlayer;

    public bool isCustomerAngry =false;
	// Use this for initialization
	void Start () {

        customer = this.gameObject;
        timerScale = customer.transform.Find("Timebar");
        customerWaitTime = customerInitialTime;
        scaleSize = timerScale.localScale.x;
        customerTimerRate -= customer.transform.Find("DiningPlate").childCount;
        penalizePlayer = new List<GameObject>(2);
	}
	
	// Update is called once per frame
	void Update () {
        StartWaiting();
	}


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
                player.GetComponent<PlayerControllerScript>().playerScore -= 20;
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

    public void AngryCustomer(GameObject player)
    {
        customerTimerRate += 2;
        penalizePlayer.Add(player);
        isCustomerAngry = true;
        print(isCustomerAngry);
    }

   public void CustomerCoolDown(GameObject player)
    {
        customerTimerRate -= 2;
        penalizePlayer.Remove(player);
        isCustomerAngry = false;
        print(isCustomerAngry);
    }


}
