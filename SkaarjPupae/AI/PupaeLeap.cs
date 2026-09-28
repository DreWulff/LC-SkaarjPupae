using SkaarjPupae.Animation;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace SkaarjPupae.AI {
    partial class PupaeAI : EnemyAI {
        [Tooltip("Leaping cooldown.")]
        [SerializeField]
        private float _leapCooldown = 3f;

        [Tooltip("Curve that defines the shape of the jump.")]
        [SerializeField]
        private JumpCurve _jumpCurve = new(5f, 15f);

        private float height;
        private float startingHeight;
        private float timeSinceLeap;
        private Vector3 leapStart;
        private Vector3 leapTarget;
        private float leapTime;
        private float targetTime;
        private JUMP_STATE jumpState;

        private enum JUMP_STATE {
            PREPARING,
            JUMPING,
            LANDED,
        }

        private bool LeapCondition() {
            if (targetPlayer == null || timeSinceLeap < _leapCooldown) return false;
            Vector3 player = targetPlayer.transform.position;
            Vector3 pupae = transform.position;
            if (Mathf.Abs(player.y - pupae.y) >= _jumpCurve.curvePeak) return false;
            Vector2 playerHorizontal = new(player.x, player.z);
            Vector2 pupaeHorizontal = new(pupae.x, pupae.z);
            float distance = Vector2.Distance(playerHorizontal, pupaeHorizontal);
            if (distance > _jumpCurve.range || distance < _jumpCurve.range / 2) return false;
            if (!CheckLineOfSightForPosition(targetPlayer.transform.position)) return false;

            // SAMPLE PLAYER POSITION IN CURVE WITH FindX()
            if (!NavMesh.SamplePosition(player, out NavMeshHit hit, maxDistance: 10f, NavMesh.AllAreas)) return false;
            leapTarget = hit.position;
            leapStart = pupae;
            leapTime = 0f;
            targetTime = _jumpCurve.FindX(Mathf.Abs(player.y - pupae.y), JumpCurve.SEGMENT.SECOND);
            return true;
        }

        /// <summary>
        /// Behaviour entry point.
        /// </summary>
        private void StartLeap() {
            SwitchToBehaviourClientRpc((int)State.LEAPING);
            inSpecialAnimation = true;
            jumpState = JUMP_STATE.PREPARING;
            DoAnimationClientRpc(State.LEAPING);
        }

        /// <summary>
        /// Second step of the behaviour.
        /// Prepares all the physics properties that allow for the
        /// correct execution of the LeapAI and LeapPhysics methods.
        /// </summary>
        [ClientRpc]
        public void LeapClientRpc() {
            jumpState = JUMP_STATE.JUMPING;
            inSpecialAnimation = false;
            timeSinceLeap = 0f;
            agent.enabled = false;
        }

        /// <summary>
        /// AI portion of the behaviour. Runs periodically every AIInterval.
        /// Disabled when inSpecialAnimation = true;
        /// </summary>
        private void LeapAI() {
            if (jumpState == JUMP_STATE.LANDED) {
                if (squadState == (int)SquadState.CHASING) { StartChase(); }
                else { StartRoam(); }
            }
        }

        private void LeapUpdate() {
            switch (jumpState) {
                case JUMP_STATE.JUMPING:
                    LeapPhysics();
                    break;
                case JUMP_STATE.PREPARING:
                    LookAtPlayer();
                    break;
            }
        }

        private void LookAtPlayer() {
            if (targetPlayer == null) return;
            Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8);
        }

        private void LeapPhysics() {
            agent.enabled = false;
            leapTime += Time.deltaTime;
            float newY = leapStart.y + _jumpCurve.Evaluate(leapTime);
            Vector3 newPos = leapStart + (leapTarget - leapStart) / leapTime;
            newPos.y = newY;
            transform.position = newPos;
            if (leapTime >= targetTime || Vector3.Distance(newPos, leapTarget) < 1f) {
                EndLeapClientRpc();
            }

            // if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, maxDistance: 10f, NavMesh.AllAreas)) {
            //     Debug.DrawLine(transform.position, hit.position, Color.cyan);
            //     height = transform.position.y - hit.position.y;
            // }
            // timeSinceLeap += Time.deltaTime;
            // rb.velocity += new Vector3(0, Mathf.Lerp(0, Physics.gravity.y, _jumpCurve.Evaluate(timeSinceLeap)), 0);
            // Ray ray = new Ray(transform.position, rb.velocity);
            // if (Physics.Raycast(ray, out RaycastHit forwardHit, 1f, LayerMask.GetMask("NavigationSurface", "Terrain", "Room"))
            //     && forwardHit.distance < 0.4) { EndLeapClientRpc(); return; }
            // if ((timeSinceLeap > AIIntervalTime
            //     && height < startingHeight)
            //     || timeSinceLeap > 1.4f) {
            //     EndLeapClientRpc();
            // }
        }

        /// <summary>
        /// Last step before changing states.
        /// </summary>
        [ClientRpc]
        private void EndLeapClientRpc() {
            jumpState = JUMP_STATE.LANDED;
            if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, maxDistance: 10f, NavMesh.AllAreas)) { transform.position = leapStart; }
            agent.enabled = true;
            agent.Warp(transform.position);
            inSpecialAnimation = false;
            timeSinceLeap = 0;
        }
    }
}
