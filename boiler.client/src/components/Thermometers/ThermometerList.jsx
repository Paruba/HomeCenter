import React, { useState, useEffect } from 'react';
import ThermometerService from './Service/ThermometerService';
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
import InfoIcon from '@mui/icons-material/Info';
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
import KeyIcon from '@mui/icons-material/Key';

const ThermometerList = () => {
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
    const [thermometerToDelete, setToDelete] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchThermometer = async () => {
            try {
                setLoading(true);
                const response = await ThermometerService.getThermometers("", page, rowsPerPage);
                setData(response.items);
                setTotalCount(response.totalCount);
            } catch (err) {
                setError(err);
            } finally {
                setLoading(false);
            }
        };

        fetchThermometer();
    }, [page, rowsPerPage]);

    const handleDetail = (thermometerId) => {
        navigate(`/temperature?thermometerId=${thermometerId}`)
    };

    const handleTokenGenerate = async (deviceId) => {
        navigate(`/device-token?deviceId=${deviceId}&device=thermometer`)
    }

    const handleChangePage = (event, newPage) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    const handleClickOpen = (thermometerId) => {
        setOpen(true);
        setToDelete(thermometerId);
    };

    const createThermometer = () => {
        navigate(`/thermometer-create`)
    };

    const handleClose = () => {
        setOpen(false);
    };

    const handleConfirmDelete = async () => {
        if (thermometerToDelete) {
            await ThermometerService.delete(thermometerToDelete);
            setData(data.filter(thermometer => thermometer.id !== thermometerToDelete));
        }
        setOpen(false);
    };

    if (loading) {
        return <CircularProgress />;
    }

    if (error) {
        return <Typography>Error: {error.message}</Typography>;
    }

    return (
        <React.Fragment>
            <Button variant="contained" color="error" onClick={() => createThermometer()} endIcon={<AddIcon />} style={{ float: 'right', marginBottom: '10px' }}>
                Přidat
            </Button>

            <TableContainer component={Paper} className="TableContainer" style={{ marginTop: '10px' }}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>{"Id"}</TableCell>
                            <TableCell>{ "N\u00E1zev" }</TableCell>
                            <TableCell></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {data.map((thermometer) => (
                            <TableRow key={thermometer.id}>
                                <TableCell align="left">{thermometer.id}</TableCell>
                                <TableCell align="left">{thermometer.name}</TableCell>
                                <TableCell>
                                    <IconButton color="info" onClick={() => handleDetail(thermometer.id)}>
                                        <InfoIcon />
                                    </IconButton>
                                    <IconButton color="success" onClick={() => handleTokenGenerate(thermometer.id)}>
                                        <KeyIcon />
                                    </IconButton>
                                    {isInRole('Admin') && (
                                        <IconButton color="error" onClick={() => handleClickOpen(thermometer.id)}>
                                            <DeleteIcon />
                                        </IconButton>
                                    )}
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

export default ThermometerList;
