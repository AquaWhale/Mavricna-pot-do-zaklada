using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainCharacter : MonoBehaviour
{
    //time at the beggining of the game after each restart or at absolute beggining
    private float startTime = 0.0f;

    [SerializeField] 
    private float jumpForce = 4;


    [SerializeField] 
    private Animator animator;
    [SerializeField] 
    private Rigidbody rigidBody;

    private float currentV = 0;
    //hitrost za premik levo in desno ko je ta zaznan
    private float currentH = 0;

    private readonly float interpolation = 10;

    private bool wasGrounded;

    private float jumpTimeStamp = 0;
    private float minJumpInterval = 0.25f;

    private bool isGrounded;
    private List<Collider> collisions = new List<Collider>();
  
    private int previousLane = 1;
    private float h = 0;
    private int lane = 1; //0 - levo, 1 - sredina, 2 - desno
    private float[] positionX = { -3.4f, 0.0f, 3.4f };

    //da vemo v katero smer se premaknemo
    private string previousKeyPress = "";

    //as long as the starting animation is going on, player cannot move
    private float animationDuration = 3.0f;

    //holds canvas object for projecting power up pickups
    private GameObject canvas;
    //this is meant for UI power up icon maintanance
    private float distanceBetwenUIIcons = -63.0f;
    private float nextPositionXOffsetOfUIIcon = 0.0f;
    private List<GameObject> activeIconInstances = new List<GameObject>();
    private int numOfActiveIcons = 0;

    //magnet instance icon power and prefab
    public GameObject magnetActiveIconPrefab;
    private GameObject magnetActiveIconInstance = null;
    //2x instance icon power and prefab
    public GameObject twoXActiveIconPrefab;
    private GameObject twoXActiveIconInstance = null;
    //Reverse keys icon active and prefab
    public GameObject reverseKeysActiveIconPrefab;
    private GameObject reverseKeysActiveIconInstance = null;
    //Rocket power icon active and prefab
    public GameObject rocketPowerActiveIconPrefab;
    private GameObject rocketPowerActiveIconInstance = null;

    public GameOverMenu gameOverMenu;

    //Vse za pobiranje kovančkov
    [SerializeField]
    private GameObject coinPickupEffect;

    private float coinEffectDuration = 1.0f;

    public Text coinScore;
    public Text highScore;

    public GameObject previousHighScore;
    public GameObject newHighScore;

    //Izogibanje mostom
    private bool bridgeOnRight = false;
    private bool bridgeOnMiddle = false;
    private bool bridgeOnLeft = false;



    //collect collectibles in naredi določeno akcijo
    void OnTriggerEnter(Collider other)
    {
        //Kovančki
        //ce se player zaleti vanj
        if (other.gameObject.CompareTag("Coin"))
        {
            other.gameObject.SetActive(false);
            this.CollectCoin();

        }
        //magnet coin power up
        else if (other.gameObject.CompareTag("Magnet"))
        {
            other.gameObject.SetActive(false);
            EnableMagnet();

        }
        //2x power up
        else if (other.gameObject.CompareTag("2X"))
        {
            other.gameObject.SetActive(false);
            Enable2X();
            
        }
        //Reverse keys
        else if (other.gameObject.CompareTag("Reverse"))
        {
            other.gameObject.SetActive(false);
            SoundManager.PlaySound("reversePickup");
            //Show on canvas that reverse keys is active -> ce instanca ze obstaja je ne kreiramo se enkrat
            if (reverseKeysActiveIconInstance == null)
            {
                GameState.enableReverseKeys();
                reverseKeysActiveIconInstance = Instantiate(reverseKeysActiveIconPrefab) as GameObject;
                reverseKeysActiveIconInstance.transform.SetParent(canvas.transform, false);
                //set x position of icon power up where it is supposed to go
                reverseKeysActiveIconInstance.transform.localPosition += new Vector3(nextPositionXOffsetOfUIIcon, 0, 0);
                //posodobi pozicijo naslednjega power up icona
                activeIconInstances.Add(reverseKeysActiveIconInstance);
                nextPositionXOffsetOfUIIcon += distanceBetwenUIIcons;

                //prikazemo trenutno stanje reverse keys time counterja
                //vzamemo 2. gameobject znotraj instance reversekeys UI
                int reverseKeysActiveIconIndex = activeIconInstances.IndexOf(reverseKeysActiveIconInstance);
                Text reverseKeysTimeCounter = activeIconInstances[reverseKeysActiveIconIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
                reverseKeysTimeCounter.text = GameState.totalReverseKeysTime.ToString("F0");
            }
            else
            {
                this.DisableReverseKeys();
            }
        }
        else if (other.gameObject.CompareTag("Obsticle"))
        {
            GameState.deadByCollision = true;
            SoundManager.PlaySound("crash");
        }
        else if (other.gameObject.CompareTag("Bodice"))
        {
            GameState.deadByCollision = true;
            SoundManager.PlaySound("scream");
        }
        else if (other.gameObject.CompareTag("Rocket"))
        {
            other.gameObject.SetActive(false);
            EnableRocket();

        }
        else if (other.gameObject.CompareTag("QuestionMark"))
        {
            other.gameObject.SetActive(false);
            RandomPowerUp();
        }
        //ce smo preckali desni most
        else if (other.CompareTag("RightCrossed"))
        {
            //Debug.Log("Right Crossed    " + Time.time);
            bridgeOnRight = true;
        }
        else if (other.CompareTag("RightLeft"))
        {
            //Debug.Log("Right left   " + Time.time);
            bridgeOnRight = false;
        }
        //ce smo preckali middle most
        else if (other.CompareTag("MiddleCrossed"))
        {
            //Debug.Log("Middle crossed   " + Time.time);
            bridgeOnMiddle = true;
        }
        else if (other.CompareTag("MiddleLeft"))
        {
            //Debug.Log("Middle left   " + Time.time);
            bridgeOnMiddle = false;
        }
        else if (other.CompareTag("LeftCrossed"))
        {
            //Debug.Log("Left crossed   " + Time.time);
            bridgeOnLeft = true;
        }
        else if (other.CompareTag("LeftLeft"))
        {
            //Debug.Log("Left left   " + Time.time);
            bridgeOnLeft = false;
        }
    }

    private void Start()
    {
        canvas = GameObject.Find("Canvas");
        startTime = Time.time;
        coinScore.text = GameState.coinScore.ToString("0000000");
        highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        previousHighScore.SetActive(true);
        newHighScore.SetActive(false);
    }

    void Update()
    {

        animator.SetBool("Grounded", isGrounded);

        if (Time.time < startTime + animationDuration)
        {
            //Če smo še vedno v animaciji samo teče naprej
            Run();
        }
        else if (!GameState.deadByCollision && !GameState.deadByExplosion)
        {
            //če nismo več v animaciji vključimo možnost premikanja levo in desno in ostale funkcionalnosti igre
            //1.) FUNKCIONALNOST MAGNETA IN PREMIK BUSOV, ČE SO V OKOLICI
            if (GameState.hasMagnetPower())
            {
                //najde objekte v magnetRadius okoli sebe
                Collider[] colliders;
                colliders = Physics.OverlapSphere(this.transform.position, GameState.magnetRadius);

                foreach (Collider collider in colliders)
                {
                    if (collider.CompareTag("Coin"))
                    {
                        Transform coinTransform = collider.transform;
                        Vector3 goTo = new Vector3(this.transform.position.x, this.transform.position.y+1.5f, this.transform.position.z+6.0f*Time.deltaTime);
                        var diffVector = (coinTransform.position - goTo);
                        var distanceFromPlayer = diffVector.sqrMagnitude;
                        if (distanceFromPlayer < 2f)
                        {
                            collider.gameObject.SetActive(false);
                            this.CollectCoin();
                        }
                        //premaknemo kovanček proti playerju z enako hitrostjo kot se gibajo platforme
                        coinTransform.position = Vector3.Lerp(coinTransform.position, goTo, (GameState.moveSpeedPlatform) * Time.deltaTime);
                    }
                }
                GameState.totalMagnetTime -= Time.deltaTime;
                //posodobimo trenutno stanje magnet time counterja
                //vzamemo 2. gameobject znotraj instance magnet UI
                int magnetActiveIconIndex = activeIconInstances.IndexOf(magnetActiveIconInstance);
                Text magnetTimeCounter = activeIconInstances[magnetActiveIconIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
                magnetTimeCounter.text = GameState.totalMagnetTime.ToString("F0");
                if (GameState.totalMagnetTime <= 0.0f)
                {
                    GameState.disableMagnet();
                    //destrojamo instanco magnet icona, ker je power potekel
                    Destroy(activeIconInstances[magnetActiveIconIndex]);
                    activeIconInstances.RemoveAt(magnetActiveIconIndex);
                    magnetActiveIconInstance = null;

                    //posodobi pozicijo naslednjega powerupa (zamakni nazaj za razmik med dvema)
                    nextPositionXOffsetOfUIIcon -= distanceBetwenUIIcons;

                    //Posodobimo v seznamu vseh icon power upov position power upov za pravkar deletanim da se pomaknejo naprej
                    for (int i = magnetActiveIconIndex; i < activeIconInstances.Count; i++)
                    {
                        activeIconInstances[i].transform.localPosition -= new Vector3(distanceBetwenUIIcons, 0, 0);
                    }
                }
            }

            //2.) FUNKCIONALNOST PREMIKANJA
            //ce pritisnemo na A
            if (Input.GetKeyDown(KeyCode.A))
            {

                if (GameState.hasReverseKeys())
                {
                    //code for D key, because A is now D
                    if ((lane + 1) <= 2)
                    {
                        //pogledamo ce na desni ni slucajno middle ali right bridge
                        if (bridgeOnMiddle && (lane + 1) == 1 && transform.position.y < 3.1f) { }
                        else if (bridgeOnRight && (lane + 1) == 2 && transform.position.y < 3.1f) { }
                        else
                        {
                            h = 1;
                            if (!previousKeyPress.Equals("D"))
                            {
                                previousLane = lane;
                            }
                            lane += 1;
                            previousKeyPress = "D";
                            SoundManager.PlaySound("move");
                        }
                    }
                }
                else
                {
                    if ((lane - 1) >= 0)
                    {
                        if (bridgeOnMiddle && (lane - 1) == 1 && transform.position.y < 3.1f) { }
                        else if (bridgeOnLeft && (lane - 1) == 0 && transform.position.y < 3.1f) { }
                        else
                        {
                            h = -1;
                            if (!previousKeyPress.Equals("A"))
                            {
                                previousLane = lane;
                            }
                            lane -= 1;
                            previousKeyPress = "A";
                            SoundManager.PlaySound("move");
                        }
                    }
                }
            }
            //ce pritisnemo D
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (GameState.hasReverseKeys())
                {
                    if ((lane - 1) >= 0)
                    {
                        if (bridgeOnMiddle && (lane - 1) == 1 && transform.position.y < 3.1f) { }
                        else if (bridgeOnLeft && (lane - 1) == 0 && transform.position.y < 3.1f) { }
                        else
                        {
                            h = -1;
                            if (!previousKeyPress.Equals("A"))
                            {
                                previousLane = lane;
                            }
                            lane -= 1;
                            previousKeyPress = "A";
                            SoundManager.PlaySound("move");
                        }
                    }
                }
                else
                {

                    if ((lane + 1) <= 2)
                    {
                        if (bridgeOnMiddle && (lane + 1) == 1 && transform.position.y < 3.1f) { }
                        else if (bridgeOnRight && (lane + 1) == 2 && transform.position.y < 3.1f) { }
                        else
                        {
                            h = 1;
                            if (!previousKeyPress.Equals("D"))
                            {
                                previousLane = lane;
                            }
                            lane += 1;
                            previousKeyPress = "D";
                            SoundManager.PlaySound("move");
                        }
                    }
                }

            }

            //3.) FUNKCIONALNOST 2X POWER UPA (kovanček je vreden dvakrat več)
            if (GameState.has2XPower())
            {
                GameState.total2XPowerTime -= Time.deltaTime;
                int twoXActiveIconIndex = activeIconInstances.IndexOf(twoXActiveIconInstance);
                //Posodobimo stanje 2x time counterja
                Text twoXTimeCounter = activeIconInstances[twoXActiveIconIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
                twoXTimeCounter.text = GameState.total2XPowerTime.ToString("F0");
                if (GameState.total2XPowerTime <= 0.0f)
                {
                    GameState.disable2XPower();
                    //destrojamo instanco 2x icona, ker je power potekel
                    Destroy(activeIconInstances[twoXActiveIconIndex]);
                    activeIconInstances.RemoveAt(twoXActiveIconIndex);
                    twoXActiveIconInstance = null;
                    //posodobi pozicijo naslednjega powerupa (zamakni nazaj za razmik med dvema)
                    nextPositionXOffsetOfUIIcon -= distanceBetwenUIIcons;

                    //Posodobimo v seznamu vseh icon power upov position power upov za pravkar deletanim da se pomaknejo naprej
                    for (int i = twoXActiveIconIndex; i < activeIconInstances.Count; i++)
                    {
                        activeIconInstances[i].transform.localPosition -= new Vector3(distanceBetwenUIIcons, 0, 0);
                    }
                }
            }

            //4. FUNCIONALNOST REVERSE KEYS (zamenja A in D)
            if (GameState.hasReverseKeys())
            {
                GameState.totalReverseKeysTime -= Time.deltaTime;
                int reverseKeysActiveIconIndex = activeIconInstances.IndexOf(reverseKeysActiveIconInstance);
                //Posodobimo stanje 2x time counterja
                Text reverseKeysTimeCounter = activeIconInstances[reverseKeysActiveIconIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
                reverseKeysTimeCounter.text = GameState.totalReverseKeysTime.ToString("F0");
                if (GameState.totalReverseKeysTime <= 0.0f)
                {
                    this.DisableReverseKeys();
                }
            }

            //5. FUNKCIONALNOST RACKET POWER (doda vecji jump plus pospešek po z vsakič ko skoči)
            if (GameState.hasRocketPower())
            {
                GameState.totalRocketPowerTime -= Time.deltaTime;
                int racketPowerActiveIconIndex = activeIconInstances.IndexOf(rocketPowerActiveIconInstance);
                //Posodobimo stanje 2x time counterja
                Text racketPowerTimeCounter = activeIconInstances[racketPowerActiveIconIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
                racketPowerTimeCounter.text = GameState.totalRocketPowerTime.ToString("F0");
                if (GameState.totalRocketPowerTime <= 0.0f)
                {
                    GameState.disableRocketPower();
                    //destrojamo instanco 2x icona, ker je power potekel
                    Destroy(activeIconInstances[racketPowerActiveIconIndex]);
                    activeIconInstances.RemoveAt(racketPowerActiveIconIndex);
                    rocketPowerActiveIconInstance = null;
                    //posodobi pozicijo naslednjega powerupa (zamakni nazaj za razmik med dvema)
                    nextPositionXOffsetOfUIIcon -= distanceBetwenUIIcons;

                    //Posodobimo v seznamu vseh icon power upov position power upov za pravkar deletanim da se pomaknejo naprej
                    for (int i = racketPowerActiveIconIndex; i < activeIconInstances.Count; i++)
                    {
                        activeIconInstances[i].transform.localPosition -= new Vector3(distanceBetwenUIIcons, 0, 0);
                    }
                }
            }

            //najde objekte v magnetRadius okoli sebe
            Collider[] objectsAround;
            objectsAround = Physics.OverlapSphere(this.transform.position, 40.0f);

            foreach (Collider o in objectsAround)
            {
                //Premik busov okoli sebe
                if (o.CompareTag("Bus"))
                {
                    Transform busTransform = o.transform.parent.gameObject.transform;
                    //premaknemo bus naprej z dodatno hitrostjo
                    busTransform.position += new Vector3(transform.forward.x, transform.forward.y, transform.forward.z * (-1.0f)) * 1.0f * 6.0f * Time.deltaTime;
                }

            }
            //NA KONCU SE ŠE PREMAKNEMO -> oziroma samo animiramo tek
            Run();
        }
        else
        {
            gameOverMenu.showEndMenu();
            Die();
        }


        wasGrounded = isGrounded;
    }

    private void Run()
    {
        float v = 1;
        
        //current V je samo zato, da vemo kako hitro naj poteka animacija teka
        currentV = Mathf.Lerp(currentV, v, Time.deltaTime * interpolation);
        currentH = Mathf.Lerp(currentH, h, Time.deltaTime * interpolation);

        transform.position += transform.right * currentH * GameState.leftRightSpeed * Time.deltaTime;

        var pos = transform.position;

        //za premikanje levo in desno
        pos.x = Mathf.Clamp(transform.position.x, (previousLane < lane) ? positionX[previousLane] : positionX[lane], (previousLane > lane) ? positionX[previousLane] : positionX[lane]);
        transform.position = pos;

        animator.SetFloat("MoveSpeed", currentV);

        if (!(Time.time < startTime + animationDuration))
        {
            JumpingAndLanding();
        }
    }

    private void Die()
    {
        GameState.moveSpeedPlatform = 0.0f;
        animator.SetFloat("MoveSpeed", 0.0f);
        if (GameState.deadByCollision)
        {
            FallOnFloor();
        }
        else if (GameState.deadByExplosion)
        {
            Explode();
            
        }
    }

    private void FallOnFloor()
    {
        Vector3 vector = new Vector3(Vector3.forward.x, Vector3.forward.y, Vector3.forward.z * -1.0f);
        //ce se ni za -90 zarotiran ga zarotiramo
        if (transform.rotation.x > -0.70f)
        {
            transform.RotateAround(this.transform.position, new Vector3(-1.0f, 0.0f, 0.0f), 120 * Time.deltaTime);
            foreach (Transform child in transform)
            {
                child.transform.RotateAround(this.transform.position, new Vector3(1.0f, 0.0f, 0.0f), 120 * Time.deltaTime);
            }
            animator.SetFloat("MoveSpeed", 0.0f);
        }

    }

    private void Explode()
    {
        //find camera object
        foreach (Transform child in transform)
        {
            if (child.gameObject.CompareTag("MainCamera"))
            {
                child.parent = null;
            }
        }
        this.gameObject.SetActive(false);
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - jumpTimeStamp) >= minJumpInterval;

        //ce se ni jumpal potem dodamo force
        if (jumpCooldownOver && isGrounded && Input.GetKey(KeyCode.Space))
        {
            jumpTimeStamp = Time.time;
            if (GameState.hasRocketPower())
            {
                rigidBody.AddForce(Vector3.up * (28f), ForceMode.Impulse);
                GameState.hasJumpedWithRocket = true;
                GameState.hasJumpedWithRocketStartTime = Time.time;
                Vector3 vel = rigidBody.velocity;
                vel.y -= 30.0f * Time.deltaTime;
                rigidBody.velocity = vel;
                SoundManager.PlaySound("rocketJump");
            }
            else
            {
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
                SoundManager.PlaySound("jump");
            }
            
        }

        if (!wasGrounded && isGrounded)
        {
            animator.SetTrigger("Land");
        }

        if (!isGrounded && wasGrounded)
        {
            animator.SetTrigger("Jump");
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Bus"))
        {
            if (this.transform.position.y < 4.2f)
            {
                GameState.deadByCollision = true;
                SoundManager.PlaySound("crash");
            }
        }
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!collisions.Contains(collision.collider))
                {
                    collisions.Add(collision.collider);
                }
                isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            isGrounded = true;
            if (!collisions.Contains(collision.collider))
            {
                collisions.Add(collision.collider);
            }
        }
        else
        {
            if (collisions.Contains(collision.collider))
            {
                collisions.Remove(collision.collider);
            }
            if (collisions.Count == 0) { isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collisions.Contains(collision.collider))
        {
            collisions.Remove(collision.collider);
        }
        if (collisions.Count == 0) { isGrounded = false; }
    }

    private void RandomPowerUp()
    {
        //vrne 1 2 ali 3 -> 1 = magnet, 2 = 2x, 3 = jump
        int randomPowerUp = Random.Range(1, 4);
        switch (randomPowerUp)
        {
            case 1:
                EnableMagnet();
                break;
            case 2:
                Enable2X();
                break;
            case 3:
                EnableRocket();
                break;
        }
    }

    private void EnableMagnet()
    {
        GameState.enableMagnet();
        //Show on canvas that magnet is active -> ce instanca ze obstaja je ne kreiramo se enkrat
        if (magnetActiveIconInstance == null)
        {
            magnetActiveIconInstance = Instantiate(magnetActiveIconPrefab) as GameObject;
            magnetActiveIconInstance.transform.SetParent(canvas.transform, false);
            //set x position of icon power up where it is supposed to go
            magnetActiveIconInstance.transform.localPosition += new Vector3(nextPositionXOffsetOfUIIcon, 0, 0);
            //posodobi pozicijo naslednjega power up icona
            activeIconInstances.Add(magnetActiveIconInstance);
            nextPositionXOffsetOfUIIcon += distanceBetwenUIIcons;
        }
        //prikazemo trenutno stanje magnet time counterja
        //vzamemo 2. gameobject znotraj instance magnet UI
        int magnetActiveIconIndex = activeIconInstances.IndexOf(magnetActiveIconInstance);
        Text magnetTimeCounter = activeIconInstances[magnetActiveIconIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
        magnetTimeCounter.text = GameState.totalMagnetTime.ToString("F0");
        //play power up pickup sound
        SoundManager.PlaySound("powerUpPickup");
    }

    private void Enable2X()
    {
        GameState.enable2XPower();
        //Show on canvas that 2x is active -> ce instanca ze obstaja je ne kreiramo se enkrat
        if (twoXActiveIconInstance == null)
        {
            twoXActiveIconInstance = Instantiate(twoXActiveIconPrefab) as GameObject;
            twoXActiveIconInstance.transform.SetParent(canvas.transform, false);
            //set x position of icon power up where it is supposed to go
            twoXActiveIconInstance.transform.localPosition += new Vector3(nextPositionXOffsetOfUIIcon, 0, 0);
            //posodobi pozicijo naslednjega power up icona
            activeIconInstances.Add(twoXActiveIconInstance);
            nextPositionXOffsetOfUIIcon += distanceBetwenUIIcons;
        }
        //prikazemo trenutno stanje 2x time counterja
        //vzamemo 2. gameobject znotraj instance 2x UI
        int twoXActiveIconIndex = activeIconInstances.IndexOf(twoXActiveIconInstance);
        Text twoXTimeCounter = activeIconInstances[twoXActiveIconIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
        twoXTimeCounter.text = GameState.total2XPowerTime.ToString("F0");
        //play power up pickup sound
        SoundManager.PlaySound("powerUpPickup");
    }

    private void EnableRocket()
    {
        GameState.enableRocketPower();
        //Show on canvas that reverse keys is active -> ce instanca ze obstaja je ne kreiramo se enkrat
        if (rocketPowerActiveIconInstance == null)
        {
            rocketPowerActiveIconInstance = Instantiate(rocketPowerActiveIconPrefab) as GameObject;
            rocketPowerActiveIconInstance.transform.SetParent(canvas.transform, false);
            //set x position of icon power up where it is supposed to go
            rocketPowerActiveIconInstance.transform.localPosition += new Vector3(nextPositionXOffsetOfUIIcon, 0, 0);
            //posodobi pozicijo naslednjega power up icona
            activeIconInstances.Add(rocketPowerActiveIconInstance);
            nextPositionXOffsetOfUIIcon += distanceBetwenUIIcons;
        }
        //prikazemo trenutno stanje reverse keys time counterja
        //vzamemo 2. gameobject znotraj instance reversekeys UI
        int rocketPowerActiveIconIndex = activeIconInstances.IndexOf(rocketPowerActiveIconInstance);
        Text rocketPowerTimeCounter = activeIconInstances[rocketPowerActiveIconIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
        rocketPowerTimeCounter.text = GameState.totalReverseKeysTime.ToString("F0");
        //play power up pickup sound
        SoundManager.PlaySound("powerUpPickup");
    }

    private void CollectCoin()
    {
        //naredi effect pobranega
        GameObject effect = Instantiate(coinPickupEffect) as GameObject;
        effect.transform.SetParent(this.transform);
        effect.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1.5f, this.transform.position.z);
        GameState.collectCoin();
        coinScore.text = GameState.coinScore.ToString("0000000");
        highScore.text = (PlayerPrefs.GetInt("HighScore", 0) > GameState.coinScore) ? (PlayerPrefs.GetInt("HighScore", 0) - GameState.coinScore).ToString() : GameState.coinScore.ToString();
        //Ponastavimo high score text, ce smo dosegli now high score
        if (PlayerPrefs.GetInt("HighScore", 0) < GameState.coinScore && previousHighScore.activeSelf && !newHighScore.activeSelf)
        {
            previousHighScore.SetActive(false);
            newHighScore.SetActive(true);
            SoundManager.PlaySound("highScoreReached");
        }
        //play coin pickup sound
        SoundManager.PlaySound("coinPickup");
        Destroy(effect, coinEffectDuration);
    }

    private void DisableReverseKeys()
    {
        GameState.disableReverseKeys();
        //destrojamo instanco 2x icona, ker je power potekel
        int reverseKeysActiveIconIndex = activeIconInstances.IndexOf(reverseKeysActiveIconInstance);
        Destroy(activeIconInstances[reverseKeysActiveIconIndex]);
        activeIconInstances.RemoveAt(reverseKeysActiveIconIndex);
        reverseKeysActiveIconInstance = null;
        //posodobi pozicijo naslednjega powerupa (zamakni nazaj za razmik med dvema)
        nextPositionXOffsetOfUIIcon -= distanceBetwenUIIcons;

        //Posodobimo v seznamu vseh icon power upov position power upov za pravkar deletanim da se pomaknejo naprej
        for (int i = reverseKeysActiveIconIndex; i < activeIconInstances.Count; i++)
        {
            activeIconInstances[i].transform.localPosition -= new Vector3(distanceBetwenUIIcons, 0, 0);
        }
    }
}
