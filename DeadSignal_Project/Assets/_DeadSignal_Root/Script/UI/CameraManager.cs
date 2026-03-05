using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private CinemachineCamera camRoom;
    [SerializeField] private CinemachineCamera camApproach;
    [SerializeField] private CinemachineCamera camScreen;

    [SerializeField] int activePriority = 10;
    [SerializeField] int inactivePriority = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void ActivateRoomCamera()
    {
        SetPriority(camRoom);
    }

    public void ActivateApproachCamera()
    {
        SetPriority(camApproach);
    }

    public void ActivateScreenCamera()
    {
        SetPriority(camScreen);
    }

    void SetPriority(CinemachineCamera cam)
    {
        camRoom.Priority = inactivePriority;
        camApproach.Priority = inactivePriority;
        camScreen.Priority = inactivePriority;

        if (cam != null)
            cam.Priority = activePriority;
    }
}