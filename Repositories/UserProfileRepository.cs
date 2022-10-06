using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Streamish.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

        public List<UserProfile> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id,Name,Email, ImageUrl, DateCreated
                    FROM UserProfile";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var userProfiles = new List<UserProfile>();
                        while (reader.Read())
                        {
                            userProfiles.Add(new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
                            });
                        }
                        return userProfiles;
                    }
                }
            }
        }

        public UserProfile GetById(int Id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id,Name,Email, ImageUrl, DateCreated
                    FROM UserProfile
                    WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@Id", Id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        UserProfile userProfile = null;
                        if (reader.Read())
                        {
                            userProfile = new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated")
                            };
                        }
                        return userProfile;
                    }
                }
            }
        }

        public UserProfile GetUserProfileByIdWithComments(int Id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT u.Id as UserProfileId,u.[Name],u.Email, u.ImageUrl as UserProfileImageUrl, u.DateCreated as UserProfileDateCreated,
                    v.Id as VideoId, v.Title as VideoTitle, v.Description as VideoDescription, v.Url as VideoUrl, v.DateCreated as VideoDateCreated,
                    c.Id as CommentId, c.[Message] as CommentMessage, c.VideoId as CommentVideoId, c.UserProfileId as CommentUserProfile
                    FROM UserProfile u
                    LEFT JOIN Video v on v.UserProfileId = u.Id
                    LEFT JOIN Comment c on c.VideoId = v.Id
                    WHERE u.Id = @Id";

                    DbUtils.AddParameter(cmd,"@Id",Id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var userProfiles = new List<UserProfile>();
                        UserProfile currentUserProfile= null;
                        while (reader.Read())
                        {
                            var userProfileId = DbUtils.GetInt(reader, "UserProfileId");

                            currentUserProfile = userProfiles.FirstOrDefault(p => p.Id == userProfileId);
                            if (currentUserProfile == null)
                            {
                                currentUserProfile = new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "UserProfileId"),
                                    Name = DbUtils.GetString(reader, "Name"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    DateCreated = DbUtils.GetDateTime(reader, "UserProfileDateCreated"),
                                    ImageUrl = DbUtils.GetString(reader, "UserProfileImageUrl"),
                                    Videos = new List<Video>()
                                };

                                userProfiles.Add(currentUserProfile);
                            }

                            var videos = new List<Video>();
                            Video currentVideo = null;
                            if (DbUtils.IsNotDbNull(reader, "VideoId"))
                            {
                                var currentVideoId = DbUtils.GetInt(reader, "VideoId");
                                currentVideo = videos.FirstOrDefault(p => p.Id == currentVideoId);
                                if (currentUserProfile.Videos.Any(v => v.Id == currentVideoId))
                                {
                                    currentUserProfile.Videos.Add(new Video()
                                    {
                                        Id = DbUtils.GetInt(reader, "VideoId"),
                                        Title = DbUtils.GetString(reader, "VideoTitle"),
                                        Description = DbUtils.GetString(reader, "VideoDescription"),
                                        Url = DbUtils.GetString(reader, "VideoUrl"),
                                        DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                        Comments = new List<Comment>()
                                    });

                                    videos.Add(currentVideo);
                                }
                                
                            }

                            if (DbUtils.IsNotDbNull(reader, "CommentId"))
                            {
                                currentVideo.Comments.Add(new Comment()
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "CommentMessage"),
                                    VideoId = DbUtils.GetInt(reader, "CommentVideoId"),
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfile")
                                });
                            }
                        }
                        reader.Close();
                        return currentUserProfile;
                    }
                }
            }
        }

        public void Add(UserProfile userProfile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO UserProfile(Name, Email, ImageUrl, DateCreated)
                        OUTPUT INSERTED.ID
                        VALUES (@Name, @Email, @ImageUrl, @DateCreated)";

                    DbUtils.AddParameter(cmd, "@Name", userProfile.Name);
                    DbUtils.AddParameter(cmd, "@Email", userProfile.Email);
                    DbUtils.AddParameter(cmd, "@ImageUrl", userProfile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@DateCreated", userProfile.DateCreated);

                    userProfile.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(UserProfile userProfile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE UserProfile
                           SET Name = @Name,
                               Email = @Email,
                               ImageUrl = @ImageUrl,
                               DateCreated = @DateCreated
                         WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Name", userProfile.Name);
                    DbUtils.AddParameter(cmd, "@Email", userProfile.Email);
                    DbUtils.AddParameter(cmd, "@ImageUrl", userProfile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@DateCreated", userProfile.DateCreated);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
