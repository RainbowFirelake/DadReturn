using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticTrap : MonoBehaviour
{
    public static event Action OnEnteringTrap;

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {

        // ���� ������� ����� ����� � ��� ���� ������ �� ��� ����������� �����:
        // ����� �������
        if (other.gameObject.tag == "Player")
        {
            OnEnteringTrap.Invoke();
        }
    }
}
