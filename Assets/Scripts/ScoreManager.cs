using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; 
using TMPro;

public class ScoreManager : MonoBehaviour, IPointerClickHandler {


    public static ScoreManager instance;

    public TextMeshProUGUI points;
    public int score = 0;

    public void Awake() {
        instance = this;
    }

    public void Start() {
        points.text = score.ToString();
    }

    public void addPoints(int clearedLines) {
        score += clearedLines * 10;
        points.text = score.ToString();
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
