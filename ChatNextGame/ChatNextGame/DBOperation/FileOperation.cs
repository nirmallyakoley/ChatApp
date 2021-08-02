using ChatNextGame.BO;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChatNextGame.DBOperation
{
    public class FileOperation : IDBOpeartion
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileOperation(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task AddtoDB(UserMessage userMessage)
        {
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string path = Path.GetFullPath(Path.Combine(contentRootPath,"LogMessage//Chat.txt"));
            File.AppendAllText(path, $"{DateTime.UtcNow}:{userMessage.UserName}:{userMessage.Message}" + Environment.NewLine);
            await Task.Delay(1000);
        }

        
    }
}
