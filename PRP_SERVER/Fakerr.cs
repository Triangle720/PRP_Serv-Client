using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRP_SERVER.Models;
using PRP_SERVER.Management;
using Bogus;


namespace PRP_SERVER
{
    public class Fakerr
    {
        public User FakeUser()
        {
            User user = new User();
            user.Name = Faker.Name.First();
            user.Surname = Faker.Name.Last();
            user.DateOfBirth = new DateTime();
            user.Login = Faker.Internet.UserName();
            user.Password = MD5class.Create(Faker.RandomNumber.Next(1,9999).ToString());
            user.IsDeleted = false;
            user.RoleId = 2;

            return user;
        }

        public Company FakeCompany()
        {
            Company c = new Company();
            c.Name = Faker.Company.Name();
            c.Nip = Faker.RandomNumber.Next(1000000000, 1999999999).ToString();
            c.Address = Faker.Address.StreetAddress();
            c.City = Faker.Address.City();
            c.UserId = Faker.RandomNumber.Next(162, 461);
            c.BranchId = Faker.RandomNumber.Next(6, 15);
            c.Branch = null;
            c.User = null;
            c.IsDeleted = false;
            return c;
        }

        public Note FakeNote()
        {
            Note n = new Note();
            n.IsDeleted = false;
            n.User = null;
            n.Company = null;
            n.CompanyId = Faker.RandomNumber.Next(32, 82);
            n.UserId = Faker.RandomNumber.Next(162, 461);
            n.Content = Faker.Lorem.Paragraph(50);
            return n;
        }

        public Contact FakeContact()
        {
            Contact c = new Contact();
            c.User = null;
            c.Company = null;
            c.CompanyId = Faker.RandomNumber.Next(32, 82);
            c.UserId = Faker.RandomNumber.Next(162, 461);
            c.Name = Faker.Name.First();
            c.Surname = Faker.Name.Last();
            c.Email = Faker.Internet.Email();
            c.IsDeleted = false;
            c.Phone = Faker.Phone.Number();
            c.Position = "POSITION_NAME";
            return c;
        }
    }
}
