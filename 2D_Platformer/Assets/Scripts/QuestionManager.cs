using System;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour {
    public Vector2Int m_LhsRange = new(1, 10);
    public Vector2Int m_RhsRange = new(1, 10);
    [SerializeField] OperatorFlags m_OpFlags = new(true, true, false, false);
    public GameObject[] m_AnswerBoxes;
    public Text m_QuestionText;
    public ScoreManager m_ScoreManager;
    [NonSerialized] public int m_Answer;
    int[] m_Answers;
    float m_Difficulty = 1;

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
                Operator.Add => m_OpFlags.add ? op : null,
                Operator.Sub => m_OpFlags.sub ? op : null,
                Operator.Mul => m_OpFlags.mul ? op : null,
                Operator.Div => m_OpFlags.div ? op : null,
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
            Operator.Mul => '*',
            Operator.Div => '/',
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
            lhs = RandOperand(m_LhsRange);
            rhs = RandOperand(m_RhsRange);
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
        int n = m_Answers.Length;
        while (n > 1) {
            int k = random.Next(n--);
            int temp = m_Answers[n];
            m_Answers[n] = m_Answers[k];
            m_Answers[k] = temp;
        }
        for (int i = 0; i < m_AnswerBoxes.Length; ++i) {
            m_AnswerBoxes[i].GetComponent<Answer>().SetNumber(m_Answers[i]);
        }
    }
    void ApplyDifficulty(float diffDelta) {
        m_Difficulty = Mathf.Clamp(m_Difficulty + diffDelta, 1, 5);
        m_LhsRange = Vector2Int.RoundToInt((Vector2)m_LhsRange * m_Difficulty);
        m_RhsRange = Vector2Int.RoundToInt((Vector2)m_RhsRange * m_Difficulty);
    }
    public void NewQuestion() {
        Equation eq = GenEquation(randVarPos: true);
        m_QuestionText.text = $"Solve for x: {FormatEquation(eq)}";
        int answerCount = m_AnswerBoxes.Length > 0 ? m_AnswerBoxes.Length : throw new ArgumentException("AnswerBox reference(s) not provided.");
        m_Answers = new int[answerCount];
        m_Answers[0] = m_Answer = EvalEquation(eq);
        for (int i = 1; i < answerCount; ++i) {
            m_Answers[i] = GenEquation(eq.op).answer;
        }
        ShuffleAnswers();
    }
    public void OnAnswer(int number) {
        int scoreDelta = number == m_Answer ? 2 : -1;
        float diffDelta = number == m_Answer ? 0.5f : -0.5f;
        m_ScoreManager.AddScore(scoreDelta);
        ApplyDifficulty(diffDelta);
        NewQuestion();
    }
}
