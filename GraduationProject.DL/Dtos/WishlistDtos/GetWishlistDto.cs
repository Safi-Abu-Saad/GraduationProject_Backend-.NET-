﻿using GraduationProject.BL.Dtos.PlaceDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduationProject.BL.Dtos;

public class GetWishListDto
{
    public string UserId { get; set; }

    public List<GetPlacesDtos> Places { get; set; } 
}
