using UnityEngine;
using Unity.Cinemachine;

namespace CustomCinemachineModules
{
    [CameraPipeline(CinemachineCore.Stage.Aim)]
    [AddComponentMenu("Cinemachine/Custom/Rotation Controller (Look Local Origin)")]
    [SaveDuringPlay]
    public class LookLocalOriginRotationController : CinemachineComponentBase
    {
        [Tooltip("回転のスムージング速度")]
        [Range(0f, 20f)]
        public float rotationSmooth = 10f;

        public override bool IsValid => enabled;

        // 回転制御なので Aim ステージ
        public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (!IsValid)
                return;

            Vector3 camPos = curState.RawPosition;
            Vector3 targetPos = FollowTarget.position;

            // 方向を算出
            Vector3 dir = (targetPos - camPos).normalized;
            if (dir.sqrMagnitude < 0.0001f)
                return;

            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

            if (rotationSmooth > 0 && deltaTime > 0)
                curState.RawOrientation = Quaternion.Slerp(curState.RawOrientation, targetRot, rotationSmooth * deltaTime);
            else
                curState.RawOrientation = targetRot;
        }
    }
}