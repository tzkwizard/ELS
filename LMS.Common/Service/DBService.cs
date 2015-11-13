using FireSharp.Interfaces;
using LMS.Common.DAL;
using LMS.Common.Service.Interface;
using Microsoft.Azure.Documents.Client;

namespace LMS.Common.Service
{
    public class DbService : IDbService
    {
        private static IDBoperation _iDBoperation;
        private static IFBoperation _iFBoperation;
        private static IResolverService _iResolverService;
        private static ICollectionService _iCollectionService;

        public DbService()
        {
            _iDBoperation = new DBoperation();
            _iFBoperation = new FBoperation();
            _iResolverService = new ResolverService();
            _iCollectionService = new CollectionService();
        }


        //Client
        public IFirebaseClient GetFirebaseClient()
        {
            return _iFBoperation.GetFirebaseClient();
        }

        public DocumentClient GetDocumentClient()
        {
            return _iDBoperation.GetDocumentClient();
        }

        public DocumentClient GetDocumentClient(bool force)
        {
            UpdateDocumentClient();
            return _iDBoperation.GetDocumentClient();
        }

        public void UpdateDocumentClient()
        {
            var rangeResolver = _iResolverService.GetResolver();
            _iDBoperation.UpdateDbClientResolver(rangeResolver);
        }

        //Interface
        public IResolverService RangePartitionResolver()
        {
            return _iResolverService;
        }

        public IDBoperation DBoperation()
        {
            return _iDBoperation;
        }

        public IFBoperation FBoperation()
        {
            return _iFBoperation;
        }

        public ICollectionService CollectionService()
        {
            return _iCollectionService;
        }
    }
}