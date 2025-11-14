using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquationManager : MonoBehaviour {
    [SerializeField] private TMP_Text _equationText;
    [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private EquationStore _equationStore;
    [SerializeField] private GameObject[] _answerBoxes;

    List<float> _answers = new();
    private float _correctAnswer;

    void Start() {
        StartCoroutine(DelayedStart());
    }
    System.Collections.IEnumerator DelayedStart() {
        while (!_equationStore.IsReady) { yield return null; }
        NewQuestion();
    }
    public void NewQuestion() {
        EquationStore.EquationEntry entry = _equationStore.GetRandomEquation();
        _equationText.text = entry.equation;

        // if loading failed or ran out of data
        if (float.IsNaN(entry.solution)) {
            AssignFallbackAnswers();
            return;
        }

        _correctAnswer = Mathf.Round(entry.solution * 100f) / 100f;

        PrepareAnswers(_correctAnswer);
        ShuffleAnswers();
        AssignAnswersToBoxes();
    }
    void PrepareAnswers(float correct) {
        if (float.IsNaN(correct)) {
            AssignFallbackAnswers();
            return;
        }

        _answers.Clear();
        _answers.Add(correct);
        int answerCount = _answerBoxes.Length;

        bool isInt = Mathf.Approximately(correct, Mathf.Round(correct));
        int intCorrect = Mathf.RoundToInt(correct);

        int attempts = 0;
        while (_answers.Count < answerCount && attempts++ < 200) {
            float cand;

            if (isInt) {
                // use integer offsets if correct is int
                int offset = Random.Range(-10, 11);
                if (offset == 0) { continue; }

                cand = intCorrect + offset;
            }
            else {
                // float offset between -5.00 and +5.00, rounded to 2dp
                float offset = Mathf.Round(Random.Range(-5f, 5f) * 100f) / 100f;
                if (Mathf.Approximately(offset, 0f)) { continue; }

                cand = correct + offset;
                cand = Mathf.Round(cand * 100f) / 100f;
            }

            if (!_answers.Contains(cand)) { _answers.Add(cand); }
        }

        // fallback just in case decoys are somehow still insufficient
        while (_answers.Count < answerCount) {
            float cand;
            if (isInt) {
                cand = intCorrect + Random.Range(-20, 21);
            }
            else {
                cand = correct + Random.Range(-10f, 10f);
                cand = Mathf.Round(cand * 100f) / 100f;
            }
            if (!_answers.Contains(cand)) { _answers.Add(cand); }
        }
    }
    void ShuffleAnswers() {
        System.Random rng = new System.Random();
        int n = _answers.Count;

        while (n > 1) {
            int k = rng.Next(n--);
            float temp = _answers[n];
            _answers[n] = _answers[k];
            _answers[k] = temp;
        }
    }    
    void AssignAnswersToBoxes() {
        for (int i = 0; i < _answerBoxes.Length; i++) {
            var ansComp = _answerBoxes[i].GetComponent<Answer>();
            ansComp.SetNumber(_answers[i]);  // assumes SetNumber(float)
        }
    }
    void AssignFallbackAnswers() {
        _answers.Clear();
        _answers.Add(0f);

        for (int i = 1; i < _answerBoxes.Length; i++) {
            _answers.Add(0f);
        }
        ShuffleAnswers();
        AssignAnswersToBoxes();
    }
    public void OnAnswer(float chosen) {  // callback from answer boxes
        bool correct = Mathf.Approximately(chosen, _correctAnswer);
        int scoreDelta = correct ? 2 : -1;
        _scoreManager.AddScore(scoreDelta);
        NewQuestion();
    }
}
