﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduationProject.DL.Dtos.SignDtos
{
    public class ResetCode
    {
        [Required(ErrorMessage = "vreficationCode is required")]
        public string VerificationCode { get; set; }

    }
}
