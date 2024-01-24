import React from 'react';
import ReactPlayer from 'react-player';
import { useSearchParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import CameraService from './Service/CameraService';
import Typography from '@mui/material/Typography';
import GlobalKeyHandler from '../../framework/GlobalKeyHandler';

const VideoArchive = () => {
    GlobalKeyHandler();
    const [searchParams] = useSearchParams();
    const cameraId = searchParams.get('cameraId');
    const videoId = searchParams.get('videoId');
    const [videoBlobUrl, setVideoBlobUrl] = useState(null);
    const [videoName, setVideoName] = useState(null);

    useEffect(() => {
        if (cameraId && videoId) {
            const fetchVideo = async () => {
                try {
                    const blob = await CameraService.getVideo(cameraId, videoId);
                    const url = URL.createObjectURL(blob);
                    setVideoBlobUrl(url);
                } catch (error) {
                    console.error('Error fetching video', error);
                }
            };
            const fetchName = async () => {
                try {
                    const response = await CameraService.getVideoName(cameraId, videoId);
                    setVideoName(response);
                } catch (error) {
                    console.error('Error fetching video name', error);
                }
            }
            fetchVideo();
            fetchName();
        }
    }, [cameraId, videoId]);

    return (
        <div>
            {videoName &&
                <Typography variant="h6" gutterBottom>
                    {"Z\u00E1znam:"} {videoName}
                </Typography>}
            {videoBlobUrl ? (
                <ReactPlayer url={videoBlobUrl} controls={true} />
            ) : (
                <p>No video found</p>
            )}
        </div>
    );
};

export default VideoArchive;
