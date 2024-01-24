import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import * as signalR from "@microsoft/signalr";
import Cookies from 'js-cookie';
import CameraService from './Service/CameraService';
import Typography from '@mui/material/Typography';
import GlobalKeyHandler from '../../framework/GlobalKeyHandler';

const ImageComponent = () => {
    GlobalKeyHandler();
    const [searchParams] = useSearchParams();
    const [imageSrc, setImageSrc] = useState('');
    const cameraId = searchParams.get('cameraId');
    const [connection, setConnection] = useState(null);
    const [lastUpdate, setLastUpdate] = useState(null);

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
            const response = await CameraService.getImage(cameraId);
            const base64Image = await response.text();
            setImageSrc(`data:image/png;base64,${base64Image}`);
            setLastUpdate(new Date());
        }
        fetchImage();
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(result => {
                    console.log('Connected!');

                    connection.on("ReceiveImage", (base64Image) => {
                        setImageSrc(`data:image/png;base64,${base64Image}`);
                        setLastUpdate(new Date());
                    });
                })
                .catch(e => console.log('Connection failed: ', e));
        }
    }, [connection]);

    return (
        <div>
            {lastUpdate &&
                <Typography variant="h6" gutterBottom>
                    {"Naposledy nahran\u00FD z\u00E1znam"} {lastUpdate.toLocaleString()}
            </Typography>}
            
            {imageSrc && <img src={imageSrc} alt="Received Image" />}
        </div>
    );
};

export default ImageComponent;
