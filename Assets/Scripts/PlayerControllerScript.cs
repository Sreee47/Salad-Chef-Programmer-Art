using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour {

    //Declaring key controllers for player movement
    public KeyCode moveForward;
    public KeyCode moveBackward;
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode pickItem =KeyCode.F;
    public KeyCode dropItem = KeyCode.G;

    //Declaring initial speed of the player
    public float playerSpeed = 1.0f;

    //declaring and initializing inital time for the player 
    public float timeLeft = 120f;

    //Initial score of the player
    public int score = 0;

    //List to store the vegetables that the player had picked
    public List<GameObject> playerBasket;

    //List of vegetables that the player had dropped on to the chop board
    public List<GameObject> playerChopPlate;

    public List<GameObject> servingPlateItem;

    public GameObject chopBoard;
    public GameObject servingPlate;
    public GameObject trashCan;

    private float defaultSpeed;
    public bool canMove = true;
	// Use this for initialization
	void Start () {
        defaultSpeed = playerSpeed;
        playerBasket = new List<GameObject>(2);
        playerChopPlate = new List<GameObject>(3);
        servingPlateItem = new List<GameObject>(1);

	}
	
	// Update is called once per frame
	void Update () {
        Timer();
        MovePlayer();
    }

    //FixedUpdate is called every fixed frames.
    //This is commonly used for rigidbody movements.
    private void FixedUpdate()
    {
       
    }

    //Timer function to controle the gametime left and to pause the player once the time is up
    void Timer()
    {
        if(timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
        }
        else
        {
            timeLeft = 0;
            canMove = false;
        }
        
    }

    //Move the player based on the key inputs.
    void MovePlayer()
    {
     
        if (canMove)
        {

            //Move forward
            if (Input.GetKey(moveForward))
            {
      
                this.gameObject.GetComponent<Rigidbody2D>().velocity=new Vector2(0,playerSpeed);
            }

            //Move backwards
            else if (Input.GetKey(moveBackward))
            {
                this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -playerSpeed);
            }

            //Move Left
            else if (Input.GetKey(moveLeft))
            {
                this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-playerSpeed, 0);
            }

            //Move right
            else if (Input.GetKey(moveRight)){
                this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(playerSpeed, 0);
            }

            // Stay in the same position if any other key is pressed
            else
            {
                this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }

        }
        else
        {
            return;
        }
    }
    
    //detects amd sents each frame when the another object is in the trigger collider of the gameobject.
   void OnTriggerStay2D(Collider2D collision)
    {

        PickDrop(collision);       
    }

    //checks wether the player has given the input to pick or drop the vegetables
    void PickDrop(Collider2D item)
    {

        if (Input.GetKeyDown(pickItem))
        {
            print("f pressed");
            PickItemFunc(item);
        }
        else if (Input.GetKeyDown(dropItem))
        {
            print("g pressed");
            DropItemFunc(item);
        }
    }

    //To pick particular vegetable from the rack.
    //The pick action is implemented based on the collider interaction of the player and the vegetables.
    void PickItemFunc(Collider2D item)
    {
        if(playerBasket.Count < 2)
        {
            if(item.gameObject.tag == "VEGETABLES")
            {
                print("in pickitem collider");
                GameObject vegItemPrefab = Instantiate(item.gameObject, transform.position, Quaternion.identity);
                playerBasket.Add(vegItemPrefab);
                vegItemPrefab.tag = "Untagged";
                vegItemPrefab.transform.parent = transform;
                Vector3 vegPos = vegItemPrefab.transform.position;
                vegPos.x += playerBasket.Count;
                vegItemPrefab.transform.position = vegPos;

                
            }

        }

    }

    //To drop particular vegetable 
    void DropItemFunc(Collider2D item)
    {
        
      
        if (playerBasket.Count >0)
        {
             // Drop the vegetable to the chopboard
            if (item.gameObject== chopBoard.gameObject)
            {
                
                if (playerChopPlate.Count<3)
                {
                    if (playerBasket.Count > 0)
                    {
                        print("in drop logic");
                        GameObject vegItem = playerBasket[playerBasket.Count - 1];
                        playerChopPlate.Add(vegItem);
                        vegItem.transform.parent = chopBoard.gameObject.transform;
                        playerBasket.Remove(vegItem);
                        StartCoroutine(ChopingTime());
                    }
                }
            }

            //Dispose to the trashcan.
            if(item.gameObject.tag == "TrashCan")
            {
                
                Destroy(playerBasket[playerBasket.Count - 1]);
                playerBasket.Remove(playerBasket[playerBasket.Count-1]);
                
                
            }

            //drop vegetable to the side plates
            if (item.gameObject == servingPlate.gameObject)
            {
                if (servingPlateItem.Count < 1)
                {
                    GameObject vegItem = playerBasket[playerBasket.Count - 1];
                    servingPlateItem.Add(vegItem);
                    vegItem.transform.parent = servingPlate.gameObject.transform;
                    playerBasket.Remove(vegItem);
                }
                     
            }
        }
    }

    // disable the player movement for chopping vegetables
     IEnumerator ChopingTime()
    {
        canMove = false;
        yield return new WaitForSeconds(2);
        canMove = true;
    }
}
