using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace QuestBookCreator
{
    interface AbstractBuilder
    {
        string Create(Project curProj); //возвращает путь к созданному файлу
    }
}