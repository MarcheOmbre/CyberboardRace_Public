using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Player.Status;
using UnityEngine;

namespace Project.Scripts.Scoring.Computers
{
    [RequireComponent(typeof(ScoringManager))]
    public class TrickScoreComputer : MonoBehaviour
    {
        private enum TrickType
        {
            SimpleFlip,
            SimpleSpin,

            BreakFlip,
            BreakSpin,
            SpinFlip,

            LateFlip,
            LateSpin,

            Undefined
        }

        private struct ComputedTrick
        {
            public TrickType TrickType;
            public TrickDetection.Direction Direction;
            public int Count;
        }

        [SerializeField] private ScoreConfig simpleFlip;
        [SerializeField] private ScoreConfig simpleSpin;
        [SerializeField] private ScoreConfig breakFlip;
        [SerializeField] private ScoreConfig breakSpin;
        [SerializeField] private ScoreConfig spinFlip;
        [SerializeField] private ScoreConfig lateFlip;
        [SerializeField] private ScoreConfig lateSpin;

        private ScoringManager _scoringManager;
        private TrickDetection _trickDetection;

        private void Awake()
        {
            _scoringManager = GetComponent<ScoringManager>();
            _trickDetection = FindObjectOfType<TrickDetection>();
        }

        private void OnEnable()
        {
            _trickDetection.OnTrick += OnTrick;
        }

        private void OnDisable()
        {
            _trickDetection.OnTrick -= OnTrick;
        }

        private void ComputeCombo(IReadOnlyList<TrickDetection.TrickData> tricks)
        {
            foreach (var lineTrick in ComputeTrick(tricks))
            {
                if (lineTrick.TrickType == TrickType.Undefined || lineTrick.Count <= 0)
                    continue;
                
                var scoreConfig = lineTrick.TrickType switch
                {
                    TrickType.SimpleFlip => simpleFlip,
                    TrickType.SimpleSpin => simpleSpin,
                    TrickType.BreakFlip => breakFlip,
                    TrickType.BreakSpin => breakSpin,
                    TrickType.SpinFlip => spinFlip,
                    TrickType.LateFlip => lateFlip,
                    TrickType.LateSpin => lateSpin,
                    _ => null
                };
                
                if(scoreConfig == null)
                    continue;

                //Set name 
                var multiplier= lineTrick.Count switch
                {
                    0 => null,
                    1 => null,
                    2 => "Double",
                    3 => "Triple",
                    4 => "Quadruple",
                    _ => "Monster"
                };

                var direction = string.Empty;
                if(lineTrick.Direction.HasFlag(TrickDetection.Direction.LeftToRight) || lineTrick.Direction.HasFlag(TrickDetection.Direction.RightToLeft))
                    direction = "Diagonal";
                else if(lineTrick.Direction != TrickDetection.Direction.Undefined)
                    direction = lineTrick.Direction.ToString();

                var prefix = multiplier;
                if (direction != null)
                    prefix += " " + direction;

                //Texts
                _scoringManager.AddSimpleScore(new ScoringManager.SimpleScoreData
                {
                    Prefix = prefix,
                    ScoreConfig = scoreConfig
                    
                });
            }
        }

        private void OnTrick(IEnumerable<TrickDetection.TrickData> tricksLine) => ComputeCombo(tricksLine.ToArray());

        private static bool SimpleTrick(TrickDetection.TrickData trick, out ComputedTrick computedTrick)
        {
            computedTrick = default;

            //First check
            if (trick.Type == TrickDetection.Type.Undefined || trick.Half < 2)
                return false;

            computedTrick = new ComputedTrick
            {
                TrickType = trick.Type switch
                {
                    TrickDetection.Type.Flip => TrickType.SimpleFlip,
                    TrickDetection.Type.Spin => TrickType.SimpleSpin,
                    _ => TrickType.Undefined
                },

                Direction = trick.Direction,

                Count = trick.Half / 2
            };

            return true;
        }

        private static bool Combined2Trick(TrickDetection.TrickData trick1, TrickDetection.TrickData trick2,
            out ComputedTrick combinedTrick)
        {
            combinedTrick = default;

            if (trick1.Type == TrickDetection.Type.Undefined || trick2.Type == TrickDetection.Type.Undefined
                                                             || trick1.Half != 1 || trick2.Half != 1)
                return false;

            //Type and direction
            if (trick1.Type == trick2.Type && trick1.Direction == trick2.Direction)
            {
                combinedTrick.TrickType =
                    trick1.Type == TrickDetection.Type.Flip ? TrickType.LateFlip : TrickType.LateSpin;

                combinedTrick.Direction = trick1.Direction;
            }
            else
            {
                if (trick1.Type == trick2.Type)
                    combinedTrick.TrickType = trick1.Type == TrickDetection.Type.Flip
                        ? TrickType.BreakFlip
                        : TrickType.BreakSpin;
                else
                    combinedTrick.TrickType = TrickType.SpinFlip;

                combinedTrick.Direction = TrickDetection.Direction.Undefined;
            }

            //Count
            combinedTrick.Count = 1;

            return true;
        }

        private static IEnumerable<ComputedTrick> ComputeTrick(IReadOnlyList<TrickDetection.TrickData> tricks)
        {
            var tricksLine = new List<ComputedTrick>();
            if (tricks.Count <= 0)
                return tricksLine;

            TrickDetection.TrickData? lastHalfTrick = null;
            for (var i = 0; i < tricks.Count; i++)
            {
                var loop = tricks[i].Half / 2;
                var half = tricks[i].Half % 2;

                if (loop > 0)
                {
                    var trick = tricks[i];
                    trick.Half = loop * 2;

                    if (SimpleTrick(trick, out var computedTrick))
                        tricksLine.Add(computedTrick);
                }

                if (half > 0)
                {
                    var trick = tricks[i];
                    trick.Half = half;

                    if (lastHalfTrick != null && Combined2Trick(lastHalfTrick.Value, trick, out var computedTrick))
                    {
                        tricksLine.Add(computedTrick);
                        lastHalfTrick = null;
                    }
                    else
                        lastHalfTrick = trick;
                }
                else
                    lastHalfTrick = null;
            }

            return tricksLine;
        }
    }
}