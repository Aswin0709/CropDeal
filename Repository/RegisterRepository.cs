﻿using CaseStudy.Dtos;
using CaseStudy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http.ModelBinding;

namespace CaseStudy.Repository
{
    public class RegisterRepository : IRegisterRepository
    {
        private enum Role
        {
            Farmer = 1,
            Dealer = 2
        }
        DatabaseContext _context;
        public RegisterRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<ActionResult<User>> CreateUserAsync(CreateUserDto newUser)
        {
            try
            {
                var user = new User();
                user.Name = newUser.Name;
                user.Email = newUser.Email;
                user.Phone = newUser.Phone;
                user.Status = newUser.Status;
                user.RoleId = (int)Enum.Parse(typeof(Role), newUser.Role);


                using (var hmac = new HMACSHA512())
                {
                    user.PasswordSalt = hmac.Key;
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(newUser.Password));
                }

                //Address
                var address = new Address();
                address.Line = newUser.Line;
                address.City = newUser.City;
                address.State = newUser.State;

                user.Address = address;

                Account acc = new Account();
                //Account
                acc.AccountNumber = newUser.AccountNumber;
                acc.IFSCCode = newUser.IFSC;
                acc.BankName = newUser.BankName;
                user.Account = acc;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Registration");
            }
            return null;
        }
        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var passHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return passHash.SequenceEqual(passwordHash);
            }
        }

       
    }
}
