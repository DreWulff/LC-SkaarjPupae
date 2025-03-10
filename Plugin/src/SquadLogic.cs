using System;
using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace SkaarjPupae
{
    partial class PupaeAI : EnemyAI
    {
        [HideInInspector] public bool isLeader;

#pragma warning disable CS8618 // Will not be NULL after Start
        [HideInInspector] public PupaeAI[] squad;
        [HideInInspector] public PupaeAI squadLeader;
        [HideInInspector] public int squadState;
        [HideInInspector] public PlayerControllerB? squadTargetPlayer;
#pragma warning restore CS8618

        enum SquadState
        {
            ROAMING,
            SURVEILLING,
            SPOTTED,
            CHASING,
        }
        

        /// <summary>
        /// Called when two pupaes meet.
        /// All pupaes, even when alone, are in a group. This method
        /// calls the Join() method when conditions are met.
        /// </summary>
        /// <param name="other"></param>
        public void CreateSquad(PupaeAI other)
        {
            // This allows pupaes to form groups of up to 6.
            if (squad.Length >= 4) return;
            if (currentBehaviourStateIndex != (int)State.ROAMING) return;
            if (squad.Contains(other)) return;
            if (other.isEnemyDead || this.isEnemyDead) return;
            if (other.GetInstanceID() > this.GetInstanceID()) return;
            other.Join(squad);

            UpdateSquadState((int)State.ROAMING);
        }

        /// <summary>
        /// Turns the pupae into the group's leader.
        /// </summary>
        /// <param name="n_squad"></param>
        public void BecomeLeader(PupaeAI[] n_squad)
        {
            isLeader = true;
            squadLeader = this;
            UpdateSquad();
        }

        /// <summary>
        /// Removes the pupae from the group and assigns a new leader
        /// if necessary.
        /// </summary>
        /// <param name="remove"></param>
        public void RemovePupae(PupaeAI remove)
        {
            squad = squad.Where(obj => obj.GetInstanceID() != remove.GetInstanceID()).ToArray();
            if (isLeader && squad.Length > 0)
            {
                foreach (PupaeAI pupae in squad)
                {
                    if (pupae.isEnemyDead)
                    { continue; }
                    pupae.BecomeLeader(squad);
                    squadLeader = pupae;
                    squadLeader.squadState = squadState;
                }
            }
            UpdateSquad();
        }

        /// <summary>
        /// Joins two squads together.
        /// </summary>
        /// <param name="n_squad"></param>
        public void Join(PupaeAI[] n_squad)
        {
            squad = squad.Concat(n_squad).ToArray();
            UpdateSquad();
        }

        /// <summary>
        /// Method that updates all squad related variables
        /// for each pupae.
        /// </summary>
        public void UpdateSquad()
        {
            foreach(PupaeAI pupae in squad)
            {
                pupae.isLeader = false;
                pupae.squad = squad;
                pupae.squadLeader = squadLeader;
            }
            squadLeader.isLeader = true;
        }

        /// <summary>
        /// Updates the chase/attack target of the squad.
        /// </summary>
        /// <param name="target"></param>
        public void UpdateSquadTarget(PlayerControllerB target)
        {
            foreach(PupaeAI pupae in squad)
            { pupae.targetPlayer = target; }
        }

        /// <summary>
        /// Updates the AI state of the squad.
        /// </summary>
        /// <param name="state"></param>
        public void UpdateSquadState(int state)
        {
            foreach(PupaeAI pupae in squad)
            { pupae.squadState = state; }
        }

        /// <summary>
        /// Checks if any of the pupaes has the targetPlayer
        /// in range.
        /// </summary>
        /// <returns></returns>
        public bool IsTargetInRange()
        {
            foreach (PupaeAI pupae in squad)
            {
                if (pupae.FindTarget())
                {
                    UpdateSquadTarget(pupae.targetPlayer);
                    return true;
                }
            }
            return false;
        }
    }
}