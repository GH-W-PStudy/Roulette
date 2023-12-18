using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RouletteController : MonoBehaviour
{
    float rotSpeed = 0; // ��]���x
    Boolean isRightClicked = false; //�E�N���b�N�`�F�b�N
    Boolean isStopped = false; //��~�`�F�b�N


    // Start is called before the first frame update
    // Start���\�b�h�F�X�N���v�g�N������Ɉ�x�������s
    void Start()
    {
        // �t���[�����[�g��60�ɌŒ�(�f�o�C�X�ɂ�鐫�\�����Ȃ���)
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    // Update���\�b�h�F�t���[�����ƂɎ��s
    void Update()
    {
        // �}�E�X�ō��N���b�N���ꂽ�烋�[���b�g����
        if (Input.GetMouseButtonDown(0) && this.rotSpeed == 0)
        {
            // ��]���x���Z�b�g
            setRotSpeed();
        }

        // ��]���x�Ԃ�A���[���b�g����]������
        transform.Rotate(0, 0, this.rotSpeed);


        // �}�E�X�ŉE�N���b�N���ꂽ�烋�[���b�g����������~
        if (Input.GetMouseButtonDown(1))
        {
            this.isRightClicked = true;
        }
        if (isRightClicked == true)
        {
            SlowDown();
        }

        // ���[���b�g�ĊJ
        if (isStopped == true && Input.GetMouseButtonDown(0))
        {
            this.isRightClicked = false;
            setRotSpeed();
        }

    }


    void setRotSpeed()
    {
        this.rotSpeed = 30;
    }

    void SlowDown()
    {
        if (this.rotSpeed >= 0.1)
        {
            // ���[���b�g�����������Ă���
            this.rotSpeed *= 0.975f;

        }
        else
        {
            isStopped = true;
            this.rotSpeed = 0;
        }
    }

    // �ȉ�������
    //// �u������x�v������������V�[���������[�h
    //public void RetryButtonDown()
    //{
    //    GameObject director = GameObject.Find("GameDirector");
    //    director.GetComponent<GameDirector>().RetryButtonDown();
    //}
}
