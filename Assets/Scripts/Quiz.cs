using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Quiz : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO>();
    [SerializeField] QuestionSO currentQuestion;

    [Header("Answers")]
    [SerializeField] GameObject[] AnswerButtons;
    int CorrectAnswerIndex;
    bool hasAnsweredEarly = true;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite CorrectAnswerSprite;
    private Image buttonImage;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("ProgressBar")]
    [SerializeField] Slider progressBar;

    public bool isComplete;

    void Awake()
    {
        timer = FindObjectOfType<Timer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;
    }

    void Update()
    {
        timerImage.fillAmount = timer.fillFraction;
        if (timer.loadNextQuestion)
        {
            if (progressBar.value == progressBar.maxValue)
            {
                isComplete = true;
                return;
            }

            hasAnsweredEarly = true;
            GetNextQuestion();
            timer.loadNextQuestion = false;
        }
        else if (!hasAnsweredEarly && !timer.isAnsweringQuestion)
        {
            DisplayAnswer(-1);
            SetButtonState(false);
        }
    }

    public void OnAnswerSelected(int index)
    {
        hasAnsweredEarly = true;
        DisplayAnswer(index);
        SetButtonState(false);
        timer.CancelTimer();
        scoreText.text = "Score: " + scoreKeeper.CalculateScore() + "%";
    }

    void DisplayAnswer(int index)
    {
        Image buttonImage;

        if (index == currentQuestion.GetCorrectAnswerIndex())
        {
            questionText.text = "Correct!";
            buttonImage = AnswerButtons[index].GetComponent<Image>();
            buttonImage.sprite = CorrectAnswerSprite;
            scoreKeeper.IncrementCorrectAnswers();
        }
        else
        {
            CorrectAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
            string CorrectAnswer = currentQuestion.GetAnswer(CorrectAnswerIndex);
            questionText.text = "Sorry, the correct answer was;\n " + CorrectAnswer;
            buttonImage = AnswerButtons[CorrectAnswerIndex].GetComponent<Image>();
            buttonImage.sprite = CorrectAnswerSprite;
        }
    }

    void GetNextQuestion()
    {
        if(questions.Count > 0)
        {
            SetButtonState(true);
            SetDefaultButtonSprites();
            GetRandomQuestion();
            DisplayQuestion();
            progressBar.value++;
            scoreKeeper.IncrementQuestionsSeen();
        }
    }
        
    void GetRandomQuestion()
    {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];


        if (questions.Contains(currentQuestion))
        {
            questions.Remove(currentQuestion);
        }
        questions.Remove(currentQuestion);
    }

    void DisplayQuestion()
    {
        questionText.text = currentQuestion.GetQuestion();

        for (int i = 0; i < AnswerButtons.Length; i++)
        {
            TextMeshProUGUI buttontext = AnswerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttontext.text = currentQuestion.GetAnswer(i);
        }
    }

    void SetButtonState(bool state)
    {
        for (int i = 0; i < AnswerButtons.Length; i++)
        {
            Button button = AnswerButtons[i].GetComponent<Button>();
            button.interactable = state;
        }
    }

    void SetDefaultButtonSprites()
    {
        for(int i = 0; i < AnswerButtons.Length; i++)
        {
            Image buttonImage = AnswerButtons[i].GetComponent<Image>();
            buttonImage.sprite = defaultAnswerSprite;
        }
    }


}
