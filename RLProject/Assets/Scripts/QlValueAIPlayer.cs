using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

class QlValueAIPlayer: Player
{
    [SerializeField] string aiScriptPath;
    private string startDir;
    private Process execPros;

    [SerializeField] int numEpisEachFrame;

    [SerializeField] float discountRate;
    [SerializeField] Range epsilonRate;
    [SerializeField] Range learningRate;
    [SerializeField] int maxEpisodes;
    [SerializeField] int maxSteps;
    [SerializeField] List<RewardType> rewards;

    [Serializable]
    struct Range
    {
        [SerializeField] public float start;
        [SerializeField] public float end;
    }

    [Serializable]
    struct RewardType
    {
        [SerializeField] public string nodeType;
        [SerializeField] public float reward;
    }

    public void Play(string startDir)
    {
        this.startDir = startDir;
        WriteDataFile($"{startDir}/ql_data.txt");

        execPros = new Process();
        execPros.StartInfo = new ProcessStartInfo(@"python3", $"{aiScriptPath} {startDir}")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        
        execPros.Start();

        StartCoroutine(DisplayMoves($"{startDir}/ql_out_episodes.txt"));
    }

    private IEnumerator DisplayMoves(string episodeFileName)
    {
        execPros.WaitForExit();

        var wait = new WaitForSeconds(0.01f);
        int episodeIndex = 0;
        foreach (string line in File.ReadLines(episodeFileName))
        {
            if (line.StartsWith("episode actions:"))
            {
                ++episodeIndex;
                if (episodeIndex%numEpisEachFrame != 0)
                {
                    continue;
                }

                var firstIndex = line.IndexOf('[');
                var strActions = line.Substring(firstIndex + 1, line.IndexOf(']') - firstIndex - 1);
                List<int> actions = new List<int>(Array.ConvertAll(strActions.Split(','), int.Parse));
                foreach (var action in actions)
                {
                    switch (action)
                    {
                        case 0:
                            MoveUp();
                            break;
                        case 1:
                            MoveRight();
                            break;
                        case 2:
                            MoveDown();
                            break;
                        case 3:
                            MoveLeft();
                            break;
                        default:
                            throw new Exception("unknown action");
                    }

                    yield return new WaitUntil(() => isMoveEnded == true);
                }

                
                SetPosition(0, 0);
                mazeBoard.intialzeTargets();
                yield return wait;
            }
        }
    }

    private void WriteDataFile(string dataFileName)
    {
        using (StreamWriter writeText = new StreamWriter(dataFileName))
        {
            writeText.Write($"discount rate:\n{discountRate}\n\n");
            writeText.Write($"epsilon rate:\n{epsilonRate.start} {epsilonRate.end}\n\n");
            writeText.Write($"learning rate:\n{learningRate.start} {learningRate.end}\n\n");
            writeText.Write($"max episodes:\n{maxEpisodes}\n\n");
            writeText.Write($"max steps:\n{maxSteps}\n\n");

            string strRewards = "{";
            foreach (var e in rewards){
                strRewards += $"'{e.nodeType}': {e.reward},";
            }
            strRewards = strRewards.Remove(strRewards.Length - 1) + "}";
            writeText.Write($"rewards by state-type:\n{strRewards}\n\n");

            writeText.Write($"states data:\n{mazeBoard}\n");

            writeText.Write($"starting position:\n{posInMaze.x} {posInMaze.y}\n");
        }
    }
}
