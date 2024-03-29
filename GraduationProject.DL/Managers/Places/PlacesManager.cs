﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GraduationProject.Bl.Dtos.PlaceDtos;
using GraduationProject.BL.Dtos.PlaceDtos;
using GraduationProject.DAL;
using GraduationProject.DAL.Data;
using GraduationProject.Data.Context;
using GraduationProject.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GraduationProject.BL.Managers.Places
{
    public class PlacesManager : IPlacesManager
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly Cloudinary _Cloudinary;
        private readonly ApplicationDbContext _context;



        public PlacesManager(IUnitOfWork unitOfWork, IOptions<CloudinarySettings> config, ApplicationDbContext context)
        {
            _context = context;
            _UnitOfWork = unitOfWork;
            var acc = new Account
           (
              config.Value.CloudName,
              config.Value.APIKey,
              config.Value.APISecret

           );

            _Cloudinary = new Cloudinary(acc);
        }

        public async Task< int> Add(AddPlaceDto addPlaceDto)
        {

            Place placedb = new Place()
            {
                Name = addPlaceDto.Name,
                Price = addPlaceDto.Price,
                Location = addPlaceDto.Location,
                Description = addPlaceDto.Description,
                PeopleCapacity = addPlaceDto.PeopleCapacity,
                OwnerId = addPlaceDto.OwnerId,
                //CategoryId = addPlaceDto.CategoryId

            };

           await _UnitOfWork.Placesrepo.AddAsync(placedb);
          await  _UnitOfWork.SaveChangesAsync();
            return (placedb.PlaceId);
        }

        //Adding place with photo
        public async Task<ImageUploadResult> AddPhotoAsync(AddPlaceDto addPlaceDto, IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.Name, stream),
                    Transformation = new Transformation()/*.Height(500).Width(500).Crop("fill").Gravity("face")*/

                };

                uploadResult = await _Cloudinary.UploadAsync(uploadParams);
                await _UnitOfWork.SaveChangesAsync();


            }
            return uploadResult;

        }


        public async Task< bool> Delete(int id)
        {

            Place? placedb =await _UnitOfWork.Placesrepo.GetById(id);
            if (placedb == null)
            {
                return false;
            }
            try
            {

              await  _UnitOfWork.Placesrepo.Delete(placedb);
                await _UnitOfWork.SaveChangesAsync();
                return true;

        //    Place? placedb = _UnitOfWork.Placesrepo.GetById(id);
        //    if (placedb == null)
        //    {
        //        return false;
        //    }
        //    try
        //    {

        //        _UnitOfWork.Placesrepo.Delete(placedb);
        //        //_UnitOfWork.Placesrepo.SaveChanges();
        //        return true;

        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}


        public async Task< IEnumerable<GetPlacesDtos>> GetAll()
        {
            IEnumerable<Place> placesdb = await _UnitOfWork.Placesrepo.GetAll();
            var placedto = placesdb.Select(x => new GetPlacesDtos
            {
                Name = x.Name,
                Description = x.Description,
                PlaceId = x.PlaceId,
                Price = x.Price,
                Location = x.Location,
                OverAllRating = x.OverAllRating,
                PeopleCapacity = x.PeopleCapacity
            });
            return placedto;
        }

        public async Task< GetPlacesDtos> GetById(int id)
        {
            Place? placesdb =await _UnitOfWork.Placesrepo.GetById(id);
            if (placesdb == null)
            {
                return null;
            }
            var placedto = new GetPlacesDtos
            {
                Name = placesdb.Name,
                Description = placesdb.Description,
                PlaceId = placesdb.PlaceId,
                Price = placesdb.Price,
                Location = placesdb.Location,
                OverAllRating = placesdb.OverAllRating,
                PeopleCapacity = placesdb.PeopleCapacity
            };
            return placedto;
        }

    //Update Place Only
       public async Task< bool> Update(UpdatePlaceDto updatePlaceDto)
        {
            Place? place =await _UnitOfWork.Placesrepo.GetById(updatePlaceDto.PlaceId);

            if (place == null) return false;
          
            place.Name = updatePlaceDto.Name;
            place.Description = updatePlaceDto.Description;
            place.Location = updatePlaceDto.Location;
            place.Price = updatePlaceDto.Price;
            place.PeopleCapacity = updatePlaceDto.PeopleCapacity;


          await  _UnitOfWork.Placesrepo.Update(place);
            await _UnitOfWork.SaveChangesAsync();

            return true;
        }




            //Update Image Only
        public async Task<ImageUploadResult> UpdateImageAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.Name, stream),
                     // Set the public ID to update the existing image
                };

                uploadResult = await _Cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

    }



}
