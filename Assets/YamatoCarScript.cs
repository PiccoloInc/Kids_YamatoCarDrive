using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YamatoCarScript : MonoBehaviour
{
    float speed = 0.2f;
    float x = -80.0f;
    private long lastTimeStamp;
    private GameObject MainCam;
    private GameObject SubCam;
    private GameObject yamato;
    private GameObject takumi;

    public AudioClip audioClip1;
    public AudioSource audioSource1;
    public AudioClip audioClip2;
    public AudioSource audioSource2;
    private Vector3 startPosition;

	bool upPressed = false;
	bool leftPressed = false;
	bool rightPressed = false;


	public void OnLeftPressed() {
		Debug.Log("onPressed");
		leftPressed = true;
	}
	public void OnLeftReleased() {	
		Debug.Log("onreleased");
		leftPressed = false;
	}
	public void OnUpPressed() {
		Debug.Log("onPressed");
		upPressed = true;
	}
	public void OnUpReleased() {	
		Debug.Log("onreleased");
		upPressed = false;
	}

	public void OnRightPressed() {
		Debug.Log("onPressed");
		rightPressed = true;
	}
	public void OnRightReleased() {	
		Debug.Log("onreleased");
		rightPressed = false;
	}

	public void OnClick() {
		Debug.Log("oncLick");

		// x -= 2f;
		// transform.rotation = Quaternion.Euler (0, x, 0);
	}	
	public void OnClick2() {
		Debug.Log("oncLick2");
		x += 2f;
		transform.rotation = Quaternion.Euler (0, x, 0);
	}


    // Use this for initialization
    void Start()
    {
        OSCHandler.Instance.Init();
        lastTimeStamp = -1;

        MainCam = GameObject.Find("Camera1");
        SubCam = GameObject.Find("Camera2");
        yamato = GameObject.Find("Camera3");
        takumi = GameObject.Find("camera4");

        Physics.gravity = new Vector3(0f, -50.0f, 0.0f);

        audioSource2.clip = audioClip2;
        startPosition = transform.position;
    }


    //オブジェクトが衝突したとき
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTrigger");
        speed = 0.0f;

        audioSource1.clip = audioClip1;
        audioSource1.Play();

    }

    //オブジェクトが衝突したとき
    void OnCollisionEnter(Collision other)
    {
        Debug.Log("OnCollision");
    }

    // Update is called once per frame
    void Update()
    {
        OSCHandler.Instance.UpdateLogs();

		if (leftPressed) {
			x -= 2;
			transform.rotation = Quaternion.AngleAxis (x, Vector3.up);
		}		

		if (rightPressed) {
			x += 2;
			transform.rotation = Quaternion.AngleAxis (x, Vector3.up);
		}		
		if (upPressed) {
			speed += 0.005f;
		}

        //speed -= 0.001f;
        //if (speed < 0) {
        //	speed = 0;
        //}
        speed *= 1f;

        foreach (KeyValuePair<string, ServerLog> item in OSCHandler.Instance.Servers)
        {
            for (int i = 0; i < item.Value.packets.Count; i++)
            {
                //				if (lastTimeStamp < item.Value.packets [i].TimeStamp) {
                lastTimeStamp = item.Value.packets[i].TimeStamp;

                string address = item.Value.packets[i].Address;
                var dataType = item.Value.packets[i].Data[0];
                var dataValue = item.Value.packets[i].Data[1];
                Debug.Log("data = " + dataType + ", " + dataValue + ", address = " + address);
                if ((int)dataValue == 1)
                {
                    if (dataType.Equals("bool"))
                    {
                        Debug.Log("aaaaaaaaa");
                    }
                }
                if (dataType.Equals("int"))
                {
                    if (address.Equals("/up"))
                    {
                        speed += (int)dataValue * 0.001f;
                    }
                    if (address.Equals("/down"))
                    {
                        speed -= (int)dataValue * 0.001f;
                    }
                    if (address.Equals("/left"))
                    {
                        Debug.Log("left coming");
                        x -= (float)dataValue * 0.5f;
                        transform.rotation = Quaternion.Euler(0, x, 0);
                    }
                    if (address.Equals("/right"))
                    {
                        x += (float)dataValue * 0.5f;
                        transform.rotation = Quaternion.Euler(0, x, 0);
                    }
                    //					}

                }
            }
        }

        transform.position += transform.right * speed;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            x -= 1.5f;

            audioSource2.Play();

            //transform.rotation = Quaternion.Euler (0, x, 0);
            transform.Rotate(0, -1.0f, 0, Space.Self);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            x += 1.5f;
            audioSource2.Play();
            //transform.rotation = Quaternion.Euler (0, x, 0);
            transform.Rotate(0, 1.0f, 0, Space.Self);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            speed += 0.005f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("baaaack");
            speed -= 0.005f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 1 がおされたとき
            transform.position = startPosition;
            transform.rotation = Quaternion.Euler(0, -90.0f, 0);
            speed = 0.0f;

        }
        else
        {
            audioSource2.Stop();
        }



        if (Input.GetKeyDown("space"))
        {
            if (MainCam.activeSelf)
            {
                MainCam.SetActive(false);
                SubCam.SetActive(true);
                yamato.SetActive(false);
                takumi.SetActive(false);
            }
            else if (SubCam.activeSelf)
            {
                MainCam.SetActive(false);
                SubCam.SetActive(false);
                yamato.SetActive(true);
                takumi.SetActive(false);
            }
            else if (yamato.activeSelf)
            {
                MainCam.SetActive(false);
                SubCam.SetActive(false);
                yamato.SetActive(false);
                takumi.SetActive(true);
            }
            else
            {
                MainCam.SetActive(true);
                SubCam.SetActive(false);
                yamato.SetActive(false);
                takumi.SetActive(false);
            }

        }
    }
}