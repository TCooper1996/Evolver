using UnityEngine;

namespace ManagerScripts
{
    public class MacGuffinScript : MonoBehaviour
    {
        [SerializeField]
        private Animator animiator;
        // Start is called before the first frame update
        void Start()
        {
            if (DirectorScript.IsMacGuffinObtained)
            {
                Destroy(gameObject);
                return;
            }
        }

        // Update is called once per frame
    }
}
