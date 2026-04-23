using UnityEngine;

namespace SkaarjPupae
{
    class PupaeAnimationEvents : MonoBehaviour
    {
        [SerializeField]
        private PupaeAI mainAI = null!;

        public void Chase()
        { mainAI.StartChase(); }

        public void EndLeapAnimation()
        { mainAI.LeapClientRpc(); }

        public void EndSurveillance()
        { mainAI.EndSurveillance(); }
    }
}
