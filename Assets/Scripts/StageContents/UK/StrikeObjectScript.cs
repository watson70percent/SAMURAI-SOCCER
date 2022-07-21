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
        [SerializeField] GameObject emergencySign;
        [SerializeField] Transform player;

        int index, maxIndex;
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
                if (elapsedTime >= shotInterval - emergencyInterval) ShowEmerge();
                //���Ԃ��o������object����
                if (elapsedTime >= shotInterval) Shot();
                //���ԑ��₷
                elapsedTime += Time.deltaTime;
            }).AddTo(this);
        }

        //���Ԃ��o������object�����܂Ōx��
        void ShowEmerge()
        {
            emergePos.x = player.position.x + 1;
            emergencySign.transform.position = player.position;
            emergencySign.SetActive(true);
        }
        //���Ԃ��o������object����
        void Shot()
        {
            index = (int)Random.Range(0, maxIndex);
            Instantiate(ShotObject, Cannons[index].position, Cannons[index].rotation);
            emergencySign.SetActive(false);
            elapsedTime = 0.0f;
        }
    }
}