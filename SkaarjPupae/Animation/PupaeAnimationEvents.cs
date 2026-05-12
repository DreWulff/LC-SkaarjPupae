using UnityEngine;
using SkaarjPupae.AI;

namespace SkaarjPupae.Animation
{
    class PupaeAnimationEvents : MonoBehaviour
    {
        [SerializeField]
        private PupaeAI mainAI = null!;

        public void Chase()
        {
            mainAI.StartChase();
        }

        public void EndLeapAnimation()
        {
            mainAI.LeapClientRpc();
        }

        public void EndSurveillance()
        {
            mainAI.EndSurveillance();
        }

        public void EndSpawn()
        {
            mainAI.FinishSpawn();
        }
    }
}
