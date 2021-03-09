using System.ComponentModel.DataAnnotations;

namespace Arma3TacMapWebApp.Models
{
    public enum ImportFileFormat
    {
        [Display(Name = "[Arma 3 After Action Report] Pre-processed text file (.txt)")]
        AarTxt,

        [Display(Name = "[Arma 3 After Action Report] Server log file (.rpt)")]
        AarLog,

        [Display(Name = "[Arma 3 1erGTD Track] Server log file (.rpt)")]
        GtdLog
    }
}