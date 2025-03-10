using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace SkaarjPupae
{
    partial class PupaeAI : EnemyAI
    {
#pragma warning disable CS8618 // Required
        [Tooltip("Leaping cooldown.")]
        [SerializeField] private float _leapCooldown;
        [Tooltip("Curve that defines how gravity increases during a leap.")]
        [SerializeField] private AnimationCurve _gravityCurve;
#pragma warning restore CS8618

        private bool jumping = false;
        private bool jumped = false;
        private float height;
        private float startingHeight;
        private float timeSinceLeap;
        private Vector3 startingPosition;

        /// <summary>
        /// Behaviour entry point.
        /// </summary>
        private void StartLeap()
        {
            SwitchToBehaviourClientRpc((int)State.LEAPING);
            inSpecialAnimation = true;
            DoAnimationClientRpc("Leap");
        }

        /// <summary>
        /// Second step of the behaviour.
        /// Prepares all the physics properties that allow for the
        /// correct execution of the LeapAI and LeapPhysics methods.
        /// </summary>
        [ClientRpc]
        public void LeapClientRpc()
        {
            jumping = true;
            jumped = false;
            inSpecialAnimation = false;
            timeSinceLeap = 0f;
            agent.enabled = false;
            rb.isKinematic = false;
            float targetX = targetPlayer.transform.position.x;
            float targetY = targetPlayer.transform.position.y;
            float baseX = transform.position.x;
            float baseY = transform.position.y;

            rb.velocity = (targetPlayer.transform.position - transform.position).normalized * 30f + new Vector3(0, 7f, 0);
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, maxDistance: 10f, NavMesh.AllAreas))
            {
                startingPosition = transform.position;
                startingHeight = Mathf.Max(Mathf.Abs(startingPosition.y - hit.position.y), 0.1f);
            }
        }

        /// <summary>
        /// AI portion of the behaviour. Runs periodically every AIInterval.
        /// Disabled when inSpecialAnimation = true;
        /// </summary>
        private void LeapAI()
        {
            if (!jumping && jumped)
            {
                jumped = false;
                if (squadState == (int)SquadState.CHASING)
                { StartChase(); }
                else
                { StartRoam(); }
            }
        }

        /// <summary>
        /// Physics portion of the behaviour. Runs every frame.
        /// </summary>
        private void LeapUpdate()
        {
            if (jumping && !jumped)
            {
                agent.enabled = false;
                if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, maxDistance: 10f, NavMesh.AllAreas))
                {
                    Debug.DrawLine(transform.position, hit.position, Color.cyan);
                    height = transform.position.y - hit.position.y;
                }
                timeSinceLeap += Time.deltaTime;
                rb.velocity += new Vector3(0, Mathf.Lerp(0, Physics.gravity.y, _gravityCurve.Evaluate(timeSinceLeap)), 0);
                Ray ray = new Ray(transform.position, rb.velocity);
                if (Physics.Raycast(ray, out RaycastHit forwardHit, 1f, LayerMask.GetMask("NavigationSurface", "Terrain", "Room"))
                    && forwardHit.distance < 0.4)
                { EndLeapClientRpc(); return; }
                if ((timeSinceLeap > AIIntervalTime
                    && height < startingHeight)
                    || timeSinceLeap > 1.4f)
                {
                    EndLeapClientRpc();
                }
            }
            if (!jumping)
            {
                if (targetPlayer != null)
                {
                    Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8);
                }
            }
        }

        /// <summary>
        /// Last step before changing states.
        /// </summary>
        [ClientRpc]
        private void EndLeapClientRpc()
        {
            jumping = false;
            jumped = true;
            if (!NavMesh.SamplePosition(transform.position, out NavMeshHit hit, maxDistance: 10f, NavMesh.AllAreas))
            { transform.position = startingPosition; }
            rb.isKinematic = true;
            agent.enabled = true;
            agent.Warp(transform.position);
            inSpecialAnimation = false;
            timeSinceLeap = 0;
        }
    }
}
