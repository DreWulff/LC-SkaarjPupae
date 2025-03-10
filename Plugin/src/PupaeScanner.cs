using UnityEngine;

namespace SkaarjPupae
{
    class PupaeScanner : MonoBehaviour
    {
        [SerializeField]
        private PupaeAI mainAI;

        public void OnTriggerEnter(Collider other)
        {
            // Spot other pupaes
            SquadLogic(other);

            //// Spot corpses
            //if (other.CompareTag("Enemy") &&
            //    other.TryGetComponent<EnemyAICollisionDetect>(out EnemyAICollisionDetect collider) &&
            //    collider.mainScript.isEnemyDead == true)
            //{
            //    float level = collider.mainScript.enemyType.PowerLevel;
            //    if (Random.Range(0, mainAI.hunger) > Random.Range(0, 1 / level))
            //    { mainAI.Eat(other.transform); }
            //}
            //if (other.CompareTag("Player") &&
            //    other.TryGetComponent<PlayerControllerB>(out PlayerControllerB player) &&
            //    player.isPlayerDead == true)
            //{
            //    float level = 3;
            //    if (Random.Range(0, mainAI.hunger) > Random.Range(0, 1 / level))
            //    { mainAI.Eat(player.transform); }
            //}
        }

        private void SquadLogic(Collider other)
        {
            if (other.TryGetComponent<PupaeScanner>(out PupaeScanner otherPupae))
            {
                otherPupae.AttemptSquadJoin(mainAI);
            }
        }

        public void AttemptSquadJoin(PupaeAI otherPupae)
        {
            mainAI.CreateSquad(otherPupae);
        }
    }
}
