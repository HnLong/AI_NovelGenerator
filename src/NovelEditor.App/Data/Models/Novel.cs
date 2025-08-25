using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NovelEditor.Data.Models
{
    /// <summary>
    /// 小说的基本数据模型。
    /// </summary>
    public class Novel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? NovelId { get; set; }

        [BsonElement("novel_title")]
        public string? NovelTitle { get; set; }

        [BsonElement("created_at")]
        public string? CreatedAt { get; set; }

        [BsonElement("updated_at")]
        public string? UpdatedAt { get; set; }
    }
}
