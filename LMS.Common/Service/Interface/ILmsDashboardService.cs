using System.Collections.Generic;
using LMS.Common.Models;
using LMS.Common.Models.Api;

namespace LMS.Common.Service.Interface
{
    public interface ILmsDashboardService
    {
        LMSresult GetList(string m);
        LMSresult GetMoreList(string m, long start);
        List<Topic> GetCalendar();
    }
}