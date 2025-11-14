using System;
using UnityEngine;
using TMPro;

[Obsolete("Use newer EquationManager")]
public class OldQuestionManager : MonoBehaviour {
    [SerializeField] Vector2Int _lhsBaseRange = new(1, 10);
    [SerializeField] Vector2Int _rhsBaseRange = new(1, 10);
    [SerializeField] OperatorFlags _opFlags = new(true, true, false, false);
    [SerializeField] GameObject[] _answerBoxes;
    [SerializeField] TMP_Text _questionText;
    [SerializeField] ScoreManager _scoreManager;

    [NonSerialized] public int answer;
    Vector2Int _lhsRange = new(1, 10);
    Vector2Int _rhsRange = new(1, 10);
    int[] _answers;
    float _difficulty = 1;

    enum Operator { Add, Sub, Mul, Div };
    enum VarPos { Lhs, Rhs, Answer };

    [Serializable] struct OperatorFlags {
        public bool add, sub, mul, div;
        public OperatorFlags(bool add, bool sub, bool mul, bool div) {
            this.add = add; this.sub = sub; this.mul = mul; this.div = div;
        }
    };
    struct Equation {
        public int lhs, rhs, answer;
        public Operator op;
        public VarPos varPos;
        public Equation(int lhs, int rhs, int answer, Operator op, VarPos varPos) {
            this.lhs = lhs; this.rhs = rhs; this.answer = answer; this.op = op; this.varPos = varPos;
        }
    };

    void Start() {
        NewQuestion();
    }
    int RandOperand(Vector2Int range) {
        return UnityEngine.Random.Range(range.x, range.y);
    }
    Operator RandOperator() {
        int enumLength = Enum.GetNames(typeof(Operator)).Length;
        Operator? op = null;
        while (op == null) {
            op = (Operator)UnityEngine.Random.Range(0, enumLength);
            op = op switch {
                Operator.Add => _opFlags.add ? op : null,
                Operator.Sub => _opFlags.sub ? op : null,
                Operator.Mul => _opFlags.mul ? op : null,
                Operator.Div => _opFlags.div ? op : null,
                _ => throw new Exception("Unexpected Operator value")
            };
        }
        return (Operator)op;
    }
    int EvalEquation(Equation eq) {
        return eq.varPos switch {
            VarPos.Lhs => eq.op switch {
                Operator.Add => eq.answer - eq.rhs,
                Operator.Sub => eq.answer + eq.rhs,
                Operator.Mul => eq.answer / eq.rhs,
                Operator.Div => eq.answer * eq.rhs,
                _ => throw new Exception("Unexpected Operator value")
            },
            VarPos.Rhs => eq.op switch {
                Operator.Add => eq.answer - eq.lhs,
                Operator.Sub => eq.lhs - eq.answer,
                Operator.Mul => eq.answer / eq.lhs,
                Operator.Div => eq.lhs / eq.answer,
                _ => throw new Exception("Unexpected Operator value")
            },
            VarPos.Answer => eq.op switch {
                Operator.Add => eq.lhs + eq.rhs,
                Operator.Sub => eq.lhs - eq.rhs,
                Operator.Mul => eq.lhs * eq.rhs,
                Operator.Div => eq.lhs / eq.rhs,
                _ => throw new Exception("Unexpected Operator value")
            },
            _ => throw new Exception("Unexpected VarPos value")
        };
    }
    char OperandRepr(Operator op) {
        return op switch {
            Operator.Add => '+',
            Operator.Sub => '-',
            Operator.Mul => '×',
            Operator.Div => '÷',
            _ => throw new Exception("Unexpected Operator value")
        };
    }
    string FormatEquation(Equation eq) {
        string lhsStr = eq.varPos == VarPos.Lhs ? "x" : eq.lhs.ToString();
        string rhsStr = eq.varPos == VarPos.Rhs ? "x" : eq.rhs.ToString();
        string answerStr = eq.varPos == VarPos.Answer ? "x" : eq.answer.ToString();
        return $"{lhsStr} {OperandRepr(eq.op)} {rhsStr} = {answerStr}";
    }
    Equation GenEquation(Operator? opArg = null, bool randVarPos = false) {
        int lhs, rhs;
        Operator op;
        do {
            lhs = RandOperand(_lhsRange);
            rhs = RandOperand(_rhsRange);
            op = (opArg == null) ? RandOperator() : (Operator)opArg;
        } while (op == Operator.Div && (rhs == 0 || lhs % rhs != 0 || lhs <= rhs));
        VarPos varPos = VarPos.Answer;
        int answer = EvalEquation(new(lhs, rhs, 0, op, varPos));
        if (randVarPos) {
            varPos = (VarPos)UnityEngine.Random.Range(0, 3);
            switch (varPos) {
                case VarPos.Lhs: lhs = 0; break;
                case VarPos.Rhs: rhs = 0; break;
                case VarPos.Answer: answer = 0; break;
            }
        }
        return new(lhs, rhs, answer, op, varPos);
    }
    void ShuffleAnswers() {
        System.Random random = new();
        int n = _answers.Length;
        while (n > 1) {
            int k = random.Next(n--);
            int temp = _answers[n];
            _answers[n] = _answers[k];
            _answers[k] = temp;
        }
        for (int i = 0; i < _answerBoxes.Length; ++i) {
            _answerBoxes[i].GetComponent<Answer>().SetNumber(_answers[i]);
        }
    }
    void ApplyDifficulty(float diffDelta) {
        _difficulty = Mathf.Clamp(_difficulty + diffDelta, 1, 4);
        _lhsRange = Vector2Int.RoundToInt((Vector2)_lhsBaseRange * _difficulty);
        _rhsRange = Vector2Int.RoundToInt((Vector2)_rhsBaseRange * _difficulty);
    }
    public void NewQuestion() {
        Equation eq = GenEquation(randVarPos: true);
        _questionText.text = FormatEquation(eq);
        int answerCount = _answerBoxes.Length > 0 ? _answerBoxes.Length : throw new ArgumentException("AnswerBox reference(s) not provided.");
        _answers = new int[answerCount];
        _answers[0] = answer = EvalEquation(eq);
        for (int i = 1; i < answerCount; ++i) {
            _answers[i] = GenEquation(eq.op).answer;
        }
        ShuffleAnswers();
    }
    public void OnAnswer(int number) {
        int scoreDelta = number == answer ? 2 : -1;
        float diffDelta = number == answer ? 0.5f : -0.5f;
        _scoreManager.AddScore(scoreDelta);
        ApplyDifficulty(diffDelta);
        NewQuestion();
    }
}
