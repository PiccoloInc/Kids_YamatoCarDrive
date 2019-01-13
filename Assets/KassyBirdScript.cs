using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class KassyBirdScript : MonoBehaviour
{

   public AudioClip audioClip1;
    private AudioSource audioSource;
    
    public Rigidbody rb;
    
    private bool leftFlag = false;
    private bool upFlag = false;

    private bool rightFlag = false;
    float upspeed = 0.0f;
    float speed = 0.1f;
    float x = -80.0f;
    private long lastTimeStamp;

    private Vector3 startPosition;

	bool upPressed = false;
	bool leftPressed = false;
	bool rightPressed = false;

    bool birdFlag = true;

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
        OSCHandler.Instance.serverInit();
        lastTimeStamp = -1;
        Physics.gravity = new Vector3(0f, -100.0f, 0.0f);

        startPosition = transform.position;
    }


    //オブジェクトが衝突したとき
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTrigger");
        speed = 0.0f;
    }

    //オブジェクトが衝突したとき
    void OnCollisionEnter(Collision other)
    {
        Debug.Log("OnCollision");
    }

    // Update is called once per frame
    void Update()
    {
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



        //  受信データの更新
        OSCHandler.Instance.UpdateLogs();
        //  受信データの解析
        foreach (KeyValuePair<string, ServerLog> item in OSCHandler.Instance.Servers)
        {
            for (int i = 0; i < item.Value.packets.Count; i++)
            {
                if (lastTimeStamp < item.Value.packets[i].TimeStamp)
                {
                    lastTimeStamp = item.Value.packets[i].TimeStamp;

                    //  OSCアドレスを取得
                    string address = (string)item.Value.packets[i].Address;

                    //  データ型を取得
                    string dataType = (string)item.Value.packets[i].Data[0];

                    // 受信データの値を取得
                    int dataValue = (int)item.Value.packets[i].Data[1];

                    // 処理の振り分けの例
                    if (address == "/vvp")
                    {
                        if (dataType == "bool")
                        {
                            if (dataValue == 1)
                            {
                                Debug.Log("アドレスが/vvp、データ型がデジタル、値がtrueのデータを受信しました。");
                            }
                            else
                            {
                                Debug.Log("アドレスが/vvp、データ型がデジタル、値がfalseのデータを受信しました。");
                            }
                        }
                        else
                        {
                            Debug.Log("アドレスが/vvp、データ型がアナログ、値が" + dataValue + "のデータを受信しました。");
                            if(dataValue > 90 && birdFlag) {
                                birdFlag = false;
                                Debug.Log("jump!!");
                                jump2();
                            } 
                            if (dataValue < 60) {
                                birdFlag = true;
                            }
                        }

                    }
                    else if (address == "/left")
                    {
                        if (dataType == "bool")
                        {
                            if (dataValue == 1)
                            {
                                Debug.Log("アドレスが/left、データ型がデジタル、値がtrueのデータを受信しました。");
                                leftFlag = true;
                            } else {
                                leftFlag = false;
                            }
                        }

                    }
                    else if (address == "/up")
                    {
                        if (dataType == "bool")
                        {
                            if (dataValue == 1)
                            {
                                Debug.Log("アドレスが/up、データ型がデジタル、値がtrueのデータを受信しました。");
                                upFlag = true;
                            } else {
                                upFlag = false;
                            }
                        }

                    }
                    else if (address == "/right")
                    {
                        if (dataType == "bool")
                        {
                            if (dataValue == 1)
                            {
                                Debug.Log("アドレスが/right、データ型がデジタル、値がtrueのデータを受信しました。");
                                rightFlag = true;
                                rotateLeft();
                            } else {
                                rightFlag = false;
                            }
                        }

                    }
                    else
                    {
                        Debug.Log("アドレスが" + address + "、データ型が" + dataType + "、値が" + dataValue + "のデータを受信しました。");
                    }
                }
            }
        }


        transform.position += transform.forward * speed;

        // ジャンプ
        if (Input.GetKey(KeyCode.F)) {
            jump();
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            x -= 1.5f;
            //transform.rotation = Quaternion.Euler (0, x, 0);
            transform.Rotate(0, -1.0f, 0, Space.Self);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            x += 1.5f;
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
        }

        if (leftFlag) {
            rotateLeft();
        }
        if (rightFlag) {
            rotateRight();
        }

        if (upFlag) {
            speed += 0.005f;
        }

    }

    void rotateRight() {
        transform.Rotate(0, 1.0f, 0, Space.Self);
    }
    void rotateLeft() {
        transform.Rotate(0, -1.0f, 0, Space.Self);
    }

    void jump() {
                    
        Rigidbody rb2 = this.GetComponent<Rigidbody> ();  

        Debug.Log("addforce");
        upspeed += 0.2f;
        Vector3 force = new Vector3 (0.0f,5.0f,0.0f);
        rb2.AddForce(force, ForceMode.Impulse);
        // rb.AddForce(force);

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioClip1;
        audioSource.Play ();

    }

    void jump2() {
                    
        Rigidbody rb2 = this.GetComponent<Rigidbody> ();  

        Debug.Log("addforce");
        upspeed += 0.2f;
        Vector3 force = new Vector3 (0.0f,30.0f,0.0f);
        rb2.AddForce(force, ForceMode.Impulse);
        // rb.AddForce(force);

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioClip1;
        audioSource.Play ();

    }

}