using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace NovelEditor.Data.Models
{
    /// <summary>
    /// 小说的增强数据模型
    /// </summary>
    public class Novel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? NovelId { get; set; }

        [BsonElement("novel_title")]
        public string NovelTitle { get; set; } = "未命名小说";

        [BsonElement("author")]
        public string Author { get; set; } = "匿名作者";

        [BsonElement("genre")]
        public string Genre { get; set; } = "未分类";

        [BsonElement("description")]
        public string Description { get; set; } = "暂无简介";

        [BsonElement("cover_image_path")]
        public string? CoverImagePath { get; set; }

        [BsonElement("created_at")]
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("o");

        [BsonElement("updated_at")]
        public string UpdatedAt { get; set; } = DateTime.UtcNow.ToString("o");
    }
}
