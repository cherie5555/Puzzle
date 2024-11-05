using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;


public class PlayerControll : MonoBehaviour
{
    public Rigidbody2D rb;
    public CharacterController2D controller;
    private float runSpeed = 0.5f;
    private float horizontalMove = 0f;
    public float hp;
    public float hp_max = 5f;
    public Image hp_bar;
    public GameObject BulletPrefab;
    public Animator animator;
    private float pushSpeed = 2f;



    void Start()
    {
        hp = hp_max;
        print("start");
        StartCoroutine(CutHp());
    }

    private IEnumerator CutHp()
    {
        while (true)
        {
            if (hp < 0.1f)
            {
                hp = 0f;
            }
            else
            {
                // 做一些事情，例如移動、播放動畫等
                hp -= 0.1f;
            }
            // 等待 10 秒
            yield return new WaitForSeconds(1f);
        }
    }
    private bool jump;
    private bool isWalking = false;
    public float stepInterval = 1f; // 步行聲音的間隔時間
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W");
            jump = true;
        }

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (horizontalMove != 0f)
        {
            if (!isWalking)
            {
                isWalking = true;
                AudioManager.Instance.PlaySFX("walk");
            }
        }
        else
        {
            isWalking = false;
            AudioManager.Instance.sfxSource.Stop();
        }

        animator.SetFloat("Speed", Math.Abs(horizontalMove));

        if (hp_bar.transform.localScale.x >= 0)
        {
            hp_bar.transform.localScale = new Vector3((float)hp / (float)hp_max, hp_bar.transform.localScale.y, hp_bar.transform.localScale.z);
        }
        Debug.Log(Input.GetAxisRaw("Horizontal"));
    }

    void FixedUpdate()
    {

        // 使用 CharacterController2D 的 Move 方法
        controller.Move(horizontalMove, false, jump);
        jump = false; // 只允许一次跳跃
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Portal")
        {
            coll.gameObject.transform.GetComponent<Portal>().ChangeScene();
        }


    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "pill")
        {
            if (hp < hp_max)
            {
                print(coll.gameObject.name);
                hp += 0.7f;
            }
        }


    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Box"))
        {
            Rigidbody2D boxRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (boxRb != null && boxRb.isKinematic)
            {
                // 获取水平输入方向，仅允许左右推动
                float horizontalInput = Input.GetAxis("Horizontal");
                if (Mathf.Abs(horizontalInput) > 0.1f) // 确保有输入
                {
                    // 按照输入方向移动箱子
                    Vector2 pushDirection = new Vector2(horizontalInput, 0);
                    boxRb.transform.Translate(pushDirection * pushSpeed * Time.deltaTime);
                }
            }
        }
    }

}
