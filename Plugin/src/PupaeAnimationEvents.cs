using UnityEngine;
using Random = UnityEngine.Random;

namespace SkaarjPupae
{
    class PupaeAnimationEvents : MonoBehaviour
    {
        [SerializeField]
        private PupaeAI mainAI;

        public void Chase()
        { mainAI.StartChase(); }

        public void EndLeapAnimation()
        { mainAI.LeapClientRpc(); }

        public void EndSurveillance()
        { mainAI.EndSurveillance(); }
    }
}
