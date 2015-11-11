using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using LMS.Common.DAL;
using LMS.Common.Models;
using LMS.Common.Service.Interface;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Partitioning;

namespace LMS.Common.Service
{
    public class ResolverService : IResolverService
    {
        private static string _masterCollectionSelfLink;
        private static IDBoperation _iDBoperation;

        public ResolverService()
        {
            _iDBoperation = new DBoperation();
            _masterCollectionSelfLink =
                ConfigurationManager.AppSettings["MasterCollectionSelfLink"] ??
                CloudConfigurationManager.GetSetting("MasterCollectionSelfLink");
        }

        #region RangePartitionResolver

        public RangePartitionResolver<long> GetResolver()
        {
            var q = _iDBoperation.GetDocumentById(null, "AZresolver");
            return q == null ? null : GenerateResolver(q);
        }

        public async Task<bool> InitResolver()
        {
            //generate
            var rangeResolver = GenerateInitResolver();
            //clean
            await _iDBoperation.DeleteDocById(_masterCollectionSelfLink, "AZresolver");
            //create
            var resolver = new RangeResolver
            {
                id = "AZresolver",
                resolver = rangeResolver
            };
            var res = await _iDBoperation.CreateDocument(resolver);
            return res;
        }

        public async Task<bool> UpdateResolver(DocumentCollection newDc)
        {
            var client = _iDBoperation.GetDocumentClient();
            var oldResolver = GetResolver();
            if (oldResolver == null) return false;

            var newResolver = GetUpdateResolver(oldResolver, newDc);
            if (newResolver == null) return false;

            await _iDBoperation.DeleteDocById(_masterCollectionSelfLink, "AZresolver");


            var resolver = new RangeResolver
            {
                id = "AZresolver",
                resolver = newResolver
            };
            var res = await _iDBoperation.CreateDocument(resolver);
            return res;
        }


        private RangePartitionResolver<long> GenerateResolver(Document q)
        {
            var map = new Dictionary<Range<long>, string>();
            dynamic d = q;
            foreach (var dd in d.resolver.PartitionMap)
            {
                string dz = dd.Name.ToString();
                var l = dz.Split(',');
                map.Add(new Range<long>(Convert.ToInt64(l[0]), Convert.ToInt64(l[1])), dd.Value.ToString());
            }
            var rangeResolver = new RangePartitionResolver<long>(
                u => ((PostMessage) u).Info.timestamp, map);
            return rangeResolver;
        }

        private RangePartitionResolver<long> GetUpdateResolver(RangePartitionResolver<long> oldResolver,
            DocumentCollection newDc)
        {
            var map = new Dictionary<Range<long>, string>();
            var vs = oldResolver.PartitionMap;
            if (vs.Count > 1)
            {
                foreach (var v in vs)
                {
                    if (map.Count < vs.Count - 1)
                    {
                        map.Add(new Range<long>(v.Key.Low, v.Key.High), v.Value);
                    }
                }
            }
            var now = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            if (now < vs.LastOrDefault().Key.Low || now > vs.LastOrDefault().Key.High) return null;

            map.Add(new Range<long>(vs.LastOrDefault().Key.Low, now), vs.LastOrDefault().Value);
            map.Add(new Range<long>(now + 1, vs.LastOrDefault().Key.High), newDc.SelfLink);
            return new RangePartitionResolver<long>(
                u => ((PostMessage) u).Info.timestamp,
                map);
        }

        private RangePartitionResolver<long> GenerateInitResolver()
        {
            var start = (long) (DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            var end = (long) (start + TimeSpan.FromDays(365).TotalMilliseconds);
            return new RangePartitionResolver<long>(
                u => ((PostMessage) u).Info.timestamp,
                new Dictionary<Range<long>, string>()
                {
                    {new Range<long>(start, end), _masterCollectionSelfLink}
                });
        }

        #endregion
    }
}