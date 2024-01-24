import React, { useState, useEffect } from 'react';
import CameraService from './Service/CameraService';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import CircularProgress from '@mui/material/CircularProgress';
import TablePagination from '@mui/material/TablePagination';
import Typography from '@mui/material/Typography';
import IconButton from '@mui/material/IconButton';
import DeleteIcon from '@mui/icons-material/Delete';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Button from '@mui/material/Button';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../Identity/AuthContext';
import AddIcon from '@mui/icons-material/Add';
import GlobalKeyHandler from '../../framework/GlobalKeyHandler';
import SettingsIcon from '@mui/icons-material/Settings';
import SearchIcon from '@mui/icons-material/Search';
import KeyIcon from '@mui/icons-material/Key';

const CameraList = () => {
    GlobalKeyHandler();
    const { isLoggedIn, logout, isInRole } = useAuth();

    if (!isLoggedIn) {
        return (
            <div>
                {"Vítejte, pro zobrazení dat se přihlašte."}
            </div>
        );
    }

    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(5);
    const [totalCount, setTotalCount] = useState(0);
    const [open, setOpen] = React.useState(false);
    const [cameraToDelete, setToDelete] = useState(null);
    const [token, setToken] = useState('');
    const navigate = useNavigate();

    useEffect(() => {
        if (!isInRole('Admin')) {
            navigate('/login');
        }
    }, [navigate, isInRole]);

    useEffect(() => {
        const fetchCamera = async () => {
            try {
                setLoading(true);
                const response = await CameraService.getCameras("", page, rowsPerPage);
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

        fetchCamera();
    }, [page, rowsPerPage]);

    const handleDetail = (cameraId) => {
        navigate(`/camera-detail?cameraId=${cameraId}`)
    };

    const handleSettings = (cameraId) => {
        navigate(`/camera-setting?cameraId=${cameraId}`)
    };

    const handleChangePage = (event, newPage) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    const handleClickOpen = (cameraId) => {
        setOpen(true);
        setToDelete(cameraId);
    };

    const createCamera = () => {
        navigate(`/camera-create`)
    };

    const handleClose = () => {
        setOpen(false);
    };

    const handleConfirmDelete = async () => {
        if (cameraToDelete) {
            await CameraService.delete(cameraToDelete);
            setData(data.filter(camera => camera.id !== cameraToDelete));
        }
        setOpen(false);
    };

    const handleTokenGenerate = async (cameraId) => {
        navigate(`/device-token?deviceId=${cameraId}&device=camera`)
    }

    if (loading) {
        return <CircularProgress />;
    }

    if (error) {
        return <Typography>Error: {error.message}</Typography>;
    }

    if (data.length === 0) {
        return (
            <div>
                <Button variant="contained" color="error" onClick={() => createCamera()} endIcon={<AddIcon />} style={{ marginBottom: '10px' }}>
                    { "P\u0159idat kameru" }
                </Button>
                <Typography>Nenalezena kamera.</Typography>;
            </div>
        );
    }

    return (
        <React.Fragment>
            <Button variant="contained" color="error" onClick={() => createCamera()} endIcon={<AddIcon />} style={{ float: 'right', marginBottom: '10px' }}>
                Přidat
            </Button>

            <TableContainer component={Paper} className="TableContainer" style={{ marginTop: '10px' }}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>{"Id"}</TableCell>
                            <TableCell>{"N\u00E1zev"}</TableCell>
                            <TableCell></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {data.map((camera) => (
                            <TableRow key={camera.id}>
                                <TableCell align="left">{camera.id}</TableCell>
                                <TableCell align="left">{camera.name}</TableCell>
                                <TableCell align="right">
                                    <IconButton color="info" onClick={() => handleDetail(camera.id)}>
                                        <SearchIcon />
                                    </IconButton>
                                    <IconButton color="success" onClick={() => handleSettings(camera.id)}>
                                        <SettingsIcon />
                                    </IconButton>
                                    <IconButton color="success" onClick={() => handleTokenGenerate(camera.id)}>
                                        <KeyIcon />
                                    </IconButton>
                                    <IconButton color="error" onClick={() => handleClickOpen(camera.id)}>
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

export default CameraList;
