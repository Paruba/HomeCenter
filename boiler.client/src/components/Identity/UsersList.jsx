import React, { useState, useEffect } from 'react';
import UserService from './Service/UserService';
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
import KeyIcon from '@mui/icons-material/Key';
import ManageAccountsIcon from '@mui/icons-material/ManageAccounts';
import { useNavigate } from 'react-router-dom';

const UsersList = () => {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [page, setPage] = useState(0);
    const [rowsPerPage, setRowsPerPage] = useState(5);
    const [totalCount, setTotalCount] = useState(0);
    const [open, setOpen] = React.useState(false);
    const [userToDelete, setUserToDelete] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                setLoading(true);
                const response = await UserService.getUsers("",page, rowsPerPage);
                setData(response.items);
                setTotalCount(response.totalCount);
            } catch (err) {
                setError(err);
            } finally {
                setLoading(false);
            }
        };

        fetchUsers();
    }, [page, rowsPerPage]);

    const handleDetail = (userId) => {
        navigate(`/change-password/${userId}`)
    };

    const handleRole = (userId) => {
        navigate(`/user-detail?userId=${userId}`);
    }

    const handleChangePage = (event, newPage) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPage(0);
    };

    const handleClickOpen = (userId) => {
        setOpen(true);
        setUserToDelete(userId);
    };

    const handleClose = () => {
        setOpen(false);
    };

    const handleConfirmDelete = async () => {
        if (userToDelete) {
            await UserService.delete(userToDelete);
            setData(data.filter(user => user.id !== userToDelete));
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
            <TableContainer component={Paper} className="TableContainer">
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Username</TableCell>
                            <TableCell></TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {data.map((user) => (
                            <TableRow key={user.id}>
                                <TableCell align="left">{user.userName}</TableCell>
                                <TableCell align="right">
                                    <IconButton color="info" onClick={() => handleDetail(user.id)}>
                                        <KeyIcon />
                                    </IconButton>
                                    <IconButton color="success" onClick={() => handleRole(user.id)}>
                                        <ManageAccountsIcon />
                                    </IconButton>
                                    <IconButton color="error" onClick={() => handleClickOpen(user.id)}>
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

export default UsersList;
