﻿using Models;
using Models.DataTransfer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class Mapper
    {
        public UserDto ConvertUserToUserDto(User user)
        {
            UserDto convertedUser = new UserDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                TeamID = user.TeamID,
                RoleID = user.RoleID
            };
            return convertedUser;
        }
        public UserLoggedInDto ConvertUserToUserLoggedInDto(User user)
        {
            UserLoggedInDto convertedUser = new UserLoggedInDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                TeamID = user.TeamID,
                RoleID = user.RoleID
            };
            return convertedUser;
        }
    }
}