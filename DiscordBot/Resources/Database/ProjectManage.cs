using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscordBot.Resources.Database
{
    public class User
    {
        [Key]
        public ulong UserId { get; set; }
        public string UserName { get; set; }
    }

    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        public DateTime DateCreated { get; set; }
        public string ProjectName { get; set; }

        //close the project and its files, not open for changing status
        public bool isComplete { get; set; }
        //referenct User table
        [ForeignKey("User")]
        public ulong UserId { get; set; }
    }

    public class ProjectFile
    {
        [Key]
        public int FileId { get; set; }
        public string FileName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdated { get; set; }

        //update by user
        public bool isActive { get; set; }

        //reference Project table
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
    }

    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [ForeignKey("User")]
        public ulong UserId { get; set; }
    }

}
