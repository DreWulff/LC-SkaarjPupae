using UnityEngine;
using TMPro;

namespace SkaarjPupae
{
    class DebugText : MonoBehaviour
    {
        [SerializeField]
        private PupaeAI mainAI;
        private TMP_Text text;

        void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        public void Update()
        {
            text.text = 
                "IS LEADER? " + mainAI.isLeader + "\n" +
                "STATE: " + mainAI.currentBehaviourState.name + "\n" +
                "SQUAD STATE: " + mainAI.squadState + "\n";
        }
    }
}
