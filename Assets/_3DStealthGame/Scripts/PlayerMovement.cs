using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    Animator m_Animator;

    public InputAction MoveAction;

    public float walkSpeed;
    public float turnSpeed = 20f;
    public bool isWalking;

    public float stamina;
    public float maxStamina = 100.0f;
    public bool hasStamina;

    public bool isFrozen;
    public GameObject frozenUI;
    public int spaceSpam;

    public Image StaminaBar;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        MoveAction.Enable();

        walkSpeed = 1.0f;
        stamina = 100.0f;
        hasStamina = true;
        StaminaBar.color = Color.green;

        isFrozen = false;
        spaceSpam = 0;
        StartCoroutine(RandomFear());
}

    void FixedUpdate()
    {
        if (isFrozen == false)
        {
                    
            //movement       
            var pos = MoveAction.ReadValue<Vector2>();

            float horizontal = pos.x;
            float vertical = pos.y;

            m_Movement.Set(horizontal, 0f, vertical);
            m_Movement.Normalize();
            
            //animation
            bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
            bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
            isWalking = hasHorizontalInput || hasVerticalInput;
            m_Animator.SetBool("IsWalking", isWalking);
                                   
            //rotation
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);

            m_Rigidbody.MoveRotation(m_Rotation);
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * walkSpeed * Time.deltaTime);
                   
            //sprint
            if (Input.GetKey(KeyCode.LeftShift) && stamina > 0 && hasStamina == true)
            {
                walkSpeed = 2.5f;
                stamina--;

                StaminaBar.fillAmount = stamina / maxStamina;
            }
            else
            {
                walkSpeed = 1.0f;
            }

        }

        //stamina
        if (stamina < 100 && !Input.GetKey(KeyCode.LeftShift) || stamina < 100 && hasStamina == false)
        {
            stamina = stamina + 0.5f;
            StaminaBar.fillAmount = stamina / maxStamina;
        }

        if(stamina == 0)
        {
            hasStamina = false;
            StaminaBar.color = Color.red;
        }
        if(stamina == 100)
        {
            hasStamina = true;
            StaminaBar.color = Color.green;
        }
                
    }

    private void Update()
    {
        //freezing
        m_Animator.SetBool("IsFrozen", isFrozen);
        
        if (isFrozen == true)
        {
            frozenUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                spaceSpam++;
            }
        }
        else
        {
            frozenUI.SetActive(false);
        }
        
        if (spaceSpam > 10)
        {
            isFrozen = false;
            spaceSpam = 0;
        }
    }

    IEnumerator RandomFear()
    {        
        float spawnTime = Random.Range(10, 15);
        yield return new WaitForSeconds(spawnTime);
        isFrozen = true;
        isWalking = false;
        m_Animator.SetBool("IsWalking", isWalking);
        StartCoroutine(RandomFear());
    }
}