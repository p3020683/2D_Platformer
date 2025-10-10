using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour {
    public Vector2Int m_LhsRange = new(1, 10);
    public Vector2Int m_RhsRange = new(1, 10);
    [SerializeField] OperatorFlags m_OpFlags = new(true, true, false, false);
    public GameObject[] m_AnswerBoxes;
    public Text m_QuestionText;
    public ScoreManager m_ScoreManager;
    [NonSerialized] public int[] m_Answers;
    int m_Difficulty = 1;

    enum Operator { Add, Sub, Mul, Div };

    [Serializable] struct OperatorFlags {
        public bool add, sub, mul, div;
        public OperatorFlags(bool add, bool sub, bool mul, bool div) {
            this.add = add; this.sub = sub; this.mul = mul; this.div = div;
        }
    };
    struct Equation {
        public int lhs, rhs, answer;
        public Operator op;
        public Equation(int lhs, int rhs, int answer, Operator op) {
            this.lhs = lhs; this.rhs = rhs; this.answer = answer; this.op = op;
        }
    };

    int RandOperand(bool side) {
        int min = side ? m_RhsRange.x : m_LhsRange.x;
        int max = side ? m_RhsRange.y : m_LhsRange.y;
        return UnityEngine.Random.Range(min, max);
    }
    Operator RandOperator() {
        int enumLength = Enum.GetNames(typeof(Operator)).Length;
        Operator? op = null;
        while (op == null) {
            op = (Operator)UnityEngine.Random.Range(0, enumLength);
            switch (op) {
                case Operator.Add: if (!m_OpFlags.add) { op = null; } break;
                case Operator.Sub: if (!m_OpFlags.sub) { op = null; } break;
                case Operator.Mul: if (!m_OpFlags.mul) { op = null; } break;
                case Operator.Div: if (!m_OpFlags.div) { op = null; } break;
                default: throw new Exception("Unexpected Operator op value");
            }
        }
        return (Operator)op;
    }
    int EvalQuestion(int lhs, int rhs, Operator op) {
        switch (op) {
            case Operator.Add: return lhs + rhs;
            case Operator.Sub: return lhs - rhs;
            case Operator.Mul: return lhs * rhs;
            case Operator.Div: return lhs / rhs;
            default: throw new Exception("Unexpected Operator op value");
        }
    }
    char OperandRepr(Operator op) {
        switch (op) {
            case Operator.Add: return '+';
            case Operator.Sub: return '-';
            case Operator.Mul: return '*';
            case Operator.Div: return '/';
            default: throw new Exception("Unexpected Operator op value");
        }
    }
    string FormatQuestion(int lhs, int rhs, Operator op) {
        return $"{lhs} {OperandRepr(op)} {rhs}";
    }
    Equation GenEquation(Operator? opArg = null) {
        int lhs = RandOperand(false);
        int rhs = RandOperand(true);
        Operator op = (opArg == null) ? RandOperator() : (Operator)opArg;
        int answer = EvalQuestion(lhs, rhs, op);
        return new(lhs, rhs, answer, op);
    }
    void DistributeAnswers() {
        int[] arr = m_Answers;
        System.Random random = new();
        int n = arr.Length;
        while (n > 1) {
            int k = random.Next(n--);
            int temp = arr[n];
            arr[n] = arr[k];
            arr[k] = temp;
        }
        for (int i = 0; i < m_AnswerBoxes.Length; ++i) {
            m_AnswerBoxes[i].GetComponent<Answer>().SetNumber(arr[i]);
        }
    }
    public void NewQuestion() {
        Equation eq = GenEquation();
        m_QuestionText.text = $"What is {FormatQuestion(eq.lhs, eq.rhs, eq.op)}?";
        int answerCount = m_AnswerBoxes.Length;
        m_Answers = new int[answerCount];
        m_Answers[0] = eq.answer;
        for (int i = 1; i < answerCount; ++i) {
            m_Answers[i] = GenEquation(eq.op).answer;
        }
        DistributeAnswers();
    }
    public void OnAnswer(int number) {
        if (number == m_Answers[0]) {
            m_ScoreManager.AddScore(2);
        }
        else {
            m_ScoreManager.AddScore(-1);
        }
    }
}
