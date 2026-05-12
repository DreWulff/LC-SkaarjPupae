using UnityEngine;

namespace SkaarjPupae.AI
{
    class PupaeScanner : MonoBehaviour
    {
        [SerializeField]
        private PupaeAI mainAI = null!;

        public void OnTriggerEnter(Collider other)
        {
            // Spot other pupaes
            SquadLogic(other);
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
