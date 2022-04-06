namespace Arma3TacMapLibrary.Maps
{
    public class MilMissionResult
    {
        public MilMissionResult(double[][][] lines)
        {
            Lines = lines;
            Labels = new string[0];
            LabelsPoints = new double[0][];
        }

        public MilMissionResult(double[][][] lines, string[] labels, double[][] labelsPoints)
        {
            Lines = lines;
            Labels = labels;
            LabelsPoints = labelsPoints;
        }

        public double[][][] Lines { get; }

        public string[] Labels { get; }

        public double[][] LabelsPoints { get; }
    }
}