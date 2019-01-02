using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DiscordBot.Resources.Database;
using DiscordBot.Resources.Datatypes;
using System.Reflection;
using System.IO;
using System.Xml;

using Discord;

namespace DiscordBot.Core.Data
{
    public struct stringToBool
    {
        public bool isBool;
        public bool value;
    }
    class projectMethod
    {
        public static async Task createUser(IUser User = null)
        {
            using (var dbContext = new SqliteDbContext())
            {
                if(dbContext.Users.Where(x=>x.UserId == User.Id).Count() < 1)
                {
                    dbContext.Users.Add(new User
                    {
                        UserId = User.Id,
                        UserName = User.Username
                    });
                }
                else
                {
                    Console.Write($"User {User.Username} already exists");
                }
                await dbContext.SaveChangesAsync();
            }
        }

        //testing how linq work as sql
        public static async Task printAllProject()
        {
            using (var dbContext = new SqliteDbContext())
            {
                var list = dbContext.Projects;

                foreach(var item in list)
                {
                    Console.WriteLine(item.ProjectName);
                }
            }
        }

        //tell if string is a bool
        public static stringToBool stringToBool(string input)
        {
            stringToBool result = new stringToBool();
            if (input == "true" || input == "True" || input == "1")
            {
                result.isBool = result.value = true;
                return result;
            }
            else if (input == "false" || input == "False" || input == "0")
            {
                result.isBool = true;
                result.value = false;
                return result;
            }
            else
            {
                result.isBool = result.value = false;
                return result;
            }
        }

        public static bool allFileInactive(int projectID)
        {
            using (var dbContext = new SqliteDbContext())
            {
                var list = dbContext.Files.Where(x => x.ProjectId == projectID);
                //loop through all files to see if any is active
                foreach(var item in list)
                {
                    if(item.isActive)
                    {
                        //if a file is still active, return false
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
