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
        [SerializeField] float emergencyInterval = 2.0f; //�x�����ԊԊu
        float elapsedTime;//������o�߂�������
        [SerializeField] GameObject ShotObject;//��������object
        //float[] shotPosZ = { 15.5f, 50, 84.3f };//�O���E���h�̓��H�̍��W
        [SerializeField] Transform[] Cannons;//�����������ꏊ�ɃI�u�W�F�N�g�u���ăA�^�b�`
        [SerializeField] GameObject[] emergencySign;
        [SerializeField] Transform player;

        int index, maxIndex;
        bool isEmerge = false;
        Vector3 emergePos;

        private void Start()
        {
            maxIndex = Cannons.Length;
            InGameEvent.Standby.Subscribe(_ =>
            {
                elapsedTime = 0;
            }).AddTo(this);
            InGameEvent.UpdateDuringPlay.Subscribe(_ =>
            {//���Ԃ��o������object�����܂Ōx��
                if (elapsedTime >= shotInterval - emergencyInterval && !isEmerge) ShowEmerge();
                //���Ԃ��o������object����
                if (elapsedTime >= shotInterval) Shot();
                //���ԑ��₷
                elapsedTime += Time.deltaTime;
            }).AddTo(this);
        }

        //���Ԃ��o������object�����܂Ōx��
        void ShowEmerge()
        {
            index = (int)Random.Range(0, maxIndex);
            isEmerge = true;
            emergePos = emergencySign[index].transform.position;
            emergePos.x = player.position.x + 1;
            emergencySign[index].transform.position = emergePos;
            emergencySign[index].SetActive(true);
        }
        //���Ԃ��o������object����
        void Shot()
        {
            Debug.Log("L51 index =" + index);
            Instantiate(ShotObject, Cannons[index].position, Cannons[index].rotation);
            emergencySign[index].SetActive(false);
            elapsedTime = 0.0f;
            isEmerge = false;
        }
    }
}