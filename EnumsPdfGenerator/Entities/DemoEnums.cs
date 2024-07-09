using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnumsPdfGenerator.Entities;

public enum DemoStatus
{
    [Display(Name = "Initiated Job")]
    Initiated = 100,

    [Display(Name = "Processing File")]
    Processing = 200,

    [Display(Name = "Processed File")]
    Processed = 300,

}
