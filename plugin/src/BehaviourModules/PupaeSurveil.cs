using UnityEngine;

namespace SkaarjPupae
{
    partial class PupaeAI : EnemyAI
    {
        /// <summary>
        /// Method that starts surveilling animation, stopping the
        /// agent until animation finishes.
        /// </summary>
        private void StartSurveillance()
        {
            SwitchToBehaviourClientRpc((int)State.SURVEILLING);
            agent.speed = 0f;
            if (isLeader)
            { UpdateSquadState((int)SquadState.SURVEILLING); }
            DoAnimationClientRpc("Surveil");
        }


        private void StartFollowerSurveillance()
        {
            SwitchToBehaviourClientRpc((int)State.SURVEILLING);
            if (Random.Range(0f, 1f) > 0.4)
            {
                agent.speed = 0f;
                DoAnimationClientRpc("Surveil");
            }
            else
            {
                Vector3 targetRandomPosition = squadLeader.transform.position + Vector3.Cross(squadLeader.transform.position - transform.position, Vector3.up).normalized * 7;
                SetDestinationToPosition(targetRandomPosition);
                agent.speed = 2f;
                SetCrawlingSpeed();
            }
        }

        /// <summary>
        /// AI portion of the behaviour. Runs periodically every AIInterval.
        /// Disabled when inSpecialAnimation = true;
        /// </summary>
        private void SurveilAI()
        {
            // If unable to find target, start to roam.
            if (FoundClosestPlayerInRange(12f, 10f))
            {
                PlayerSpotted(this);
                return;
            }

            if (squadState == (int)SquadState.SPOTTED)
            { StartSpot(); return; }
            else if (squadState == (int)SquadState.ROAMING)
            { StartRoam(); return; }
        }

        /// <summary>
        /// Called when animation finishes. Restores agent movement.
        /// </summary>
        public void EndSurveillance()
        {
            if (isLeader)
            { UpdateSquadState((int)SquadState.ROAMING); }
        }
    }
}