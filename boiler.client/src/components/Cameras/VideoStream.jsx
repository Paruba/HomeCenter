import React, { useState, useEffect, useRef } from 'react';
import { useSearchParams } from 'react-router-dom';
import * as signalR from "@microsoft/signalr";
import Cookies from 'js-cookie';
import CameraService from './Service/CameraService';
import Typography from '@mui/material/Typography';
import GlobalKeyHandler from '../../framework/GlobalKeyHandler';
import ReactPlayer from 'react-player';

const VideoStream = () => {
    GlobalKeyHandler();
    const [searchParams] = useSearchParams();
    const cameraId = searchParams.get('cameraId');
    const [connection, setConnection] = useState(null);
    const [videoList, setVideoList] = useState([]);
    const [videoBlobUrl, setVideoBlobUrl] = useState(null);
    const [currentVideoIndex, setCurrentVideoIndex] = useState(0);
    const [isVideoPlaying, setIsVideoPlaying] = useState(false);
    const playerRef = useRef(null);

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:8081/streamNotificationHub", {
                accessTokenFactory: () => Cookies.get('AccessToken')
            })
            .configureLogging(signalR.LogLevel.Warning)
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);

        return () => {
            if (newConnection) {
                newConnection.stop();
            }
        }
    }, []);

    useEffect(() => {
        const fetchImage = async () => {
            const blob = await CameraService.getLastVideo(cameraId);
            const url = URL.createObjectURL(blob);
            setVideoBlobUrl(url);
        }
        fetchImage();
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(result => {
                    console.log('Connected!');

                    connection.on("ReceiveVideo", async (videoId) => {
                        const blob = await CameraService.getVideo(cameraId, videoId);
                        const url = URL.createObjectURL(blob);
                        setVideoList(prevList => {
                            const newList = [...prevList, url];
                            if (newList.length === 1 || currentVideoIndex >= prevList.length) {
                                setCurrentVideoIndex(newList.length - 1);
                            }
                            if (!isVideoPlaying || newList.length === 1) {
                                setCurrentVideoIndex(newList.length - 1);
                                setIsVideoPlaying(true);
                            }
                            return newList;
                        });
                    });
                })
                .catch(e => console.log('Connection failed: ', e));
        }
    }, [connection]);

    const handleVideoEnd = () => {
        const nextIndex = currentVideoIndex + 1;
        if (nextIndex < videoList.length) {
            setCurrentVideoIndex(nextIndex);
            setVideoBlobUrl(videoList[nextIndex]);
        } else {
            setIsVideoPlaying(false);
        }
    };

    const handlePlay = () => {
        setIsVideoPlaying(true);
    };

    const handlePause = () => {
        setIsVideoPlaying(false);
    };

    return (
        <div>
            <Typography variant="h6" gutterBottom>
                {videoList.length > 0 && `Video: ${videoList[currentVideoIndex]}`}
            </Typography>
            {videoBlobUrl ? (
                <ReactPlayer
                    ref={playerRef}
                    url={videoBlobUrl}
                    controls={true}
                    playing={true}
                    onEnded={handleVideoEnd}
                    onPlay={handlePlay}
                    onPause={handlePause}
                />
            ) : (
                <p>No video found</p>
            )}
        </div>
    );
};

export default VideoStream;
