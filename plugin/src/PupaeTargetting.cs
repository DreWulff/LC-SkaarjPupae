using UnityEngine;

namespace SkaarjPupae
{
    partial class PupaeAI : EnemyAI
    {
        /// <summary>
        /// Checks if the current target is within the range of the creature.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool FindTarget(float range = 15f)
        {
            if (!TargetClosestPlayerInAnyCase() ||
                (Vector3.Distance(transform.position, targetPlayer.transform.position) > range && !CheckLineOfSightForPosition(targetPlayer.transform.position))
                || targetPlayer.isPlayerDead)
            { return false; }

            else
            { return true; }
        }


        /// <summary>
        /// Checks if a player is within the sight range or sensing range of the creature.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="senseRange"></param>
        /// <returns></returns>
        bool FoundClosestPlayerInRange(float range, float senseRange)
        {
            TargetClosestPlayer(bufferDistance: 5f, requireLineOfSight: true, viewWidth: 140);
            if (targetPlayer == null)
            {
                // Couldn't see a player, so we check if a player is in sensing distance instead
                TargetClosestPlayer(bufferDistance: 5f, requireLineOfSight: false);
                range = senseRange;
            }
            return targetPlayer != null && Vector3.Distance(transform.position, targetPlayer.transform.position) < range && !targetPlayer.isPlayerDead;
        }

        /// <summary>
        /// Function to find the closest player.
        /// </summary>
        /// <returns></returns>
        bool TargetClosestPlayerInAnyCase()
        {
            mostOptimalDistance = 2000f;
            targetPlayer = null;
            for (int i = 0; i < StartOfRound.Instance.connectedPlayersAmount + 1; i++)
            {
                tempDist = Vector3.Distance(transform.position, StartOfRound.Instance.allPlayerScripts[i].transform.position);
                if (tempDist < mostOptimalDistance)
                {
                    mostOptimalDistance = tempDist;
                    targetPlayer = StartOfRound.Instance.allPlayerScripts[i];
                }
            }
            if (targetPlayer == null || targetPlayer.isPlayerDead) return false;
            return true;
        }
    }
}
