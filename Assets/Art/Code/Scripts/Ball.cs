using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Ball : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private GameObject goalFX;

    [Header("Attributes")]
    [SerializeField] private float maxPower = 10f;
    [SerializeField] private float power = 2f;
    [SerializeField] private float maxGoalSpeed = 4f;
    
    private bool isDragging;
    private bool inHole;

    private void Update(){
        PlayerInput();
    }

    private bool IsReady(){
        return rb.velocity.magnitude <= 0.2f;
    }

    private void PlayerInput(){
        if(!IsReady()) return;
   
            Vector2 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float distance =  Vector2.Distance(transform.position, inputPos);

            if(Input.GetMouseButtonDown(0) && distance <= 0.5f) DragStart();
            if(Input.GetMouseButton(0) && isDragging) DragChange(inputPos);
            if(Input.GetMouseButtonUp(0) && isDragging) DragRelease(inputPos);
        }

        private void DragStart(){
            isDragging = true;
            lr.positionCount = 2;
        }

        private void DragChange(Vector2 pos){
            Vector2 dir = (Vector2)transform.position - pos;

            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, (Vector2)transform.position + Vector2.ClampMagnitude((dir * power)/2, maxPower/2));
        }

        private void DragRelease(Vector2 pos){
            float distance = Vector2.Distance((Vector2)transform.position,pos);
            isDragging = false;
            lr.positionCount = 0;

            if(distance < 1f){
                return;
            }

            Vector2 dir = (Vector2)transform.position - pos;

            rb.velocity = Vector2.ClampMagnitude(dir * power, maxPower);
        }

        private void CheckWinState(){
            if(inHole) return;

            if(rb.velocity.magnitude <= maxGoalSpeed){
                inHole = true;

                rb.velocity = Vector2.zero;
                gameObject.SetActive(false);

                GameObject fx = Instantiate(goalFX, transform.position, UnityEngine.Quaternion.identity);
                Destroy(fx,2f);
            }
        }

        private void OnTriggerEnter2D(Collider2D other){
            if(other.tag == "Goal") CheckWinState();
        }
                private void OnTriggerStay2D(Collider2D other){
            if(other.tag == "Goal") CheckWinState();
        }
}