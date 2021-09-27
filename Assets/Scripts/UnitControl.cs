using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour
{
    public bool isPlayer = true;

    private Vector3 targetPos = Vector3.zero;
    private Vector3 dir = Vector3.zero;

    public float moveSpeed = 3f;

    private void Update()
    {
        GetKey();
        Move();
    }

    private void GetKey()
    {
        if (!isPlayer) return;

        if (Input.GetMouseButtonDown(0))
        {
            targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPos.z = 0;
        }
    }

    private void Move()
    {
        if (!isPlayer) return;

        dir = targetPos - transform.position;

        if (dir.sqrMagnitude <= 0.1f * 0.1f)
        {
            transform.position = targetPos;
            return;
        }

        transform.position += dir.normalized * moveSpeed * Time.deltaTime;
    }
}
