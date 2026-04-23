using UnityEngine;

namespace SkaarjPupae
{
    partial class PupaeAI : EnemyAI
    {
        /// <summary>
        /// Behaviour entry point.
        /// </summary>
        public void StartChase()
        {
            SwitchToBehaviourClientRpc((int)State.CHASING);
            // Non-special NavMesh movement
            agent.enabled = true;
            inSpecialAnimation = false;

            if (isLeader)
            { UpdateSquadState((int)SquadState.CHASING); }
            DoAnimationClientRpc("Chase");
        }

        /// <summary>
        /// AI portion of the behaviour. Runs periodically every AIInterval.
        /// Called when in a group. Disabled when inSpecialAnimation = true;
        /// </summary>
        private void ChaseAI()
        {
            // If group is in roaming state, roam.
            if (squadState == (int)SquadState.ROAMING)
            {
                StartRoam();
                return;
            }

            // Leader checks if pupaes have target in range.
            if (isLeader && (!IsTargetInRange() || targetPlayer.isPlayerDead))
            {
                UpdateSquadState((int)SquadState.ROAMING);
                StartRoam();
                return;
            }

            if (timeSinceLeap < _leapCooldown)
            { timeSinceLeap += AIIntervalTime; }

            // If close enough to target, leap.
            if (targetPlayer != null)
            {
                if (Vector3.Distance(transform.position, targetPlayer.transform.position) < 10
                    && Vector3.Distance(transform.position, targetPlayer.transform.position) > 5
                    && timeSinceLeap >= _leapCooldown
                    && targetPlayer.transform.position.y - transform.position.y < 2
                    && CheckLineOfSightForPosition(targetPlayer.transform.position))
                {
                    StartLeap();
                    return;
                }

                // If target is still in range:
                agent.speed = 6f;
                SetDestinationToPosition(targetPlayer.transform.position);
            }
        }
    }
}