using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerControllerScript : MonoBehaviour {

    //Declaring key controllers for player movement
    public KeyCode moveForward;
    public KeyCode moveBackward;
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode pickItem = KeyCode.F;
    public KeyCode dropItem = KeyCode.G;

    //Declaring initial speed of the player
    public float playerSpeed = 3.0f;

    //declaring and initializing inital time for the player 
    public float timeLeft = 120f;

    //Initial score of the player
    public int playerScore = 0;

    //List to store the vegetables that the player had picked
    public List<GameObject> playerBasket;

    //List of vegetables that the player had dropped on to the chop board
    public List<GameObject> playerChopPlate;

    //List of vegetables placed on the extra plate
    public List<GameObject> servingPlateItem;

    //List of customers
    public List<GameObject> customerList;

    public List<GameObject> customerDineList;

    //Declaration for chopboard of the player
    public GameObject chopBoard;

    //Extra plate for the player
    public GameObject servingPlate;

    //Trash can declaration
    public GameObject trashCan;

    //EvaluatorGameobject for pickup
    public GameObject gameController;

    private float defaultSpeed;

    //to determine wether its a combination to be served for the customer.
    public bool combinationReadyToServe;

    //To set players moving condition.
    public bool canMove = true;

    //To enable a player for powerup.
    public bool readyToPowerUp = false;

    //To drop veggies on chop board one by one.
    public bool drop = true;

    //Hud display for the score of the player
    public Text playerScoreText;
    public Text playerTimeLeft;

    // Use this for initialization
    void Start () {
        drop = true;
        gameController = GameObject.Find ("GameController");
        defaultSpeed = playerSpeed;
        playerBasket = new List<GameObject> (2);
        playerChopPlate = new List<GameObject> (3);
        servingPlateItem = new List<GameObject> (1);
        customerList = new List<GameObject> (5);
        customerDineList = new List<GameObject> (3);
        combinationReadyToServe = false;

    }

    // Update is called once per frame
    void Update () {

        Timer ();
        HudDisplay ();
        MovePlayer ();
    }

    //To display the live score and the timings in the HUD.
    void HudDisplay () {
        playerScoreText.text = "Score : " + playerScore.ToString ();
        playerTimeLeft.text = "Time : " + timeLeft.ToString ();
    }
    //Timer function to controle the gametime left and to pause the player once the time is up
    void Timer () {
        if (timeLeft > 0) {
            timeLeft -= Time.deltaTime;
        } else {
            timeLeft = 0;
            canMove = false;
        }

    }

    //Move the player based on the key inputs.
    void MovePlayer () {

        if (canMove) {

            //Move forward
            if (Input.GetKey (moveForward)) {

                gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, playerSpeed);
            }

            //Move backwards
            else if (Input.GetKey (moveBackward)) {
                gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, -playerSpeed);
            }

            //Move Left
            else if (Input.GetKey (moveLeft)) {
                gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (-playerSpeed, 0);
            }

            //Move right
            else if (Input.GetKey (moveRight)) {
                gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (playerSpeed, 0);
            }

            // Stay in the same position if any other key is pressed
            else {
                gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
            }

        } else {
            gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);

        }
    }

    //Sent when another object enters a trigger collider attached to this object for 2D physics
    //Here its used to collect pickups.
    private void OnTriggerEnter2D (Collider2D collision) {

        if (collision.gameObject.tag == "Powerup" && readyToPowerUp) {

            AddPower (collision.gameObject);

        }

    }

    //detects amd sents each frame when the another object is in the trigger collider of the gameobject.
    void OnTriggerStay2D (Collider2D collision) {
        PickDrop (collision);
    }

    //checks wether the player has given the input to pick or drop the vegetables
    void PickDrop (Collider2D item) {

        if (Input.GetKeyDown (pickItem)) {

            PickItemFunc (item);
        } else if (Input.GetKeyDown (dropItem)) {
            DropItemFunc (item);

        }
    }

    //To pick particular vegetable from the rack.
    //The pick action is implemented based on the collider interaction of the player and the vegetables.
    void PickItemFunc (Collider2D item) {
        if (playerBasket.Count < 2) {
            if (item.gameObject.tag == "VEGETABLES") {

                GameObject vegItemPrefab = Instantiate (item.gameObject, transform.position, Quaternion.identity);
                playerBasket.Add (vegItemPrefab);
                vegItemPrefab.tag = "Untagged";
                vegItemPrefab.transform.parent = transform;
                Vector3 vegPos = vegItemPrefab.transform.position;
                vegPos.x += playerBasket.Count;
                vegItemPrefab.transform.position = vegPos;

            }

            //Picking the vegetable placed over the extra plate
            if (item.gameObject == servingPlate && servingPlateItem.Count == 1) {

                GameObject vegItemPrefab = servingPlateItem[0];
                servingPlateItem.Remove (vegItemPrefab);
                playerBasket.Add (vegItemPrefab);
                vegItemPrefab.transform.parent = transform;

            }

            //Picking particular combination from the chopBoar for the customer serving.
            if (item.gameObject == chopBoard && playerBasket.Count == 0 && playerChopPlate.Count != 0) {

                foreach (var vegItem in playerChopPlate) {

                    playerBasket.Add (vegItem);
                    vegItem.transform.parent = transform;

                }
                playerChopPlate.Clear ();
                combinationReadyToServe = true;
            }

        }

    }

    //To drop particular vegetable & combination.
    void DropItemFunc (Collider2D item) {
        if (playerBasket.Count > 0) {
            // Drop the vegetable to the chopboard
            if (item.gameObject == chopBoard.gameObject) {

                if (playerChopPlate.Count < 3) {
                    if (drop) {
                        drop = false;

                        GameObject vegItem = playerBasket[0];
                        playerChopPlate.Add (vegItem);
                        vegItem.transform.parent = chopBoard.gameObject.transform;
                        playerBasket.Remove (vegItem);
                        StartCoroutine (ChopingTime ());
                    }

                }
            }

            //Dispose to the trashcan.
            if (item.gameObject.tag == "TrashCan") {

                Destroy (playerBasket[playerBasket.Count - 1]);
                playerBasket.Remove (playerBasket[playerBasket.Count - 1]);
                combinationReadyToServe = false;
                playerScore -= 10;

            }

            //drop vegetable to the side plates
            if (item.gameObject == servingPlate.gameObject) {

                if (servingPlateItem.Count < 1) {
                    GameObject vegItem = playerBasket[playerBasket.Count - 1];
                    servingPlateItem.Add (vegItem);
                    vegItem.transform.parent = servingPlate.gameObject.transform;
                    playerBasket.Remove (vegItem);
                }

            }

            //Dropping combination for the customer
            if (item.gameObject.tag == "DiningPlate" && combinationReadyToServe) {
                List<Transform> childVeggies = new List<Transform> (3);
                foreach (Transform vegChilds in item.gameObject.transform) {

                    childVeggies.Add (vegChilds);
                }
                foreach (var vegItem in playerBasket) {
                    customerDineList.Add (vegItem);
                    vegItem.transform.parent = item.gameObject.transform;

                }
                playerBasket.Clear ();
                combinationReadyToServe = false;
                CalculatePoints (childVeggies, item.transform.parent);
            }
        }
    }

    //Verifying combination and Calculating points after the combination is given to the customer
    void CalculatePoints (List<Transform> childVegies, Transform customer) {
        bool addPoint = true;

        if (customerDineList.Count != childVegies.Count) {
            addPoint = false;
            ClearDiningTable ();
        } else {
            foreach (Transform item in childVegies) {

                foreach (var dineItem in customerDineList) {
                    if (dineItem.name.Contains (item.name)) {
                        addPoint = true;
                        break;

                    } else {
                        addPoint = false;
                    }
                }
            }
        }

        if (addPoint) {
            playerScore += 20;
            customer.gameObject.GetComponent<CustomerControllerScript> ().customerWaitTime += 20;
            if (customer.gameObject.GetComponent<CustomerControllerScript> ().isCustomerAngry) {
                customer.gameObject.GetComponent<CustomerControllerScript> ().CustomerCoolDown (gameObject);
            }

            CheckPowerUpEligibility (customer.gameObject);
        } else {
            playerScore -= 10;
            customer.gameObject.GetComponent<CustomerControllerScript> ().AngryCustomer (gameObject);
        }
        ClearDiningTable ();
    }

    //To clear the delivered salad
    void ClearDiningTable () {
        foreach (var dineItem in customerDineList) {
            Destroy (dineItem);
        }

        customerDineList.Clear ();
    }

    // Check wether the player is eligible for powerup spawn.
    //Player should have served proper salad within 70% time limit of the customer waiting time. 
    void CheckPowerUpEligibility (GameObject customer) {
        var totalTime = customer.GetComponent<CustomerControllerScript> ().customerInitialTime;
        var eligibleTime = totalTime - (totalTime * 0.7);
        var currentTime = customer.GetComponent<CustomerControllerScript> ().customerWaitTime;
        if (currentTime >= eligibleTime) {
            readyToPowerUp = true;
            gameController.GetComponent<GameEvaluatorScript> ().SpawnPickUp ();

            //To remove ability to pick powerups after some time
            StartCoroutine (RemovePowerUpAbility ());
        }
    }

    // Add power to the player based on the pickup the player has got.
    void AddPower (GameObject powerUp) {
        if (powerUp.name.Contains ("AddScore")) {
            playerScore += 50;
        }
        if (powerUp.name.Contains ("timer")) {
            timeLeft += 50;
        }
        if (powerUp.name.Contains ("Speedup")) {
            StartCoroutine (BoltUp ());
        }
        Destroy (powerUp);
        readyToPowerUp = false;

    }

    // Speedup the player for 10 seconds. 
    IEnumerator BoltUp () {
        playerSpeed += 5;
        yield return new WaitForSeconds (10);
        playerSpeed = defaultSpeed;
    }

    // disable the player movement for chopping vegetables
    IEnumerator ChopingTime () {
        canMove = false;
        yield return new WaitForSeconds (2);
        canMove = true;
        drop = true;
    }

    //Disabling the player power to accept powerups
    IEnumerator RemovePowerUpAbility () {
        yield return new WaitForSeconds (10);
        readyToPowerUp = false;
    }
}