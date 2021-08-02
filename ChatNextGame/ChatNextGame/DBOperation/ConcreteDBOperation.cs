using ChatNextGame.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatNextGame.DBOperation
{
    public class ConcreteDBOperation : IDBOpeartion
    {
        public async Task AddtoDB(UserMessage userMessage)
        {
            //Add to DB >> Each message string to individual User//
            //SaveMessageToDB(obUserMessage.UserName, obUserMessage.Message);
            await Task.Run(()=> { return; }); 
        }
    }
}
