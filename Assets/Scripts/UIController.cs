using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    public Messages InitialLanguage;
    public Messages Language;

    public GameObject Player1Holder;
    public GameObject Player2Holder;

    public Text Player1Text;
    public Text Player2Text;
    
    public Text Player1StonesText;
    public Text Player2StonesText;

    public Image Player1Image;
    public Image Player2Image;

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

    public void ChangeLanguage(Messages newLanguage)
    {
        Language = newLanguage;
    }

    public void ChangeFont(Font newFont)
    {
        Player1Text.font = newFont;
        Player2Text.font = newFont;

        Player1StonesText.font = newFont;
        Player2StonesText.font = newFont;

        MessageText.font = newFont;
    }

    public void ChangeImageSprites(Sprite playerSprite, Sprite messageSprite)
    {
        Player1Image.sprite = playerSprite;
        Player2Image.sprite = playerSprite;
        MessageImage.sprite = messageSprite;
}

    public void UpdateUIPlayerNames()
    {
        Player1Text.text = Language.Player1;
        Player2Text.text = Language.Player2;
    }

    public void UpdateUIStones(int player1stones, int player2stones)
    {
        Player1StonesText.text = "- " + player1stones.ToString() + " -";
        Player2StonesText.text = "- " + player2stones.ToString() + " -";
    }

    public void UpdateUIMessage(string message)
    {
        MessageText.text = message;
    }

    public void UpdateUIAlternateScreens()
    {
        bool trigger = Player1Holder.activeSelf;
            
        Player1Holder.SetActive(!trigger);
        Player2Holder.SetActive(trigger);
    }

    public void UpdateUIGameOverScreen()
    {
        Player1Holder.SetActive(true);
        Player2Holder.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
