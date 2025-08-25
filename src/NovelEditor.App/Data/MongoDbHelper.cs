using MongoDB.Driver;
using NovelEditor.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NovelEditor.Data
{
    /// <summary>
    /// 用于处理与 MongoDB 数据库所有交互的帮助类。
    /// </summary>
    public class MongoDbHelper
    {
        private readonly IMongoCollection<Novel> _novelsCollection;
        private const string ConnectionString = "mongodb://localhost:27017";
        private const string DatabaseName = "NovelEditorDb";
        private const string CollectionName = "Novels";

        /// <summary>
        /// 初始化 MongoDbHelper 类的新实例。
        /// </summary>
        public MongoDbHelper()
        {
            var client = new MongoClient(ConnectionString);
            var database = client.GetDatabase(DatabaseName);
            _novelsCollection = database.GetCollection<Novel>(CollectionName);
        }

        /// <summary>
        /// 从数据库中异步获取所有小说。
        /// </summary>
        /// <returns>一个包含所有小说的列表。</returns>
        public async Task<List<Novel>> GetAllNovelsAsync()
        {
            return await _novelsCollection.Find(_ => true).ToListAsync();
        }

        /// <summary>
        /// 异步创建一个新的小说。
        /// </summary>
        /// <param name="novel">要创建的小说对象。</param>
        public async Task CreateNovelAsync(Novel novel)
        {
            await _novelsCollection.InsertOneAsync(novel);
        }

        /// <summary>
        /// 根据小说 ID 异步删除一本小说。
        /// </summary>
        /// <param name="novelId">要删除的小说的 ID。</param>
        public async Task DeleteNovelAsync(string novelId)
        {
            await _novelsCollection.DeleteOneAsync(n => n.NovelId == novelId);
        }
    }
}
