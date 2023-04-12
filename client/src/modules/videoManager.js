const baseUrl = '/api/video';

export const getAllVideos = () => {
  return fetch(baseUrl+"/GetWithComments")
    .then((res) => res.json())
};

export const addVideo = (video) => {
  return fetch(baseUrl, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(video),
  });
};

export const searchVideos = (title) => {
    return fetch(baseUrl+"/search?q="+title+"&sortDesc=true")
        .then((res) => res.json())
};

export const getHottest = (since) => {
    return fetch(baseUrl+"/hottest?since="+since)
        .then((res) => res.json())
}