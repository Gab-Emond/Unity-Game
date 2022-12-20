
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{

    public bool gameHasEnded = false;

    public GameObject CompleteLevelUI;

    public float restartDelay = 1f;
    
    public void CompleteLevel(){

        CompleteLevelUI.SetActive(true);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);


    }

    public void EndGame()
    {

        if(gameHasEnded == false){
            Debug.Log("game over");
            
            gameHasEnded = true;
            
            Invoke("Restart", restartDelay);

        }
    }

    private void Restart(){

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
