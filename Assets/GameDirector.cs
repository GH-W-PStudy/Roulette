using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// ��U�s�g�p
public class GameDirector : MonoBehaviour
{
    // �u������x�v�{�^��
    GameObject RetryButton;

    // Start is called before the first frame update
    void Start()
    {
        this.RetryButton = GameObject.Find("RetryButton");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �u������x�v������������V�[���������[�h
    public void RetryButtonDown()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
