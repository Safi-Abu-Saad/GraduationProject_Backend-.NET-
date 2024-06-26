﻿using GraduationProject.BL.Dtos;
using GraduationProject.BL.Dtos.PlaceDtos;
using GraduationProject.DAL.Data;
using GraduationProject.Data.Context;
using GraduationProject.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GraduationProject.BL.Managers;

public class WishlistManager : IWishlistManager
{
    private readonly IUnitOfWork _UnitOfWork;
    private readonly ApplicationDbContext _context;

    public WishlistManager(IUnitOfWork unitOfWork , ApplicationDbContext context)
    {
        _UnitOfWork = unitOfWork;
        _context = context;
    }
    async Task<int> IWishlistManager.Add(AddWishlistDto addWishlistDto)
    {
        WishList wishList = new WishList
        {
            UserId = addWishlistDto.UserId,
            PlaceId = addWishlistDto.PlaceId
        };

        await _UnitOfWork.Wishlistrepo.AddAsync(wishList);
        await _UnitOfWork.SaveChangesAsync();
        return wishList.PlaceId;

    }

    

    [HttpGet("{userid}")]
    public async Task<ActionResult<IEnumerable<GetPlaceWishlistDto>>> UserplaceList(string userid)
    {
        var userPlaces = await _context.Users
            .Where(u => u.Id == userid)
            .Include(u => u.OwnedPlaces)
            .ThenInclude(p => p.Images)
            .SelectMany(u => u.OwnedPlaces)
            .ToListAsync();

        var placeDtos = userPlaces.Select(p => new GetPlaceWishlistDto
        {
            PlaceId = p.PlaceId,
            Name = p.Name,
            Price = p.Price,
            OverAllRating = p.OverAllRating,
            Description = p.Description,
            ImageUrls = p.Images.Select(x => x.ImageUrl).ToArray()
        }).ToList();

        return placeDtos;
    }



    public async Task<WishList> DeletePlaceFromWishlist(string userid, int placeid)
    {
        var user = await _context.Users
            .Include(u => u.OwnedPlaces)
            .ThenInclude(d => d.WishListPlaceUsers)
            .FirstOrDefaultAsync(u => u.Id == userid);

        if (user != null)
        {
            var placeToDelete = user.WishListUserPlaces.FirstOrDefault(p => p.PlaceId == placeid);

            if (placeToDelete != null)
            {
                user.WishListUserPlaces.Remove(placeToDelete);
                await _context.SaveChangesAsync();
                return placeToDelete;
            }
        }

        return null;
    }

    public async Task<WishList> GetByUserIdAndPlaceId(string userid , int placeid)
    {
        return await _UnitOfWork.Wishlistrepo.Wishlistbyuseridandplaceid(userid, placeid);
    }




   public async Task<IEnumerable<GetPlaceWishlistDto>> GetAll(string userid)
    {
        try
        {
            var wishlist = await _context.WishList
               .Include(a => a.User)
               .Where(d => d.UserId == userid)
               .Include(s => s.Places)
               .ThenInclude(e => e.Images)
               .ToListAsync();


            var wishlistDtoList = wishlist
                .Select(item => item.Places)
                .Select(place => new GetPlaceWishlistDto
                {
                    PlaceId = place.PlaceId,
                    Name = place.Name,
                    Price = place.Price,
                    OverAllRating = place.OverAllRating,
                    Description = place.Description,
                    ImageUrls = place.Images.Select(x => x.ImageUrl).ToArray()

                }).ToList();
              


            return wishlistDtoList;
        }
        catch (Exception ex)
        {

            return null;
        }

    }

}




