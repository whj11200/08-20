using UnityEngine;

public interface IDeamage
{                //�������̽� ��ü�� ������ Ŀ�ø�
                 // �� �޼��带 �޴� Ŭ������ � Ŭ��������
                 // �˻縦 ���� �ʴ´�.
    void OnDeamge(float damge, Vector3 hitPoint, Vector3 hitNormal);
    
}
