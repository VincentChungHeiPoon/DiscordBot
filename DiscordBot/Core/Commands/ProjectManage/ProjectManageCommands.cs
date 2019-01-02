using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core.Data;
using DiscordBot.Resources.Database;

namespace DiscordBot.Core.Commands.ProjectManage
{
    public class ProjectManageCommands : ModuleBase<SocketCommandContext>
    {
        [Group("project"), Alias("Project, projects, Projects"), Summary("Group contains all project related command")]
        public class ProjectGroup : ModuleBase<SocketCommandContext>
        {

            [Command("help"), Summary("List of all commands")]
            public async Task help()
            {
                //projects status
                await Context.Channel.SendMessageAsync($"**project help**: list of all command");
                await Context.Channel.SendMessageAsync($"**project** : list of all owned project by you");
                await Context.Channel.SendMessageAsync($"**project createproject 'user' 'project name**': create a project owned by 'user' named 'project name'");
                await Context.Channel.SendMessageAsync($"**project removeproject 'project id'**: remove a project with id project id if you are the owner");
                await Context.Channel.SendMessageAsync($"**project projectiscomplete 'project id' 'true/false'**: update the status of a project");
            }


            [Command(""), Summary("Shows project owned by user")]
            public async Task ownedProject()
            {
                //var projectList;
                EmbedBuilder embed = new EmbedBuilder();

                using (var dbContext = new SqliteDbContext())
                {
                    var list = dbContext.Projects.Where(x => x.UserId == Context.User.Id);
                    //if user owns 0 project
                    if(list.Count() == 0)
                    {
                        await Context.Channel.SendMessageAsync($"{Context.User.Username} owns 0 project!");
                        return;
                    }
                    foreach(var item in list)
                    {
                        embed.AddInlineField(item.ProjectName, item.ProjectId);
                    }
                }
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }

            [Command("createproject"), Alias("CreateProject"), Summary("Create new project with the command user as owner")]
            public async Task createProject(IUser User = null, String projectName = null)
            {
                //check
                if(User == null && projectName != null)
                {
                    await Context.Channel.SendMessageAsync($"Project {projectName} will be owned by {Context.User.Username}");
                }
                if(User != null && projectName != null)
                {
                    await Context.Channel.SendMessageAsync($"Project {projectName} will be owned by {User.Username}");
                }
                if(User.IsBot)
                {
                    await Context.Channel.SendMessageAsync($"Bot cannot use this command");
                    return;
                }
                if (projectName == null)
                {
                    await Context.Channel.SendMessageAsync("Project name cannot be null, Please use >>project createproject user projectname");
                    return;
                }
                //execution
                using (var dbContext = new SqliteDbContext())
                {
                    //it is a new user
                    if(dbContext.Users.Where(x => x.UserId == User.Id).Count() < 1)
                    {
                        await projectMethod.createUser(Context.User);
                    }

                    //generate random ID
                    int ID = 0;
                    Random Rand = new Random();
                    while (dbContext.Projects.Where(x=>x.ProjectId == ID).Count() >= 1)
                    {
                        await Context.Channel.SendMessageAsync("Generating random ProjectID");
                        ID = Rand.Next(999999999);
                    }
                    ulong userID;
                    if (User == null)
                    {
                        userID = Context.User.Id;
                    }
                    else
                    {
                        userID = User.Id;
                    }
                    dbContext.Projects.Add(new Project
                    {
                        ProjectId = ID,
                        ProjectName = projectName,
                        DateCreated = DateTime.Now,
                        isComplete = false,
                        UserId = userID
                    });
                    //save
                    await dbContext.SaveChangesAsync();
                }             
            }

            [Command("removeproject"), Summary("remove project from the database")]
            public async Task removeProject(int projectID)
            {
                using (var dbContext = new SqliteDbContext())
                {
                    var item = dbContext.Projects.Where(x=>x.ProjectId == projectID).FirstOrDefault();
                    //if user is not the owner, return
                    if(item.UserId != Context.User.Id)
                    {
                        var owner = dbContext.Users.Where(x => x.UserId == item.UserId).FirstOrDefault();
                        await Context.Channel.SendMessageAsync($"You are not **{owner.UserName}**, please contach owner to remove project!");
                        return;
                    }
                    //if user is the owner, proceed
                    else
                    {
                        dbContext.Projects.Remove(item);
                        await dbContext.SaveChangesAsync();
                        await Context.Channel.SendMessageAsync($"Project: **{item.ProjectName}** successfully removed");
                    }

                }
            }

            [Command("projectstatus"), Summary("each project's status")]
            public async Task projectStatus(int projectId)
            {
                EmbedBuilder embed = new EmbedBuilder();
                using (var dbContext = new SqliteDbContext())
                {
                    var item = dbContext.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
                    embed.AddField("Project ID", item.ProjectId);
                    embed.AddField("Project Name", item.ProjectName);
                    embed.AddField("Project Created Date", item.DateCreated);
                    embed.AddField("Project is Complete", item.isComplete);
                }

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
           

            [Command("projectiscomplete"), Summary("change the status of the project")]
            public async Task projectiscomplete(int projectID, string newStatus)
            {
                stringToBool isValidInput = new stringToBool();

                isValidInput = projectMethod.stringToBool(newStatus);
                
                if(!isValidInput.isBool)
                {
                    await Context.Channel.SendMessageAsync("T/F Input is invalid try **true** or **false**");
                    return;
                }

                using (var dbContext = new SqliteDbContext())
                {
                    if(dbContext.Projects.Where(x => x.ProjectId == projectID).Count() < 1)
                    {
                        await Context.Channel.SendMessageAsync("The project do not exist, maybe is a wrong ID");
                    }
                    var item = dbContext.Projects.Where(x => x.ProjectId == projectID).FirstOrDefault();
                    //if user is not the owner, return
                    if (item.UserId != Context.User.Id)
                    {
                        var owner = dbContext.Users.Where(x => x.UserId == item.UserId).FirstOrDefault();
                        await Context.Channel.SendMessageAsync($"You are not **{owner.UserName}**, please contach owner to remove project!");
                        return;
                    }
                    else if (!projectMethod.allFileInactive(projectID))
                    {
                        await Context.Channel.SendMessageAsync("Some files are still active, please set to active to false and try again!");
                        return;
                    }
                    else 
                    {
                        item.isComplete = (isValidInput.value);
                        await dbContext.SaveChangesAsync();
                        await Context.Channel.SendMessageAsync($"Project: **{item.ProjectName}** successfully updated");
                    }                   
                }
            }
        }
    }
}
