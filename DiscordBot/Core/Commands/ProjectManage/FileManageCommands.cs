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
    public class FileManageCommands : ModuleBase<SocketCommandContext>
    {
        [Group("file"), Alias("File, files, Files"), Summary("Group contains all file related command")]
        public class fileGroup : ModuleBase<SocketCommandContext>
        {
            [Command("help"), Summary("list all command related to file")]
            public async Task help()
            {
                await Context.Channel.SendMessageAsync("**file help** : list all command related to file");
                await Context.Channel.SendMessageAsync("**file projectfile 'projectID'** : list all file under a project");
                await Context.Channel.SendMessageAsync("**file createfile 'projectID' 'fileName'** : create a new file under project with ID projectID");
                await Context.Channel.SendMessageAsync("**file filestatus 'projectID' 'fileName' 'status'** : change a file's status");
            }

            [Command("projectfile"), Summary("list file under a project")]
            public async Task projectFile(int projectID)
            {
                EmbedBuilder embed = new EmbedBuilder();
                using (var dbContext = new SqliteDbContext())
                {
                    var list = dbContext.Files.Where(x => x.ProjectId == projectID);
                    var projectSource = dbContext.Projects.Where(x => x.ProjectId == projectID).FirstOrDefault();
                    embed.AddInlineField(projectSource.ProjectName, projectSource.ProjectId);
                    if (list.Count() < 1)
                    {
                        await Context.Channel.SendMessageAsync("The project have no file, create files to user this command again!");
                    }
                    foreach (var item in list)
                    {
                        embed.AddInlineField(item.FileName, item.FileId);
                    }
                }
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }

            [Command("createfile"), Summary("create a new file under a project")]
            public async Task createFile(int projectID, string fileName)
            {
                if (fileName == null)
                {
                    await Context.Channel.SendMessageAsync("file name cannot be null, try createfile projectid **file name**");
                    return;
                }
                using (var dbContext = new SqliteDbContext())
                {
                    if(dbContext.Projects.Where(x=>x.ProjectId == projectID).Count()< 1)
                    {
                        await Context.Channel.SendMessageAsync($"project with id {projectID}, do not exists, please try another ID");
                        return;
                    }

                    int ID = 0;
                    Random Rand = new Random();
                    while (dbContext.Files.Where(x => x.FileId == ID).Count() >= 1)
                    {
                        await Context.Channel.SendMessageAsync("Generating random fileID");
                        ID = Rand.Next(999999999);
                    }

                    dbContext.Files.Add(new ProjectFile
                    {
                        FileId = ID,
                        FileName = fileName,
                        DateCreated = DateTime.Now,
                        LastUpdated = DateTime.Now,
                        isActive = false,
                        ProjectId = projectID
                    });
                    string projectName = dbContext.Projects.Where(x => x.ProjectId == projectID).FirstOrDefault().ProjectName;
                    await Context.Channel.SendMessageAsync($"file **{fileName}** created under project **{projectName}**, ID {projectID}");
                    await dbContext.SaveChangesAsync();
                }
            }

            [Command("removefile"), Summary("remove file in a project")]
            public async Task removeFile(int projectID, int fileID)
            {
                //check if user is project owner
                using (var dbContext = new SqliteDbContext())
                {
                    if(dbContext.Projects.Where(x=>x.ProjectId == projectID).Count() < 1)
                    {
                        await Context.Channel.SendMessageAsync($"project with id {projectID} do not exists, pleaese try another ID!");
                        return;
                    }
                    else if(dbContext.Files.Where(x => x.ProjectId == projectID && x.FileId == fileID).Count() < 1)
                    {
                        await Context.Channel.SendMessageAsync($"There is no file {fileID} in project {projectID}");
                        return;
                    }
                    else if(dbContext.Projects.Where(x => x.ProjectId == projectID).FirstOrDefault().UserId != Context.User.Id)
                    {
                        await Context.Channel.SendMessageAsync("You are not the owner of the project, please contact the owner");
                    }

                    dbContext.Files.Remove(dbContext.Files.Where(x => x.ProjectId == projectID && x.FileId == fileID).FirstOrDefault());
                    await Context.Channel.SendMessageAsync($"File {fileID} removed from project {projectID}!");
                    await dbContext.SaveChangesAsync();
                }
            }

            [Command("filestatus"), Summary("read file stauts")]
            public async Task fileStatus(int projectID, int fileID)
            {
                EmbedBuilder embed = new EmbedBuilder();
                using (var dbContext = new SqliteDbContext())
                {
                    if(dbContext.Files.Where(x => x.ProjectId == projectID && x.FileId == fileID).Count() < 1)
                    {
                        await Context.Channel.SendMessageAsync("no such file");
                        return;
                    }
                    var item = dbContext.Files.Where(x => x.ProjectId == projectID && x.FileId == fileID).FirstOrDefault();
                    embed.AddField("project ID", item.ProjectId);
                    embed.AddField(item.FileName, item.FileId);
                    embed.AddField("Date created", item.DateCreated);
                    embed.AddField("Date updated", item.LastUpdated);
                    embed.AddField("Is active", item.isActive);
                }
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }

            [Command("fileisactive"), Summary("change file status")]
            public async Task fileIsActive(int projectID, int fileID, string newStatus)
            {
                stringToBool isValidInput = new stringToBool();

                isValidInput = projectMethod.stringToBool(newStatus);
                if (!isValidInput.isBool)
                {
                    await Context.Channel.SendMessageAsync("T/F Input is invalid try **true** or **false**");
                    return;
                }

                using (var dbContext = new SqliteDbContext())
                {
                    var list = dbContext.Files.Where(x => x.ProjectId == projectID && x.FileId == fileID);
                    if(list.Count() < 1)
                    {
                        await Context.Channel.SendMessageAsync("target file does not exist, please ensure the input is correct");
                    }

                    //input is false/0 change incomplete back to false
                    if(isValidInput.value)
                    {
                        dbContext.Projects.Where(x => x.ProjectId == projectID).FirstOrDefault().isComplete = false;
                    }
                    var item = list.FirstOrDefault();
                    item.isActive = isValidInput.value;
                    item.LastUpdated = DateTime.Now;
                    await Context.Channel.SendMessageAsync("File status updated");
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
