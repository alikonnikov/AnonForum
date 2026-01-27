using Grpc.Core;
using AnonForum.Contracts;
using MongoDB.Driver;
using Server.Models;
using Topic = Server.Models.Topic;
using Message = Server.Models.Message;

namespace Server.Services;

public class ForumService : Forum.ForumBase
{
    private readonly IMongoCollection<Topic> _topics;
    private readonly IMongoCollection<Message> _messages;

    public ForumService(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("AnonForum");
        _topics = database.GetCollection<Topic>("Topics");
        _messages = database.GetCollection<Message>("Messages");
    }

    public override async Task<GetTopicsResponse> GetTopics(GetTopicsRequest request, ServerCallContext context)
    {
        try
        {
            var filter = string.IsNullOrWhiteSpace(request.SearchQuery)
                ? FilterDefinition<Topic>.Empty
                : Builders<Topic>.Filter.Regex(t => t.Title, new MongoDB.Bson.BsonRegularExpression(System.Text.RegularExpressions.Regex.Escape(request.SearchQuery), "i"));

            var topics = await _topics.Find(filter).SortByDescending(t => t.CreatedAt).ToListAsync();

            var response = new GetTopicsResponse();
            response.Topics.AddRange(topics.Select(t => new AnonForum.Contracts.Topic
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CreatedAt = new DateTimeOffset(t.CreatedAt).ToUnixTimeSeconds()
            }));

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG_LOG] Error in GetTopics: {ex}");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<AnonForum.Contracts.Topic> GetTopic(GetTopicRequest request, ServerCallContext context)
    {
        try
        {
            if (!MongoDB.Bson.ObjectId.TryParse(request.Id, out _))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Id"));
            }

            var topic = await _topics.Find(t => t.Id == request.Id).FirstOrDefaultAsync();
            if (topic == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Topic not found"));
            }

            return new AnonForum.Contracts.Topic
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description,
                CreatedAt = new DateTimeOffset(topic.CreatedAt).ToUnixTimeSeconds()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG_LOG] Error in GetTopic: {ex}");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<AnonForum.Contracts.Topic> CreateTopic(CreateTopicRequest request, ServerCallContext context)
    {
        try
        {
            var topic = new Topic
            {
                Title = request.Title,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _topics.InsertOneAsync(topic);

            return new AnonForum.Contracts.Topic
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description,
                CreatedAt = new DateTimeOffset(topic.CreatedAt).ToUnixTimeSeconds()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG_LOG] Error in CreateTopic: {ex}");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<GetMessagesResponse> GetMessages(GetMessagesRequest request, ServerCallContext context)
    {
        try
        {
            if (!MongoDB.Bson.ObjectId.TryParse(request.TopicId, out _))
            {
                return new GetMessagesResponse();
            }

            var messages = await _messages.Find(m => m.TopicId == request.TopicId)
                .SortBy(m => m.CreatedAt)
                .ToListAsync();

            var response = new GetMessagesResponse();
            response.Messages.AddRange(messages.Select(m => new AnonForum.Contracts.Message
            {
                Id = m.Id,
                TopicId = m.TopicId,
                Content = m.Content,
                CreatedAt = new DateTimeOffset(m.CreatedAt).ToUnixTimeSeconds()
            }));

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG_LOG] Error in GetMessages: {ex}");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }

    public override async Task<AnonForum.Contracts.Message> SendMessage(SendMessageRequest request, ServerCallContext context)
    {
        try
        {
            if (!MongoDB.Bson.ObjectId.TryParse(request.TopicId, out _))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid TopicId"));
            }

            var message = new Message
            {
                TopicId = request.TopicId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _messages.InsertOneAsync(message);

            return new AnonForum.Contracts.Message
            {
                Id = message.Id,
                TopicId = message.TopicId,
                Content = message.Content,
                CreatedAt = new DateTimeOffset(message.CreatedAt).ToUnixTimeSeconds()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG_LOG] Error in SendMessage: {ex}");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
    }
}
