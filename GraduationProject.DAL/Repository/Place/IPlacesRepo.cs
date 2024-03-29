﻿using GraduationProject.DAL.Repository.Generics;
using GraduationProject.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace GraduationProject.DAL;

public interface IPlacesRepo : IGenericRepo<Place>
{
}