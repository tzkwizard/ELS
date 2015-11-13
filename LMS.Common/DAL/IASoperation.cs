using System.Collections.Generic;
using LMS.Common.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace LMS.Common.DAL
{
    public interface IASoperation
    {
        CloudTableClient GetTableClient();
        List<TableChat> GetChat(string roomId, long start, long end);
    }
}