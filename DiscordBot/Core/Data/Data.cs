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

namespace DiscordBot.Core.Data
{
    public static class Data
    {
        public static int GetStone(ulong UserID)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Stones.Where(x => x.UserID == UserID).Count() < 1)
                    return 0;
                return DbContext.Stones.Where(x => x.UserID == UserID).Select(x => x.Amount).FirstOrDefault();
            }
        }

        public static async Task SaveStone(ulong UserId, int Amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Stones.Where(x => x.UserID == UserId).Count() < 1)
                {
                    DbContext.Stones.Add(new Stone
                    {
                        UserID = UserId,
                        Amount = Amount
                    });
                }
                else
                {
                    Stone Current = DbContext.Stones.Where(x => x.UserID == UserId).FirstOrDefault();
                    Current.Amount += Amount;
                    DbContext.Stones.Update(Current);
                }
                await DbContext.SaveChangesAsync();
            }
        }

        public static Sticker GetSticker()
        {
            string StickersLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1", @"Data\Stickers.xml"));
            if(!File.Exists(StickersLocation))
            {
                return null;
            }
            // the file exists
            FileStream Stream = new FileStream(StickersLocation, FileMode.Open, FileAccess.Read);
            XmlDocument Doc = new XmlDocument();
            Doc.Load(Stream);
            Stream.Dispose();

            List<Sticker> Stickers = new List<Sticker>();
            foreach (XmlNode Node in Doc.DocumentElement)
                Stickers.Add(new Sticker { name = Node.ChildNodes[0].InnerText, file = Node.ChildNodes[1].InnerText, description = Node.ChildNodes[2].InnerText });

            Random Rand = new Random();
            int Number = Rand.Next(Stickers.Count());

            return Stickers[Number];
        }
    }
}
