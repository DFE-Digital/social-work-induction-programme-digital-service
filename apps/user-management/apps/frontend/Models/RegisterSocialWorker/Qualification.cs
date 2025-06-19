using System.ComponentModel.DataAnnotations;

namespace Dfe.Sww.Ecf.Frontend.Models.RegisterSocialWorker;

public enum Qualification
{
    [Display(Name = "Another qualification equivalent to a degree")] OtherQualificationDegreeEquivalent,
    [Display(Name = "Bachelor of Arts (BA)")] Ba,
    [Display(Name = "Bachelor of Science (BSc)")] Bsc,
    [Display(Name = "Master of Arts (MA)")] Ma,
    [Display(Name = "Master of Science (MSc)")] Msc,
    [Display(Name = "Postgraduate Diploma")] PostgraduateDiploma
}
