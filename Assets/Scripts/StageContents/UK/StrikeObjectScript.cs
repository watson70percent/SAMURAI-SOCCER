using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRX;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.UK
{
    public class StrikeObjectScript : MonoBehaviour
    {
        [SerializedField] float shotInterval = 4.0f; //��Q�������Ԋu
        [SerializedField] float emergencyInterval = 2.0f; //�x�����ԊԊu
        float elapsedTime;//������o�߂�������
        [SerializedField] GameObject ShotObject;//��������object
        //float[] shotPosZ = { 15.5f, 50, 84.3f };//�O���E���h�̓��H�̍��W
        [SerializedField] Transform[] Cannons;//�����������ꏊ�ɃI�u�W�F�N�g�u���ăA�^�b�`
        [SerializedField] GameObject emergencySign;
        [SerialisedField] Transform player;

        int index, maxIndex;
        Vector3 emergePos;

        private void Start()
        {
            maxIndex = Cannons.Length;
            InGameEvent.Standby.Subscribe(_ =>
            {
                elapsedTime = 0;
            });
            InGameEvent.UpdateDuringPlay(_ =>
            {//���Ԃ��o������object�����܂Ōx��
                if (elapsedTime >= shotInterval - emergencyInterval) ShowEmerge();
                //���Ԃ��o������object����
                if (elapsedTime >= shotInterval) Shot();
                //���ԑ��₷
                elapsedTime += Time.deltaTime;
            });

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
            index = (int)RandomRange(0, maxIndex);
            Instantiate(ShotObject, Cannons[index].position, Cannons[index].rotation);
            emergencySign.SetActive(false);
            elapsedTime = 0.0f;
        }



    }
}