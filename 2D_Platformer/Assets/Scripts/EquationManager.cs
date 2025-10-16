using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using UnityEngine;

public class EquationManager : MonoBehaviour {
    System.Random rand;

    double SolveForX(string equation) {
        // split equation into left and right sides
        string[] parts = equation.Split('=');
        if (parts.Length != 2) {
            throw new ArgumentException("Equation must have one '=' sign.");
        }

        string lhs = parts[0].Trim();
        string rhs = parts[1].Trim();

        // insert explicit multiplication operators
        lhs = Regex.Replace(lhs, @"(\d+)x", "$1*x");
        lhs = Regex.Replace(lhs, @"(\d+)\(", "$1*(");

        rhs = Regex.Replace(rhs, @"(\d+)x", "$1*x");
        rhs = Regex.Replace(rhs, @"(\d+)\(", "$1*(");

        // evaluate constant parts and find coefficients of expressions
        double lhsConst = Evaluate(lhs.Replace('x', '0'));
        double lhsWithX1 = Evaluate(lhs.Replace('x', '1'));
        double lhsCoeff = lhsWithX1 - lhsConst;

        double rhsConst = Evaluate(rhs.Replace('x', '0'));
        double rhsWithX1 = Evaluate(rhs.Replace('x', '1'));
        double rhsCoeff = rhsWithX1 - rhsConst;

        // calculate net constant part and coefficient of equation
        double netConst = rhsConst - lhsConst;
        double netCoeff = lhsCoeff - rhsCoeff;
        if (Math.Abs(netCoeff) < 1e-9) {
            throw new InvalidOperationException("Equation does not contain a valid x term.");
        }
        return Math.Round(netConst / netCoeff, 2);

        static double Evaluate(string expr) {
            return Convert.ToDouble(new DataTable().Compute(expr, ""));
        }
    }
    string RandomTerm(bool includeX = true) {
        // 60% chance of x-term, 40% constant
        bool isX = includeX && rand.NextDouble() < 0.6;
        int coeff = rand.Next(1, 10);
        string term = isX ? $"{coeff}x" : $"{coeff}";
        if (rand.Next(2) == 0)
            return term;
        return $"-{term}";
    }

    string RandomOperator() {
        string[] ops = { " + ", " - ", " * ", " / " };
        return ops[rand.Next(ops.Length)];
    }
    string RandomExpression(bool forceX = true) {
        int termCount = rand.Next(2, 4);
        List<string> terms = new List<string>();

        for (int i = 0; i < termCount; i++)
            terms.Add(RandomTerm(includeX: i == 0 && forceX));

        string expr = string.Join(RandomOperator(), terms);

        // 30% chance to wrap in parentheses
        if (rand.NextDouble() < 0.3)
            expr = $"({expr})";

        return expr;
    }
    string GenerateEquation() {
        // randomly choose whether x appears on both sides
        bool xBothSides = rand.NextDouble() < 0.3;

        string lhs = RandomExpression(forceX: true);
        string rhs = RandomExpression(forceX: xBothSides);

        // occasionally make one side just a number
        if (!xBothSides && rand.NextDouble() < 0.4)
            rhs = rand.Next(-10, 25).ToString();

        return $"{lhs} = {rhs}";
    }
}
