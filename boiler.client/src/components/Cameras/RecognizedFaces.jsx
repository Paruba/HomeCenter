import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import FaceService from './Service/FaceService';
import { Grid, Pagination, Box, Button } from '@mui/material';
import base64 from 'react-native-base64'

const RecognizedFaces = () => {
    const [recognizedFaces, setRecognizedFaces] = useState([]);
    const [isLoading, setIsLoading] = useState(true); // Add a loading state
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(8);
    const [searchParams] = useSearchParams();
    const cameraId = searchParams.get('cameraId');
    const videoId = searchParams.get('videoId');
    const [totalCount, setTotalCount] = useState(0);

    useEffect(() => {
        const fetchFaces = async () => {
            setIsLoading(true);
            try {
                const data = await FaceService.getDetectedFaces(videoId, cameraId, page, rowsPerPage);
                setRecognizedFaces(data.items);
                setTotalCount(data.totalCount);
            } catch (error) {
                console.error('Error during face detection:', error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchFaces();
    }, [cameraId, videoId]);

    const ConvertToImageSrc = (base64Image) => {
        return `data:image/png;base64,${base64Image}`
    };

    const handleChangePage = async (event, newPage) => {
        setPage(newPage);
        const data = await FaceService.getDetectedFaces(videoId, cameraId, newPage, rowsPerPage);
        setRecognizedFaces(data.items);
        setTotalCount(data.totalCount);
    };

    const handleChangeRowsPerPage = (event) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    if (isLoading) {
        return <div>Loading...</div>;
    }

    if (!recognizedFaces.length) {
        return <div>Žádná data...</div>;
    }

    return (
        <Box sx={{ flexGrow: 1, position: 'relative' }}>
            <Grid container spacing={2}>
                {Array.isArray(recognizedFaces) && recognizedFaces
                    .slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)
                    .map(face => (
                        <Grid item xs={12} sm={6} md={4} lg={3} key={face.id}>
                            <img src={ConvertToImageSrc(face.frameSrc)} alt={face.fileName} style={{ width: '100%' }} />
                            <p>{face.fileName}</p>
                        </Grid>
                    ))}
            </Grid>
            <Pagination
                count={Math.ceil(totalCount / rowsPerPage)}
                page={page + 1}
                onChange={(event, value) => handleChangePage(event, value - 1)}
                color="primary"
            />
        </Box>
    );

};

export default RecognizedFaces;
