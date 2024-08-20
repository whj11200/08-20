using UnityEngine;

public interface IDeamage
{                //인터페이스 자체가 느슨한 커플링
                 // 이 메서드를 받는 클래스가 어떤 클래스인지
                 // 검사를 하지 않는다.
    void OnDeamge(float damge, Vector3 hitPoint, Vector3 hitNormal);
    
}
