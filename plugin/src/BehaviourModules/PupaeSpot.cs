using UnityEngine;

namespace SkaarjPupae
{
    partial class PupaeAI : EnemyAI
    {
        /// <summary>
        /// Group's behaviour entry point.
        /// Alerts group of target and enters state.
        /// </summary>
        /// <param name="sender"></param>
        public void PlayerSpotted(PupaeAI sender)
        {
            UpdateSquadState((int)SquadState.SPOTTED);
            UpdateSquadTarget(sender.targetPlayer);
            StartSpot();
        }

        /// <summary>
        /// Behaviour entry point.
        /// </summary>
        public void StartSpot()
        {
            SwitchToBehaviourClientRpc((int)State.SPOTTED);
            StopSearch(currentSearch);
            agent.enabled = true;
            inSpecialAnimation = false;
            agent.speed = 0f;
            DoAnimationClientRpc("Spot");
        }

        /// <summary>
        /// Physics portion of the behaviour. Runs every frame.
        /// </summary>
        private void SpotUpdate()
        {
            if (targetPlayer != null)
            {
                Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2);
            }
        }
    }
}