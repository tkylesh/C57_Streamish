using System;
using Streamish.Models;
using System.Collections.Generic;

namespace Streamish.Repositories
{
    public interface IVideoRepository
    {
        void Add(Video video);
        void Delete(int id);
        List<Video> GetAll();
        public List<Video> GetAllWithComments();
        Video GetById(int id);
        Video GetVideoByIdWithComments(int id);
        List<Video> Search(string criterion, bool sortDescending);
        void Update(Video video);
        List<Video> GetHottestVideos(DateTime since);
    }
}