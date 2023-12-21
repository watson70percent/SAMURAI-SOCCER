using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.StageContents.UK
{
    public class StrikeObjectScript : MonoBehaviour
    {
        [SerializeField] float shotInterval = 4.0f; //��Q�������Ԋu
        float elapsedTime;//������o�߂�������
        [SerializeField] GameObject ShotObject;//��������object
        [SerializeField] Transform[] Cannons;//�����������ꏊ�ɃI�u�W�F�N�g�u���ăA�^�b�`
        [SerializeField] Transform player;

        int index, maxIndex;

        private void Start()
        {
            maxIndex = Cannons.Length;
            InGameEvent.Standby.Subscribe(_ =>
            {
                elapsedTime = 0;
            }).AddTo(this);
            InGameEvent.UpdateDuringPlay.Subscribe(_ =>
            {
                //���Ԃ��o������object����
                if (elapsedTime >= shotInterval) Shot();
                //���ԑ��₷
                elapsedTime += Time.deltaTime;
            }).AddTo(this);
        }

        //���Ԃ��o������object����
        void Shot()
        {
            index = (int)Random.Range(0, maxIndex);
            Instantiate(ShotObject, Cannons[index].position, Cannons[index].rotation);
            elapsedTime = 0.0f;
        }
    }
}