import React, { useEffect, useRef, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import TablePagination from '@mui/material/TablePagination';
import IconButton from '@mui/material/IconButton';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import DeleteIcon from '@mui/icons-material/Delete';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Button from '@mui/material/Button';
import CameraService from './Service/CameraService';
import CircularProgress from '@mui/material/CircularProgress';
import Typography from '@mui/material/Typography';
import LiveTvIcon from '@mui/icons-material/LiveTv';
import GlobalKeyHandler from '../../framework/GlobalKeyHandler';
import Checkbox from '@mui/material/Checkbox';
import FaceService from './Service/FaceService';
import { useSearchParams } from 'react-router-dom';
import DoneIcon from '@mui/icons-material/Done';
import ClearIcon from '@mui/icons-material/Clear';
import PortraitIcon from '@mui/icons-material/Portrait';
import Tooltip from '@mui/material/Tooltip';

const CameraDetail = () => {
    GlobalKeyHandler();
    const [data, setData] = useState([]);
    const [searchParams] = useSearchParams();
    const cameraId = searchParams.get('cameraId');
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(5);
    const [totalCount, setTotalCount] = useState(0);
    const [open, setOpen] = React.useState(false);
    const [loading, setLoading] = useState(true);
    const [cameraToDelete, setToDelete] = useState(null);
    const [error, setError] = useState(null);
    const [selected, setSelected] = useState(new Set());
    const navigate = useNavigate();
    const fileInputRef = useRef(null);
    const videoInputRef = useRef(null);
    const [filesUploaded, setFilesUploaded] = useState(false);

    const fetchVideos = async () => {
        try {
            setLoading(true);
            const response = await CameraService.getVideos(cameraId, "", page, rowsPerPage);
            if (response.items) {
                setData(response.items);
            }
            setTotalCount(response.totalCount);
        } catch (err) {
            setError(err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchVideos();
    }, [page, rowsPerPage]);
    
    const handleClose = () => {
        setOpen(false);
    };

    const handleVideoSelect = async (event) => {
        const files = event.target.files;
        const formData = new FormData();
        for (const file of files) {
            formData.append('video', file);
        }
        if (files.length > 0) {
            try {
                setLoading(true);
                await CameraService.uploadVideo(cameraId, formData)
                await fetchVideos();
                setLoading(false);
            } catch (error) {
                console.error('Error during video upload:', error);
            }
        }
    };

    const handleFileSelect = (event) => {
        const files = event.target.files;
        if (files.length > 0) {
            setFilesUploaded(true); // Set file uploaded state
            console.log("Files selected for upload:", files);
        }
    };

    const handleFindFace = async () => {
        const videoIdsQuery = Array.from(selected).join('&videoIds=');
        try {
            setLoading(true);
            const data = await FaceService.getFaceDetection(videoIdsQuery, cameraId);
            console.log(data);
            await fetchVideos();
            setLoading(false);
            setFilesUploaded(false);
        } catch (error) {
            console.error('Error during face detection:', error);
        }
    };

    const handleFindPeople = async () => {
        const videoIdsQuery = Array.from(selected).join('&videoIds=');
        const formData = new FormData();
        for (const file of fileInputRef.current.files) {
            formData.append('targetFaces', file);
        }

        try {
            setLoading(true);
            const response = await FaceService.findPeople(videoIdsQuery, cameraId, formData);
            console.log(response);
            setLoading(false);
        } catch (error) {
            console.error('Error during specific face detection:', error);
        }
    };

    const handleFoundedFaces = async (videoId) => {
        navigate(`/detected-faces?cameraId=${cameraId}&videoId=${videoId}`);
    };

    const triggerFileInput = () => {
        fileInputRef.current.click();
    };

    const triggerVideoInput = () => {
        videoInputRef.current.click();
    };

    const handleSelect = (video) => {
        setSelected(prev => {
            const newSelected = new Set(prev);
            if (newSelected.has(video)) {
                newSelected.delete(video);
            } else {
                newSelected.add(video);
            }
            return newSelected;
        });
    };

    const handleSelectAll = (event) => {
        if (event.target.checked) {
            const newSelected = new Set(data.map(video => video));
            setSelected(newSelected);
            return;
        }
        setSelected(new Set());
    };

    const handleClickOpen = (videoId) => {
        setOpen(true);
        setToDelete(videoId);
    };

    const handleConfirmDelete = async () => {
        if (cameraToDelete) {
            await CameraService.deleteVideo(cameraId, cameraToDelete);
            setData(data.filter(camera => camera.id !== cameraToDelete));
        }
        fetchVideos();
        setOpen(false);
    };

    const handleChangePage = (event, newPage) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    const handleDetail = (videoId) => {
        navigate(`/video-archive?cameraId=${cameraId}&videoId=${videoId}`);
    };

    const liveStream = () => {
        navigate(`/video-stream?cameraId=${cameraId}`);
    }

    if (loading) {
        return <CircularProgress />;
    }

    if (error) {
        return <Typography>Error: {error.message}</Typography>;
    }

    return (
        
        <React.Fragment>
            <Button variant="contained" color="error" onClick={() => liveStream()} endIcon={<LiveTvIcon />} style={{ float: 'right', marginBottom: '10px', marginLeft: '5px' }}>
                Live
            </Button>

            {selected.size > 0 && (
                filesUploaded ? (
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={handleFindPeople}
                        style={{ float: 'right', marginLeft: '5px', marginBottom: '10px' }}
                    >
                        Najdi osoby
                    </Button>
                ) : (
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={handleFindFace}
                        style={{ float: 'right', marginLeft: '5px', marginBottom: '10px' }}
                    >
                        Najdi obličeje
                    </Button>
                )
            )}

            <div>
                <Button
                    variant="contained"
                    color="primary"
                    onClick={triggerVideoInput}
                    style={{ marginBottom: '10px', float: 'right', marginLeft: '5px' }}
                >
                    Upload video
                </Button>
                <input
                    type="file"
                    ref={videoInputRef}
                    style={{ display: 'none', float: 'right', marginLeft: '5px' }}
                    onChange={handleVideoSelect}
                    multiple
                />
            </div>
            
            <Button
                variant="contained"
                color="primary"
                onClick={triggerFileInput}
                style={{ marginBottom: '10px', float: 'right', marginLeft: '5px' }}
            >
                Načtení obličeje
            </Button>
            <input
                type="file"
                ref={fileInputRef}
                style={{ display: 'none', float: 'right', marginLeft: '5px' }}
                onChange={handleFileSelect}
                multiple
            />

            <TableContainer component={Paper} className="TableContainer" style={{ marginTop: '10px' }}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell padding="checkbox">
                                <Checkbox
                                    indeterminate={selected.size > 0 && selected.size < data.length}
                                    checked={data.length > 0 && selected.size === data.length}
                                    onChange={handleSelectAll}
                                />
                            </TableCell>
                            <TableCell>{"N\u00E1zev"}</TableCell>
                            <TableCell></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {data.map((video) => (
                            <TableRow key={video.name}>
                                <TableCell padding="checkbox">
                                    <Checkbox
                                        checked={selected.has(video.name)}
                                        onChange={() => handleSelect(video.name)}
                                    />
                                </TableCell>
                                <TableCell align="left">{video.name}</TableCell>
                                <TableCell align="right">
                                    {video.processed ? (
                                        <div>
                                            <Tooltip title="Data zpracována">
                                                <IconButton color="success">
                                                    <DoneIcon />
                                                </IconButton>
                                            </Tooltip>
                                            <Tooltip title="Nalezené tváře">
                                                <IconButton color="success">
                                                    <PortraitIcon onClick={() => handleFoundedFaces(video.id)} />
                                                </IconButton>
                                            </Tooltip>
                                        </div>
                                    ) : (
                                            <Tooltip title="Data nezpracována">
                                                <IconButton color="warning">
                                                    <ClearIcon />
                                                </IconButton>
                                            </Tooltip>
                                    )}
                                    <IconButton color="info" onClick={() => handleDetail(video.name)}>
                                        <PlayArrowIcon />
                                    </IconButton>
                                    <IconButton color="error" onClick={() => handleClickOpen(video.name)}>
                                        <DeleteIcon />
                                    </IconButton>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
            <TablePagination
                component="div"
                count={totalCount}
                rowsPerPage={rowsPerPage}
                page={page}
                onPageChange={handleChangePage}
                onRowsPerPageChange={handleChangeRowsPerPage}
                className="TablePagination"
            />
            <Dialog
                open={open}
                onClose={handleClose}
                aria-labelledby="alert-dialog-title"
                aria-describedby="alert-dialog-description"
            >
                <DialogTitle id="alert-dialog-title">
                    {"Upozorn\u011Bn\u00ED!"}
                </DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-description">
                        {"P\u0159ejete si vymazat z\u00E1znam? Akce nelze vr\u00E1tit."}
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleConfirmDelete}>Ano</Button>
                    <Button onClick={handleClose} autoFocus>
                        Ne
                    </Button>
                </DialogActions>
            </Dialog>
        </React.Fragment>
    );
};

export default CameraDetail;