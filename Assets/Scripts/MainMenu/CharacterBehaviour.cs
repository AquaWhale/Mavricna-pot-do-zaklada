using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Rigidbody rigidBody;

    private readonly float interpolation = 10;

    private bool wasGrounded;

    private float jumpTimeStamp = 0;
    private float minJumpInterval = 0.25f;

    private float currentV = 0.0f;
    private float v = 0.0f;

    private bool isGrounded;
    private List<Collider> collisions = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Grounded", isGrounded);
        Walk();
        wasGrounded = isGrounded;
    }

    private void Walk()
    {
        v = Mathf.Lerp(v, 0.5f, Time.deltaTime*interpolation);

        //current V je samo zato, da vemo kako hitro naj poteka animacija teka
        currentV = Mathf.Lerp(currentV, v, Time.deltaTime * interpolation);

        animator.SetFloat("MoveSpeed", currentV);

        LandIfInAir();
    }

    private void LandIfInAir()
    {

        if (!wasGrounded && isGrounded)
        {
            animator.SetTrigger("Land");
            animator.SetTrigger("Wave");
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
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
}
