using HyperSimplices.SampleGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyperSimplices.Utils
{


    public class ProgressBar
    {
        private const int _barLength = 30;
        private const char _marker = '|';                
        private List<double> _elapsedSecondsPerIteration;

        public ProgressBar()
        {
            _elapsedSecondsPerIteration = new List<double>();
        }

        public void HandleSampleCreationEvent(object sender, SampleCreationEventArgs e)
        {
            Report(e.CreatedSamples, e.TotalSamples, e.ElapsedTime);
        }

        private string Animator(int step)
        {
            var ani = step % 4;

            switch(step % 4)
            {
                case 0:
                    return "\\";
                case 1:
                    return "|";
                case 2:
                    return "/";
                case 3:
                    return "--";
                default:
                    return String.Empty;
            }
        }

        private string EstimateRemaingTime(int performedIterations, int totalIterations)
        {
            var averageTimePerStep = _elapsedSecondsPerIteration.Average();
            var estimatedRemaingTime = TimeSpan.FromSeconds(((double)totalIterations - (double)performedIterations) 
                * averageTimePerStep);
            return estimatedRemaingTime.ToString(@"dd\:hh\:mm\:ss");
        }

        public void Report(int performedIterations, int totalIterations, double elapsedTime)
        {
            _elapsedSecondsPerIteration.Add(elapsedTime);

            var percentagePerformed = (double)performedIterations / (double)totalIterations;

            var stringBuilder = new StringBuilder(); 
            var nbMarkers = Math.Min((int)Math.Round(percentagePerformed * _barLength), _barLength);
            stringBuilder.Append("|");
            stringBuilder.Append(_marker, nbMarkers);
            stringBuilder.Append(' ', _barLength - nbMarkers);
            stringBuilder.Append("|");
            stringBuilder.Append(' ', 2);
            stringBuilder.Append(Animator(performedIterations));
            stringBuilder.Append(' ', 2);
            stringBuilder.Append(percentagePerformed.ToString("P", CultureInfo.InvariantCulture));

            stringBuilder.Append($"    Remaining time in min: {EstimateRemaingTime(performedIterations, totalIterations)}");

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(stringBuilder.ToString());
        }
    }
}
