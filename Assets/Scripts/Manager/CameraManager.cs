using Cinemachine;
using UnityEngine;

public class CameraManager : UnitySingleton<CameraManager>
{
    private CinemachineVirtualCamera _playerCam;

    private CinemachineVirtualCamera playerCam
    {
        get
        {
            if (_playerCam == null)
            {
                _playerCam = GameObject.Find("PlayerCam")?.GetComponent<CinemachineVirtualCamera>();
                if (_playerCam == null)
                {
                    Debug.LogError("PlayerCam not found in the scene. Please add a CinemachineVirtualCamera named 'PlayerCam' to the scene.");
                }
            }
            return _playerCam;
        }
    }
    public void SetPlayerTarget(Transform playerTransform)
    {
        Debug.Log(playerCam);
        Debug.Log(playerTransform);
        if (playerCam != null && playerTransform != null)
        {
            playerCam.Follow = playerTransform;
        }
        else
        {
            Debug.LogError("Failed to set camera target. PlayerCam or player transform is null.");
        }
    }
}
