import * as React from 'react';
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

function TemperatureTable({ data, page, rowsPerPage, count, onChangePage, onChangeRowsPerPage }) {
    if (data === null || data === undefined) {
        return <CircularProgress />;
    }

    if (data.length === 0) {
        return <Typography>No temperature data available.</Typography>;
    }
    return (
        <React.Fragment>
            <TableContainer component={Paper} className="TableContainer">
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Time</TableCell>
                            <TableCell align="right">{"Temperature (" + String.fromCharCode(176) + "C)"}</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {data.map((row, index) => (
                            <TableRow key={index}>
                                <TableCell component="th" scope="row">
                                    {new Date(row.time).toLocaleTimeString()}
                                </TableCell>
                                <TableCell align="right">{row.value}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
            <TablePagination
                component="div"
                count={count}
                rowsPerPage={rowsPerPage}
                page={page}
                onPageChange={onChangePage}
                onRowsPerPageChange={onChangeRowsPerPage}
                className="TablePagination"
            />
        </React.Fragment>
    );
}

export default TemperatureTable;
