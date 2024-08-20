using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// Gun Ŭ������ ���� ����� �����ϸ�, Photon ��Ʈ��ũ�� ���� ��Ƽ�÷��̾� ȯ�濡���� �۵��ϵ��� ����Ǿ����ϴ�.
public class Gun : MonoBehaviourPun, IPunObservable
{
    // ���� ���¸� �����ϴ� ������
    public enum State { Ready, Empty, Reloading }
    public State state { get; private set; } // ���� ���� ����

    public Transform fireTransform; // �߻� ��ġ
    public ParticleSystem muzzleFlashEffect; // �ѱ� �Ҳ� ȿ��
    public ParticleSystem shellEjectEffect; // ź�� ���� ȿ��
    public LineRenderer lineRenderer; // �Ѿ� ������ ��Ÿ���� ��
    private AudioSource gunAudioPlayer; // ���� ����� �ҽ�
    public AudioClip shotClip; // �� �߻� �Ҹ�
    public AudioClip relordcilp; // ������ �Ҹ�
    public float damage = 25f; // ���� ������
    public float firedistance = 50f; // ���� �����Ÿ�
    internal int ammoRemain = 50; // ���� ��ü ź��
    internal int magCapacity = 25; // ���� �ִ� ź�� �뷮
    internal int magAmmo; // ���� ���� ź�� ��
    public float timeBetfire = 0.12f; // �߻� ����
    public float reloadTime = 1.0f; // ������ �ð�
    private float lastFiretime; // ������ �߻� �ð�

    void Awake()
    {
        // �ʿ��� ������Ʈ�� �ʱ�ȭ
        gunAudioPlayer = GetComponent<AudioSource>(); // ����� �ҽ� ������Ʈ ��������
        lineRenderer = GetComponent<LineRenderer>(); // �� ������ ������Ʈ ��������

        lineRenderer.positionCount = 2; // ���� �� ���� ����
        lineRenderer.enabled = false; // �ʱ⿡�� �� ��Ȱ��ȭ
    }

    private void OnEnable()
    {
        magAmmo = magCapacity; // ������ �� źâ�� ���� ä��
        state = State.Ready; // ���¸� �غ� �Ϸ�� ����
        lastFiretime = 0f; // ������ �߻� �ð� �ʱ�ȭ
    }

    // �� �߻� �õ� �޼ҵ�
    public void Fire()
    {
        // ���� ���°� �غ� �����̰�, �߻� ������ �ð����� Ȯ��
        if (state == State.Ready && Time.time >= lastFiretime + timeBetfire)
        {
            lastFiretime = Time.time; // ������ �߻� �ð� ����
            Shot(); // ���� �߻� �޼ҵ� ȣ��
        }
    }

