using ChatNextGame.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatNextGame.DBOperation
{
    public interface IDBOpeartion
    {
         Task AddtoDB(UserMessage userMessage);
    }
}
