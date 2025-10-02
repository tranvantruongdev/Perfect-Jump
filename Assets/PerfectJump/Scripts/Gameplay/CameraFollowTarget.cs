using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    [Header("Object to follow")]
    public GameObject TargetXFollowGameObject;
    [Header("Follow speed")]
    [Range(0.0f, 50.0f)]
    public float SpeedConfig = 40f;

    [Header("Camera Offset")]
    [Range(-10.0f, 10.0f)]
    public float XOffsetConfig = 1.5f;

    [Space(15)]
    public UIManager UIManagerInstance;

    float _interpolationVar;
    Vector3 _positionVector3;

    //camera follow the player
    void LateUpdate()
    {
        if (UIManagerInstance.GameStateEnum == GameStateEnum.PLAYING)
        {
            _interpolationVar = SpeedConfig * Time.deltaTime;

            _positionVector3 = transform.position;

            if (TargetXFollowGameObject.transform.position.x + XOffsetConfig > transform.position.x)
                _positionVector3.x = Mathf.Lerp(transform.position.x, TargetXFollowGameObject.transform.position.x + XOffsetConfig, _interpolationVar);

            transform.position = _positionVector3;
        }
    }
}