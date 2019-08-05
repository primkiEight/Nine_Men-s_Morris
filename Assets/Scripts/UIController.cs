using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    [Header("Language Settings")]
    public Messages InitialLanguage;
    public Messages Language;

    [Header("UI Player Holders")]
    public GameObject Player1Holder;
    public GameObject Player2Holder;
    public Text Player1Text;
    public Text Player2Text;
    public Text Player1StonesText;
    public Text Player2StonesText;
    public Image Player1Image;
    public Image Player2Image;

    [Header("UI Message Info Holders")]
    public Text MessageText;
    public Image MessageImage;

    private void Awake()
    {
        UpdateUIAwake(false);

        Language = InitialLanguage;

        Player1Holder.SetActive(true);
        Player2Holder.SetActive(false);
    }

    public void UpdateUIAwake(bool awake)
    {
        gameObject.GetComponent<Canvas>().enabled = awake;
    }

    //Method is not an option ATM
    //public void ChangeLanguage(Messages newLanguage)
    //{
    //    Language = newLanguage;
    //}

    //Changes Font to match the environment (Environment, GameManager)
    public void ChangeFont(Font newFont)
    {
        Player1Text.font = newFont;
        Player2Text.font = newFont;

        Player1StonesText.font = newFont;
        Player2StonesText.font = newFont;

        MessageText.font = newFont;
    }

    //Sets UI Images to match the environment (GameManager)
    public void ChangeImageSprites(Sprite playerSprite, Sprite messageSprite)
    {
        Player1Image.sprite = playerSprite;
        Player2Image.sprite = playerSprite;
        MessageImage.sprite = messageSprite;
}

    //Displays UI Texts to present the predefined player names (Messages, GameManager)
    public void UpdateUIPlayerNames()
    {
        Player1Text.text = Language.Player1;
        Player2Text.text = Language.Player2;
    }

    //Displays numbers of remaining stones for the players to place (GameManager)
    public void UpdateUIStones(int player1stones, int player2stones)
    {
        Player1StonesText.text = "~ " + player1stones.ToString() + " ~";
        Player2StonesText.text = "~ " + player2stones.ToString() + " ~";
    }

    //Updates UI predefined messages for the players during the game (Messages, GameManager)
    public void UpdateUIMessage(string message)
    {
        MessageText.text = message;
    }

    //Alternates UI holders for player displays each turn (GameManager)
    public void UpdateUIAlternateScreens()
    {
        bool trigger = Player1Holder.activeSelf;
            
        Player1Holder.SetActive(!trigger);
        Player2Holder.SetActive(trigger);
    }

    //Displays both playere screens when the game is over (GameManager)
    public void UpdateUIGameOverScreen()
    {
        Player1Holder.SetActive(true);
        Player2Holder.SetActive(true);
    }

    //Button event - Restarts the scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Button event - Closes the build application
    public void QuitGame()
    {
        Application.Quit();
    }
}
