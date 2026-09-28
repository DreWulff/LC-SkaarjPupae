namespace SkaarjPupae.AI {
    partial class PupaeAI : EnemyAI {
        /// <summary>
        /// Behaviour entry point.
        /// </summary>
        public void StartChase() {
            SwitchToBehaviourClientRpc((int)State.CHASING);
            // Non-special NavMesh movement
            agent.enabled = true;
            inSpecialAnimation = false;

            if (isLeader) {
                UpdateSquadState(SquadState.CHASING);
            }
            DoAnimationClientRpc(State.CHASING);
        }

        /// <summary>
        /// AI portion of the behaviour. Runs periodically every AIInterval.
        /// Called when in a group. Disabled when inSpecialAnimation = true;
        /// </summary>
        private void ChaseAI() {
            // If group is in roaming state, roam.
            if (squadState == (int)SquadState.ROAMING) {
                StartRoam();
                return;
            }

            // Leader checks if pupaes have target in range.
            if (isLeader && (!IsTargetInRange() || targetPlayer.isPlayerDead)) {
                UpdateSquadState(SquadState.ROAMING);
                StartRoam();
                return;
            }

            if (timeSinceLeap < _leapCooldown) {
                timeSinceLeap += AIIntervalTime;
            }

            // Conditions
            if (LeapCondition()) {
                StartLeap();
            } else {
                SetCrawlingSpeed(6f);
                SetDestinationToPosition(targetPlayer.transform.position);
            }
        }
    }
}