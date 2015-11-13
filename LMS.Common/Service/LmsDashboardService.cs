using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using LMS.Common.DAL;
using LMS.Common.Models;
using LMS.Common.Models.Api;
using LMS.Common.Service.Interface;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace LMS.Common.Service
{
    public class LmsDashboardService : ILmsDashboardService
    {
        private static IDbService _iDbService;

        public LmsDashboardService(IDbService iDbService)
        {
            _iDbService = iDbService;
        }

        public List<Topic> GetCalendar()
        {
            var client = _iDbService.GetDocumentClient(true);

            var dataSelfLink = CloudConfigurationManager.GetSetting("DBSelfLink") ??
                               ConfigurationManager.AppSettings["DBSelfLink"];
            var items =
                client.CreateDocumentQuery<Topic>(dataSelfLink,
                    new FeedOptions {MaxItemCount = 100})
                    .Where(f => f.Type == "Topic")
                    .OrderBy(o => o.Info.due).ToList();
            return items.ToList();
        }

        public LMSresult GetList(string m)
        {
            var end = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            var n = 0;
            var items = new List<PostMessage>();
            while (items.Count < 5 && n < 6)
            {
                end = end - (long) TimeSpan.FromMinutes(30).TotalMilliseconds;
                var path = m.Split('/');
                items = _iDbService.DBoperation().GetPostMessages(end);
                n++;
            }

            var res = new LMSresult
            {
                time = end,
                list = items
            };
            return res;
        }

        public LMSresult GetMoreList(string m, long start)
        {
            var t = (long) (DateTime.UtcNow.AddMonths(-1).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            var items = new List<PostMessage>();
            var t1 = DateTime.Now;
            var n = 1;
            var i = 1;
            var end = start;
            while (items.Count < 5 && end > t)
            {
                i = i + n;
                start = start - (long) TimeSpan.FromHours(i).TotalMilliseconds;
                var path = m.Split('/');
                items = _iDbService.DBoperation().GetPostMessages(start, end);
                var t2 = DateTime.Now;
                if (t2 - t1 > TimeSpan.FromSeconds(10))
                {
                    return new LMSresult
                    {
                        moreData = true,
                        time = end,
                        list = items
                    };
                }
                n++;
            }
            var res = new LMSresult
            {
                moreData = false,
                time = start,
                list = items
            };
            return res;
        }
    }
}