using UnityEngine;

namespace SkaarjPupae
{
    partial class PupaeAI : EnemyAI
    {
        private float followDelay = 0;
        private float surveillanceTimer = 0;

        /// <summary>
        /// Behaviour entry point.
        /// </summary>
        private void StartRoam()
        {
            SwitchToBehaviourClientRpc((int)State.ROAMING);
            agent.enabled = true;
            inSpecialAnimation = false;
            timeSinceLeap = _leapCooldown;
            DoAnimationClientRpc("Crawl");
            if (isLeader)
            { UpdateSquadState((int)SquadState.ROAMING); }
            else
            {
                StartFollow();
                return;
            }
            surveillanceTimer = Random.Range(10f, 25f);
            StartSearch(transform.position);
        }

        /// <summary>
        /// In case the pupae is not the leader, this is called, assigning
        /// the leader as the actor's target.
        /// </summary>
        private void StartFollow()
        {
            followDelay = Random.Range(1, 4);
            StopSearch(currentSearch);
            SetDestinationToPosition(squadLeader.transform.position);
            LogIfDebugBuild("Following leader!");
        }

        /// <summary>
        /// AI portion of the behaviour. Runs periodically every AIInterval.
        /// Disabled when inSpecialAnimation = true;
        /// </summary>
        private void RoamAI()
        {
            if (FoundClosestPlayerInRange(10f, 10f))
            { 
                PlayerSpotted(this);
                return;
            }
            if (squadState == (int)SquadState.SPOTTED)
            { StartSpot(); return; }
            if (surveillanceTimer > 0)
            { surveillanceTimer -= AIIntervalTime; }
            if (surveillanceTimer <= 0)
            { StartSurveillance(); }
            else
            { 
                agent.speed = 3f;
                SetCrawlingSpeed();
            }
        }

        /// <summary>
        /// AI portion of the behaviour. Runs periodically every AIInterval.
        /// Called when not the leader. Disabled when inSpecialAnimation = true;
        /// </summary>
        private void RoamFollowerAI()
        {
            // If pupae spots a player, alert group.
            if (FoundClosestPlayerInRange(10f, 5f))
            {
                Debug.Log("Spotted player! Alerting group!");
                PlayerSpotted(this);
                return;
            }

            if (followDelay <= 0)
            {
                if (squadState == (int)SquadState.SURVEILLING)
                { StartFollowerSurveillance(); }
                else if (squadState == (int)SquadState.SPOTTED)
                { StartSpot(); }
                else
                { FollowLeader(); }
                followDelay = Random.Range(1, 11);
            }
            else
            { followDelay -= 1; }
        }

        /// <summary>
        /// Follower's agent module.
        /// </summary>
        private void FollowLeader()
        {
            if (Vector3.Distance(squadLeader.transform.position, transform.position) > 7)
            {
                SetDestinationToPosition(squadLeader.transform.position);
                agent.speed = 4.5f;
                SetCrawlingSpeed();
            }
            else
            {
                if (squadLeader.currentSearch.currentTargetNode != null)
                { SetDestinationToPosition(squadLeader.currentSearch.currentTargetNode.transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f))); }
                else
                { SetDestinationToPosition(squadLeader.transform.position); }
                agent.speed = 2.5f + Random.Range(-0.5f, 0.5f);
                SetCrawlingSpeed();
            }
        }

        /// <summary>
        /// Sets the animation speed based on the speed of the agent.
        /// </summary>
        private void SetCrawlingSpeed()
        { SetAnimationParameterClientRpc("CrawlSpeed", agent.speed / 3f); }
    }
}