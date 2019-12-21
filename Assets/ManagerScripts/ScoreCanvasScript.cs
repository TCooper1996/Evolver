using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace ManagerScripts
{
    public class ScoreCanvasScript : MonoBehaviour
    {
        public Text depth;
        public Text damageTaken;
        public Text damageDealt;
        public Text amountHealed;
        
        private AnalyticsTracker info;

        private void Start()
        {
            SoundScript.PlaySound(SoundScript.Sound.Death);
            info = AnalyticsTracker.instance;

            depth.text = $"Depth\n {info.depth}";
            damageDealt.text = $"Damage Dealt\n {info.damageDealt}";
            damageTaken.text = $"Damage Taken\n {info.damageTaken}";
            amountHealed.text = $"Amount Healed\n {info.amountHealed}";

        }

        public void ExitToMainMenu()
        {
            SceneManager.LoadScene("Scenes/MainMenu", LoadSceneMode.Single);
        }
    }
}