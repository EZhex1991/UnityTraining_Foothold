/*
 * Author:      #AUTHORNAME#
 * CreateTime:  #CREATETIME#
 * Description:
 * 
*/
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EZhex1991
{
    public class LevelController : MonoBehaviour
    {
        public PlayerController controller;
        public GUIText text_Restart;
        public GUIText text_BrickCount;

        private GameObject[] holders;
        private GameObject[] grounds;
        private GameObject[] horizon;

        private int holderTouches = 0;
        private int groundsTouches = 0;

        void Start()
        {
            if (controller == null)
            {
                controller = GameObject.FindObjectOfType<PlayerController>();
            }
            controller.onStuck += ShowRestart;
            controller.onBrickCountChanged += RefreshBrickCount;

            text_Restart.text = "";
            text_BrickCount.text = "";

            holders = GameObject.FindGameObjectsWithTag("Holder");
            for (int i = 0; i < holders.Length; i++)
            {
                holders[i].GetComponent<Ground>().onTriggerValueChanged += OnHolderTouch;
            }
            grounds = GameObject.FindGameObjectsWithTag("Ground");
            for (int i = 0; i < grounds.Length; i++)
            {
                grounds[i].GetComponent<Ground>().onTriggerValueChanged += OnGroundTouch;
            }
            horizon = GameObject.FindGameObjectsWithTag("Horizon");
            for (int i = 0; i < horizon.Length; i++)
            {
                horizon[i].GetComponent<Trigger>().onValueChanged += GameOver;
            }
        }

        void ShowRestart()
        {
            text_Restart.text = "Press 'R' to Restart.";
        }
        void RefreshBrickCount(int count)
        {
            text_BrickCount.text = "Bricks: " + count;
        }

        void OnHolderTouch(bool isTouching)
        {
            if (isTouching)
            {
                holderTouches++;
                CheckStatus();
            }
            else
            {
                holderTouches--;
            }
        }
        void OnGroundTouch(bool isTouching)
        {
            if (isTouching)
            {
                groundsTouches++;
            }
            else
            {
                groundsTouches--;
                CheckStatus();
            }
        }
        void CheckStatus()
        {
            print(string.Format("{0}, {1}", groundsTouches, holderTouches));
            if (groundsTouches == 0 && holderTouches == holders.Length)
            {
                controller.canMove = false;
                print("OK");
                int index = SceneManager.GetActiveScene().buildIndex + 1;
                if (index <= SceneManager.sceneCountInBuildSettings)
                    StartCoroutine(Cor_StartLevel(SceneManager.GetActiveScene().buildIndex + 1));
            }
        }

        public void GameOver(bool isTouching)
        {
            controller.canMove = false;
            print("Game Over");
            StartCoroutine(Cor_RestartLevel());
        }
        IEnumerator Cor_RestartLevel()
        {
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        IEnumerator Cor_StartLevel(int index)
        {
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(index);
        }
    }
}