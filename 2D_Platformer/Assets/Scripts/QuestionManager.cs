using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class QuestionManager : MonoBehaviour {
    public Vector2Int m_LhsRange = new(1, 10);
    public Vector2Int m_RhsRange = new(1, 10);
    public bool m_Add = true;
    public bool m_Sub = true;
    public bool m_Mul = false;
    public bool m_Div = false;
    public int m_AnswerCount = 2;
    public Text m_QuestionText;

    int[] m_Answers;

    enum Operator { Add, Sub, Mul, Div };

    int RandOperand(bool side) {
        int min = side ? m_RhsRange.x : m_LhsRange.x;
        int max = side ? m_RhsRange.y : m_LhsRange.y;
        return Random.Range(min, max);
    }
    Operator RandOperator() {
        Operator? op = null;
        while (op == null) {
            op = (Operator)Random.Range(0, System.Enum.GetNames(typeof(Operator)).Length);
            switch (op) {
                case Operator.Add: if (!m_Add) { op = null; } break;
                case Operator.Sub: if (!m_Sub) { op = null; } break;
                case Operator.Mul: if (!m_Mul) { op = null; } break;
                case Operator.Div: if (!m_Div) { op = null; } break;
                default: throw new System.Exception("Unexpected Operator op value");
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
            default: throw new System.Exception("Unexpected Operator op value");
        }
    }
    char OperandRepr(Operator op) {
        switch (op) {
            case Operator.Add: return '+';
            case Operator.Sub: return '-';
            case Operator.Mul: return '*';
            case Operator.Div: return '/';
            default: throw new System.Exception("Unexpected Operator op value");
        }
    }
    string FormatQuestion(int lhs, int rhs, Operator op) {
        return $"{lhs} {OperandRepr(op)} {rhs}";
    }
    int[] GenQuestion(Operator? opArg = null) {
        int lhs = RandOperand(false);
        int rhs = RandOperand(true);
        Operator op = (opArg == null) ? RandOperator() : (Operator)opArg;
        int answer = EvalQuestion(lhs, rhs, op);
        return new int[] { lhs, rhs, (int)op, answer };
    }
    public void NewQuestion() {
        int[] question = GenQuestion();
        m_QuestionText.text = $"What is {FormatQuestion(question[0], question[1], (Operator)question[2])}?";
        m_Answers = new int[] { question[3] };
        for (int i = 0; i < m_AnswerCount; ++i) {
            m_Answers.Append(GenQuestion((Operator)question[2])[3]);
        }

        Debug.Log(m_Answers.Length);
        Debug.Log(m_Answers.ToString());
    }
}
