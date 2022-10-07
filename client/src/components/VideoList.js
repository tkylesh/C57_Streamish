import React, { useEffect, useState } from "react";
import Video from './Video';
import { getAllVideos,searchVideos } from "../modules/videoManager";

const VideoList = () => {
  const [videos, setVideos] = useState([]);

  const getVideos = () => {
    getAllVideos().then(videos => setVideos(videos));
  };

  const searchVids = (event) => {
    let searchTitle = event.target.value;
    
    searchVideos(searchTitle).then(v => setVideos(v));

  }

  useEffect(() => {
    getVideos();
  }, []);

  return (
    <div className="container">
      <div className="row justify-content-center">
        <input type="" placeholder="search videos" onChange={searchVids}/>
        {
            videos.length>0 ?
            videos.map((video) => (
            <Video video={video} key={video.id} />
             ))
            :<h2>No Result Found</h2>
        }
      </div>
    </div>
  );
};

export default VideoList;