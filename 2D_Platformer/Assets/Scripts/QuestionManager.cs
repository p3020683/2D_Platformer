using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class QuestionManager : MonoBehaviour {
    public int m_LhsMin = 1;
    public int m_LhsMax = 10;
    public int m_RhsMin = 1;
    public int m_RhsMax = 10;
    public bool m_Add = true;
    public bool m_Sub = true;
    public bool m_Mul = false;
    public bool m_Div = false;
    public int m_AnswerCount = 2;
    public Text m_QuestionText;

    int[] m_Answers;

    enum Operand { Add, Sub, Mul, Div };

    int RandRange(bool side) {
        // bool side arg: false = lhs, true = rhs
        int min = side ? m_RhsMin : m_LhsMin;
        int max = side ? m_RhsMax : m_LhsMax;
        return Random.Range(min, max);
    }
    Operand RandOperand() {
        Operand? op = null;
        while (op == null) {
            op = (Operand)Random.Range(0, System.Enum.GetNames(typeof(Operand)).Length);
            switch (op) {
                case Operand.Add: if (!m_Add) { op = null; } break;
                case Operand.Sub: if (!m_Sub) { op = null; } break;
                case Operand.Mul: if (!m_Mul) { op = null; } break;
                case Operand.Div: if (!m_Div) { op = null; } break;
                default: throw new System.Exception("Unexpected Operand value");
            }
        }
        return (Operand)op;
    }
    int EvalQuestion(int lhs, int rhs, Operand op) {
        switch (op) {
            case Operand.Add: return lhs + rhs;
            case Operand.Sub: return lhs - rhs;
            case Operand.Mul: return lhs * rhs;
            case Operand.Div: return lhs / rhs;
            default: throw new System.Exception("Unexpected Operand value");
        }
    }
    char OperandRepr(Operand op) {
        switch (op) {
            case Operand.Add: return '+';
            case Operand.Sub: return '-';
            case Operand.Mul: return '*';
            case Operand.Div: return '/';
            default: throw new System.Exception("Unexpected Operand value");
        }
    }
    System.Tuple<string, int> GenQuestion(Operand? operand = null) {
        int lhs = RandRange(false);
        int rhs = RandRange(true);
        Operand op = (operand == null) ? RandOperand() : (Operand)operand;
        int answer = EvalQuestion(lhs, rhs, op);
        return new($"{lhs} {OperandRepr(op)} {rhs}", answer);
    }
    void NewQuestion() {
        System.Tuple<string, int> question = GenQuestion();
        m_QuestionText.text = $"What is {question.Item1}?";
        m_Answers = new int[] { question.Item2 };
        for (int i = 0; i < m_AnswerCount; ++i) {
            m_Answers.Append<int>(GenQuestion().Item2);
        }
    }
}
