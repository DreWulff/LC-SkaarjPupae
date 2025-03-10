using System.Diagnostics;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace SkaarjPupae {

    [RequireComponent(typeof(Rigidbody))]
    partial class PupaeAI : EnemyAI
    {
#pragma warning disable 0649, CS8618
        [Tooltip("Cooldown for damage instances.")]
        [SerializeField] private float damageCooldown;
        [Tooltip("Damage done to the player when hit during a leap.")]
        [SerializeField] private int leapDamage;

        private float timeSinceDamagingPlayer;
        [HideInInspector]
        public Rigidbody rb;
#pragma warning restore 0649, CS8618
        enum State {
            ROAMING,
            SURVEILLING,
            SPOTTED,
            CHASING,
            LEAPING,
        }

        [Conditional("DEBUG")]
        void LogIfDebugBuild(string text)
        {
            Plugin.Logger.LogInfo(text);
        }

        public override void Start()
        {
            base.Start();
            float randomSize = _gravityCurve.Evaluate(Random.Range(0f, 1f)) * 0.3f + 0.9f;
            if (randomSize > 1)
            {
                enemyHP = (int)(enemyHP * 1.5f);
                leapDamage = (int)(leapDamage * 1.5f);
            }
            gameObject.transform.transform.localScale = new Vector3(randomSize, randomSize, randomSize);
            rb = gameObject.GetComponent<Rigidbody>();
            timeSinceDamagingPlayer = damageCooldown;
            squad = [this];
            isLeader = true;
            squadLeader = this;
            LogIfDebugBuild("Skaarj Pupae Spawned");
            StartRoam();
        }
        
        // Runs every frame.
        public override void Update() {
            base.Update();
            // Make sure leap finished even in death.
            if (isEnemyDead && !jumping)
            { return; }
            // Cooldown timer for damaging a player
            if (timeSinceDamagingPlayer < damageCooldown)
            { timeSinceDamagingPlayer += Time.deltaTime; }
            // Physics/visual updates depending on state.
            switch (currentBehaviourStateIndex)
            {
                case (int)State.ROAMING:
                    break;
                case (int)State.SURVEILLING:
                    break;
                case (int)State.SPOTTED:
                    SpotUpdate();
                    break;
                case (int)State.CHASING:
                    break;
                case (int)State.LEAPING:
                    LeapUpdate();
                    break;
                default:
                    break;
            }
        }

        // Runs every AIInterval. Disabled if inSpecialAnimation = true.
        public override void DoAIInterval() {
            
            base.DoAIInterval();
            if (isEnemyDead || StartOfRound.Instance.allPlayersDead)
            { return; };

            // Behaviour when in a squad.
            switch (currentBehaviourStateIndex)
            {
                // When roaming, if player is spotted, enter SPOTTED state.
                case (int)State.ROAMING:
                    if (isLeader) RoamAI();
                    else RoamFollowerAI();
                    break;

                // When roaming, if player is spotted, enter SPOTTED state.
                case (int)State.SURVEILLING:
                    SurveilAI();
                    break;

                // When spotted a player, wait for animation to end
                // to attempt a chase. (event in Animation)
                case (int)State.SPOTTED:
                    return;

                // When CHASING, move towards the player and attempt
                // a leap if close enough.
                case (int)State.CHASING:
                    ChaseAI();
                    return;

                // When LEAPING, attempt a chase if a target is close enough, or
                // roam otherwise.
                case (int)State.LEAPING:
                    LeapAI();
                    return;

                default:
                    break;
            }
        }

        public override void HitEnemy(int force = 1, PlayerControllerB? playerWhoHit = null, bool playHitSFX = false, int hitID = -1) {
            base.HitEnemy(force, playerWhoHit, playHitSFX, hitID);
            if(isEnemyDead)
            { return; }
            enemyHP -= force;
            if (IsOwner) {
                if (enemyHP <= 0 && !isEnemyDead) {
                    // Our death sound will be played through creatureVoice when KillEnemy() is called.
                    // KillEnemy() will also attempt to call creatureAnimator.SetTrigger("KillEnemy"),
                    // so we don't need to call a death animation ourselves.

                    // We need to stop our search coroutine, because the game does not do that by default.
                    StopCoroutine(searchCoroutine);
                    KillEnemyOnOwnerClient();
                }
            }
        }

        public override void KillEnemy(bool destroy = false)
        {
            base.KillEnemy(destroy);
            RemovePupae(this);
            creatureSFX.enabled = false;
            creatureVoice.enabled = false;
        }

        /// <summary>
        /// Damage the player on collision.
        /// </summary>
        /// <param name="other"></param>
        public override void OnCollideWithPlayer(Collider other)
        {
            PlayerControllerB playerControllerB = MeetsStandardPlayerCollisionConditions(other);
            if (playerControllerB != null && timeSinceDamagingPlayer >= damageCooldown)
            {
                playerControllerB.DamagePlayer(leapDamage);
                timeSinceDamagingPlayer = 0f;
            }
        }

        [ClientRpc]
        public void DoAnimationClientRpc(string animationName)
        { creatureAnimator.SetTrigger(animationName); }

        [ClientRpc]
        public void SetAnimationParameterClientRpc(string parameter, float value)
        { creatureAnimator.SetFloat(parameter, value); }
    }
}