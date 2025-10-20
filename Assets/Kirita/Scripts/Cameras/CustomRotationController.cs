using UnityEngine;
using Unity.Cinemachine;

namespace CustomCinemachineModules
{
    [CameraPipeline(CinemachineCore.Stage.Aim)]
    [AddComponentMenu("Cinemachine/Custom/Rotation Controller (Look Local Origin)")]
    [SaveDuringPlay]
    public class LookLocalOriginRotationController : CinemachineComponentBase
    {
        [Tooltip("��]�̃X���[�W���O���x")]
        [Range(0f, 20f)]
        public float rotationSmooth = 10f;

        public override bool IsValid => enabled;

        // ��]����Ȃ̂� Aim �X�e�[�W
        public override CinemachineCore.Stage Stage => CinemachineCore.Stage.Aim;

        public override void MutateCameraState(ref CameraState curState, float deltaTime)
        {
            if (!IsValid)
                return;

            Vector3 camPos = curState.RawPosition;
            Vector3 targetPos = FollowTarget.position;

            // �������Z�o
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