    // ���� �߻� ����
    private void Shot()
    {
        RaycastHit hit; // �浹 ������ ������ ����
        Vector3 hitposition = Vector3.zero; // �浹 ��ġ �ʱ�ȭ

        // �߻� ��ġ���� �������� ����ĳ��Ʈ
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, firedistance))
        {
            // �浹�� ������Ʈ���� IDeamage �������̽��� ������ ��ü�� ������
            IDeamage target = hit.collider.GetComponent<IDeamage>();
            if (target != null)
            {
                // ���濡�� �������� ����
                target.OnDeamge(damage, hit.point, hit.normal);
            }
            // ���̰� �浹�� ��ġ ����
            hitposition = hit.point;
        }
        else
        {
            // ���̰� �浹���� �ʾ��� ��� �ִ� ���� �Ÿ����� ��ġ ���
            hitposition = fireTransform.position + fireTransform.forward * firedistance;
        }

        // �߻� ȿ�� ����
        StartCoroutine(ShotEffect(hitposition));
        // ȣ��Ʈ�� �߻� ������ ���
        photonView.RPC("ShotProcessOnSever", RpcTarget.MasterClient);
        // ���� �߻�ó���� ȣ��Ʈ�� �븮 
        magAmmo--; // ���� ź�� �� ����

        // ź���� 0 ������ ��� ���¸� Empty�� ����
        if (magAmmo <= 0)
        {
            state = State.Empty;
        }
    }

    [PunRPC] // ȣ��Ʈ�� ������ �߻� ó��
    private void ShotProcessOnSever()
    {
        RaycastHit hit;
        Vector3 hitPos = Vector3.zero;
        if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, firedistance))
        {
            IDeamage target = hit.collider.GetComponent<IDeamage>();
            if (target != null)
            {
                target.OnDeamge(damage, hit.point, hit.normal);
            }
            hitPos = hit.point;

        }
        else
        {
            hitPos = fireTransform.position + fireTransform.forward * firedistance;
        }

        photonView.RPC("ShotEffectProcessOnClient", RpcTarget.All, hitPos);
    }

    [PunRPC]
    void ShotEffectProcessOnClient(Vector3 hitpos)
    {
        StartCoroutine(ShotEffect(hitpos));
    }

    // �߻� ȿ���� ó���ϴ� �ڷ�ƾ
    IEnumerator ShotEffect(Vector3 hitposition)
    {
        lineRenderer.enabled = true; // �� Ȱ��ȭ
        muzzleFlashEffect.Play(); // �ѱ� �Ҳ� ȿ�� ���
        shellEjectEffect.Play(); // ź�� ���� ȿ�� ���
        gunAudioPlayer.PlayOneShot(shotClip); // �� �߻� �Ҹ� ���

        // ���� �������� ���� ����
        lineRenderer.SetPosition(0, fireTransform.position);
        lineRenderer.SetPosition(1, hitposition);
        yield return new WaitForSeconds(0.03f); // ��� ���

        lineRenderer.enabled = false; // �� ��Ȱ��ȭ
    }

    // ������ �õ� �޼ҵ�
    public bool Relord()
    {
        // ���� ���°� ������ ���̰ų� ���� ź���� ���ų� źâ�� ���� �������� false ��ȯ
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= magCapacity)
        {
            return false;
        }

        StartCoroutine(RelordRoutine()); // ������ ��ƾ ����
        return true; // ������ �õ� ����
    }

    // ������ ��ƾ �ڷ�ƾ
    IEnumerator RelordRoutine()
    {
        state = State.Reloading; // ���¸� ������ ������ ����
        gunAudioPlayer.PlayOneShot(relordcilp); // ������ �Ҹ� ���

        yield return new WaitForSeconds(reloadTime); // ������ �ð� ���

        // ä�� ź�� �� ���
        int ammoToFill = magCapacity - magAmmo;
        // ���� ź���� ������ ��� ä���� �� ź�� �� ����
        if (ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }
        // źâ�� ź�� ä���
        magAmmo += ammoToFill;
        // ���� ź�࿡�� ä�� ��ŭ ����
        ammoRemain -= ammoToFill;

        state = State.Ready; // ���¸� �غ� �Ϸ�� ����
    }

    [PunRPC]
    public void AddAmmo(int ammo)
    {
        ammoRemain += ammo; // ���� ź�� �� ����
    }

    // Photon ��Ʈ��ũ ���� ����ȭ �޼ҵ�
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // ���ÿ��� �۽��ϴ� ���
        {
            stream.SendNext(ammoRemain); // ���� ź�� �� �۽�
            stream.SendNext(magAmmo); // ���� ź�� �� �۽�
            stream.SendNext(state); // ���� ���� ���� �۽�
        }
        else if (stream.IsReading) // �ٸ� ������ ���¸� �����ϴ� ���
        {
            ammoRemain = (int)stream.ReceiveNext(); // ���� ź�� �� ����
            magAmmo = (int)stream.ReceiveNext(); // ���� ź�� �� ����
            state = (State)stream.ReceiveNext(); // ���� ���� ����
        }
    }
}
