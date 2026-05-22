using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace SkaarjPupae.AI {
    [RequireComponent(typeof(Rigidbody))]
    partial class PupaeAI : EnemyAI {
        [Tooltip("Cooldown for damage instances.")]
        [SerializeField] private float damageCooldown = 1f;
        [Tooltip("Damage done to the player when hit during a leap.")]
        [SerializeField] private int leapDamage = 10;

        private float timeSinceDamagingPlayer;
        [HideInInspector]
        public Rigidbody rb = null!;
        public enum State {
            ROAMING,
            SURVEILLING,
            SPOTTED,
            CHASING,
            LEAPING,
        }

        public override void Start() {
            base.Start();
            float randomSize = Random.Range(2f, 3f);
            gameObject.transform.transform.localScale *= randomSize;
            rb = gameObject.GetComponent<Rigidbody>();
            timeSinceDamagingPlayer = damageCooldown;
            squad = [this];
            isLeader = true;
            squadLeader = this;
            inSpecialAnimation = true;
        }

        public void FinishSpawn() {
            inSpecialAnimation = false;
            StartRoam();
        }

        // Runs every frame.
        public override void Update() {
            base.Update();
            // Make sure leap finished even in death.
            if (isEnemyDead && jumpState != JUMP_STATE.JUMPING) {
                return;
            }

            if (timeSinceDamagingPlayer < damageCooldown) {
                timeSinceDamagingPlayer += Time.deltaTime;
            }

            switch (currentBehaviourStateIndex) {
                case (int)State.ROAMING:
                case (int)State.SURVEILLING:
                case (int)State.CHASING:
                    break;
                case (int)State.SPOTTED:
                    SpotUpdate();
                    break;
                case (int)State.LEAPING:
                    LeapUpdate();
                    break;
            }
        }

        // Runs every AIInterval. Disabled if inSpecialAnimation = true.
        public override void DoAIInterval() {

            base.DoAIInterval();
            if (isEnemyDead || StartOfRound.Instance.allPlayersDead) {
                return;
            }

            // Behaviour when in a squad.
            switch (currentBehaviourStateIndex) {
                case (int)State.ROAMING:
                    if (isLeader) RoamAI();
                    else RoamFollowerAI();
                    break;
                case (int)State.SURVEILLING:
                    SurveilAI();
                    break;
                case (int)State.SPOTTED:
                    return;
                case (int)State.CHASING:
                    ChaseAI();
                    return;
                case (int)State.LEAPING:
                    LeapAI();
                    return;
            }
        }

        public override void HitEnemy(int force = 1, PlayerControllerB? playerWhoHit = null, bool playHitSFX = false, int hitID = -1) {
            base.HitEnemy(force, playerWhoHit, playHitSFX, hitID);
            if (isEnemyDead) { return; }
            enemyHP -= force;
            if (IsOwner) {
                if (enemyHP <= 0 && !isEnemyDead) {
                    // Our death sound will be played through creatureVoice when KillEnemy() is called.
                    // KillEnemy() will also attempt to call creatureAnimator.SetTrigger("KillEnemy"),
                    // so we don't need to call a death animation ourselves.
                    KillEnemyOnOwnerClient();
                }
            }
        }

        public override void KillEnemy(bool destroy = false) {
            base.KillEnemy(destroy);
            RemovePupae(this);
            creatureSFX.enabled = false;
            creatureVoice.enabled = false;
        }

        /// <summary>
        /// Damage the player on collision.
        /// </summary>
        /// <param name="other"></param>
        public override void OnCollideWithPlayer(Collider other) {
            PlayerControllerB playerControllerB = MeetsStandardPlayerCollisionConditions(other);
            if (playerControllerB != null && timeSinceDamagingPlayer >= damageCooldown) {
                playerControllerB.DamagePlayer(leapDamage);
                timeSinceDamagingPlayer = 0f;
            }
        }

        [ClientRpc]
        public void DoAnimationClientRpc(State animationName) {
            creatureAnimator.SetIntegerString("State", (int)animationName);
        }

        [ClientRpc]
        public void SetAnimationParameterClientRpc(string parameter, float value) {
            creatureAnimator.SetFloat(parameter, value);
        }
    }
}