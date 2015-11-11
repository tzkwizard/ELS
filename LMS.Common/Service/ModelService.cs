using System;
using LMS.Common.Models;

namespace LMS.Common.Service
{
    public class ModelService
    {
        public static PostMessage PostData(dynamic data, string[] path)
        {
            Random r = new Random();
            PostMessage message = new PostMessage
            {
                Type = "Post",
                Path = new PostPath()
                {
                    District = path[1] + r.Next(1, 51),
                    School = path[2],
                    Classes = path[3]
                },
                Info = new Info()
                {
                    user = data.body.user,
                    uid = data.body.uid,
                    timestamp = data.body.timestamp,
                    message = data.body.message
                }
            };
            return message;
        }

        public static TableChat TableChatData(dynamic room, dynamic message)
        {
            TableChat item = new TableChat(room.Value.room.ID.ToString(), message.Name.ToString())
            {
                roomName = room.Value.room.Name,
                timestamp = message.Value.timestamp,
                user = message.Value.user,
                uid = message.Value.uid,
                message = message.Value.message
            };
            return item;
        }

        public static TablePost TablePostData(dynamic post)
        {
            TablePost item = new TablePost(post.Type, post.id)
            {
                district = post.Path.District,
                school = post.Path.School,
                classes = post.Path.Classes,
                timestamp = post.Info.timestamp,
                user = post.Info.user,
                uid = post.Info.uid,
                message = post.Info.message
            };
            return item;
        }
    }
}