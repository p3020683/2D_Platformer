using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EquationStore : MonoBehaviour {
    [SerializeField] string _fileName = "equations.kvp";

    List<EquationEntry> equations = new();
    bool _ready = false;

    public bool IsReady { get => _ready; }

    public struct EquationEntry {
        public string equation;   // e.g. "3x + 5 = 17"
        public float solution;    // x coefficient
    }

    void Start() {
        string path = Path.Combine(Application.streamingAssetsPath, _fileName);
        if (File.Exists(path)) {
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines) {
                if (string.IsNullOrEmpty(line) || !line.Contains(':')) { continue; }

                string[] parts = line.Split(':');
                if (parts.Length != 2) { continue; }

                float sol = float.Parse(parts[1]);

                equations.Add(new EquationEntry {
                    equation = parts[0],
                    solution = sol
                });
            }
            _ready = true;
        }
        else { throw new FileNotFoundException($"{_fileName} not found in StreamingAssets."); }
    }
    public EquationEntry GetRandomEquation() {
        if (equations.Count == 0) {
            return new EquationEntry {
                equation = "All equations consumed.\nCongrats..?",
                solution = float.NaN
            };
        }

        int index = Random.Range(0, equations.Count);
        EquationEntry entry = equations[index];
        equations.RemoveAt(index);
        return entry;
    }
}
