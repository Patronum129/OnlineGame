using Client;
using GamePlay;
using Model;
using Server;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class WinPanel : MonoBehaviour
    {
        [SerializeField] private Button m_ReturnBtn;
        [SerializeField] private TMP_Text m_Title;

        private bool isInit;
        
        public void Init(string _name)
        {
            if (isInit) return;
            isInit = true;

            AudioManager.Singleton.PlayEnd();
            
            if (_name == "A tie")
            {
                m_Title.text = "A Tie!";
            }
            else if (_name == "Game Over")
            {
                m_Title.text = "Game Over!";
            }
            else
            {
                m_Title.text = $"Player:{_name} Win!";
            }
            
            if (GameModel.IsServer)
            {
                GameModel.IsServer = false;

                ServerManager.Singleton.Close();
            }
            else
            {
                ClientManager.Singleton.Close();
            }

            Time.timeScale = 0;
            
            m_ReturnBtn.onClick.AddListener(OnclickReturnBtn);
        }

        private void OnclickReturnBtn()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }
    }
}