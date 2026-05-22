using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace SkaarjPupae.AI {
    partial class PupaeAI : EnemyAI {
        [Tooltip("Leaping cooldown.")]
        [SerializeField] private float _leapCooldown = 3f;
        [Tooltip("Curve that defines how gravity increases during a leap.")]
        [SerializeField] private AnimationCurve _gravityCurve = null!;

        private bool jumping = false;
        private bool jumped = false;
        private float height;
        private float startingHeight;
        private float timeSinceLeap;
        private Vector3 startingPosition;
        private JUMP_STATE jumpState;

        private enum JUMP_STATE {
            PREPARING,
            JUMPING,
            LANDED,
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
            rb.isKinematic = false;
            float targetX = targetPlayer.transform.position.x;
            float targetY = targetPlayer.transform.position.y;
            float baseX = transform.position.x;
            float baseY = transform.position.y;

            rb.velocity = (targetPlayer.transform.position - transform.position).normalized * 30f + new Vector3(0, 7f, 0);
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, maxDistance: 10f, NavMesh.AllAreas)) {
                startingPosition = transform.position;
                startingHeight = Mathf.Max(Mathf.Abs(startingPosition.y - hit.position.y), 0.1f);
            }
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
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, maxDistance: 10f, NavMesh.AllAreas)) {
                Debug.DrawLine(transform.position, hit.position, Color.cyan);
                height = transform.position.y - hit.position.y;
            }
            timeSinceLeap += Time.deltaTime;
            rb.velocity += new Vector3(0, Mathf.Lerp(0, Physics.gravity.y, _gravityCurve.Evaluate(timeSinceLeap)), 0);
            Ray ray = new Ray(transform.position, rb.velocity);
            if (Physics.Raycast(ray, out RaycastHit forwardHit, 1f, LayerMask.GetMask("NavigationSurface", "Terrain", "Room"))
                && forwardHit.distance < 0.4) { EndLeapClientRpc(); return; }
            if ((timeSinceLeap > AIIntervalTime
                && height < startingHeight)
                || timeSinceLeap > 1.4f) {
                EndLeapClientRpc();
            }
        }

        /// <summary>
        /// Last step before changing states.
        /// </summary>
        [ClientRpc]
        private void EndLeapClientRpc() {
            jumpState = JUMP_STATE.LANDED;
            if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, maxDistance: 10f, NavMesh.AllAreas)) { transform.position = startingPosition; }
            rb.isKinematic = true;
            agent.enabled = true;
            agent.Warp(transform.position);
            inSpecialAnimation = false;
            timeSinceLeap = 0;
        }
    }
}
