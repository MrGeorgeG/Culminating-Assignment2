﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] ParticleSystem PE_Fireball;
    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask grasslayer;

    public event Action OnEncountered;

    public bool isMoving;
    private Vector2 input;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (PlayerPrefs.GetFloat("PosX") != 0 && PlayerPrefs.GetFloat("PosY")!=0)
        {
            transform.position = new Vector2(PlayerPrefs.GetFloat("PosX"), PlayerPrefs.GetFloat("PosY"));
        }
    }
    private void Update()
    {
        //PlayerPrefs.SetFloat("PosX", transform.position.x);
        //PlayerPrefs.SetFloat("PosY", transform.position.y);
    }

    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //remove diagonal movement
            if (input.x != 0) input.y = 0;

            if(input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if(IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }

            animator.SetBool("isMoving", isMoving);

            if(Input.GetKeyDown(KeyCode.Space))
            {
                Interact();
            }
        }

        void Interact()
        {
            var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
            var interactPos = transform.position + facingDir;

            //Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

            var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);

            if(collider != null)
            {
                collider.GetComponent<NPCController>()?.Interact();
            }
        }

        IEnumerator Move(Vector3 targetPos)
        {
            isMoving = true;

            while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPos;

            isMoving = false;

            CheckForEncounters();

        }
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }

        return true;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grasslayer) != null)
        {
            if (UnityEngine.Random.Range(1,101) <= 10)
            {
                Debug.Log("Encountered a wild enemy");
                animator.SetBool("isMoving", false);
                OnEncountered();
            }
        }
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("PosX", transform.position.x);
        PlayerPrefs.SetFloat("PosY", transform.position.y);
    }
}
